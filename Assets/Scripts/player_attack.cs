using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    [Header("Light Attack")]
    public int lightDamage = 10;
    public float lightCooldown = 0.6f;
    public float lightRange = 2f;
    public float lightRadius = 0.75f;

    [Header("Heavy Attack")]
    public int heavyDamage = 25;
    public float heavyCooldown = 1.2f;
    public float heavyChargeTime = 0.8f;
    public float heavyRadius = 1.2f;

    [Header("Spin Attack")]
    public float spinRadius = 2f;
    public float spinDuration = 0.8f;

    [Header("Combo Settings")]
    public float comboResetTime = 3f;

    [Header("Debug Visuals")]
    public Color outlineColor = new Color(1f, 0.3f, 0f, 1f);   // orange ring while active
    public Color flashColor   = new Color(1f, 0f,   0f, 0.35f); // red transparent flash on hit
    public float flashDuration = 0.12f;
    public int circleSegments = 48;

    // ---- runtime state ----
    private float lastLightTime;
    private float lastHeavyTime;
    private int   heavyComboCount;
    private bool  isHeavyAttacking;

    // ---- visual state ----
    private bool  drawOutline;
    private float outlineRadius;
    private Vector3 outlineOrigin;

    // each flash: origin, radius, expiry time
    private struct FlashData { public Vector3 origin; public float radius; public float expiryTime; }
    private List<FlashData> activeFlashes = new List<FlashData>();

    private Material glMat;

    PlayerLocalmotion movement;
    Rigidbody rb;

    // ------------------------
    // SETUP
    // ------------------------

    void Awake()
    {
        movement = GetComponent<PlayerLocalmotion>();
        rb       = GetComponent<Rigidbody>();
        BuildGLMaterial();
    }

    void BuildGLMaterial()
    {
        // Built-in unlit transparent shader — no asset needed
        Shader shader = Shader.Find("Hidden/Internal-Colored");
        glMat = new Material(shader);
        glMat.hideFlags = HideFlags.HideAndDontSave;
        glMat.SetInt("_SrcBlend",  (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        glMat.SetInt("_DstBlend",  (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        glMat.SetInt("_Cull",      (int)UnityEngine.Rendering.CullMode.Off);
        glMat.SetInt("_ZWrite",    0);
        glMat.SetInt("_ZTest",     (int)UnityEngine.Rendering.CompareFunction.Always);
    }

    // ------------------------
    // INPUT
    // ------------------------

    void Update()
    {
        HandleInput();
        CleanExpiredFlashes();
    }

    void HandleInput()
    {
        if (Keyboard.current.spaceKey.wasPressedThisFrame) TryLightAttack();
        if (Keyboard.current.hKey.wasPressedThisFrame)     TryHeavyAttack();
    }

    // ------------------------
    // LIGHT ATTACK
    // ------------------------

    void TryLightAttack()
    {
        if (Time.time < lastLightTime + lightCooldown) return;
        lastLightTime = Time.time;

        Vector3 origin = GetForwardAttackOrigin(lightRange);
        ShowOutline(origin, lightRadius);
        DealDamage(origin, lightRadius, lightDamage);
        HideOutline();

        Debug.Log("Light Attack");
    }

    // ------------------------
    // HEAVY ATTACK
    // ------------------------

    void TryHeavyAttack()
    {
        if (Time.time < lastHeavyTime + heavyCooldown || isHeavyAttacking) return;
        StartCoroutine(HeavyAttackRoutine());
    }

    IEnumerator HeavyAttackRoutine()
    {
        isHeavyAttacking  = true;
        movement.canMove  = false;

        // Show outline during charge
        Vector3 chargeOrigin = GetForwardAttackOrigin(lightRange);
        ShowOutline(chargeOrigin, heavyRadius);

        Debug.Log("Charging Heavy Attack...");
        yield return new WaitForSeconds(heavyChargeTime);

        HideOutline();
        HandleComboReset();
        heavyComboCount++;

        if (heavyComboCount % 3 == 0)
            yield return SpinAttackRoutine();
        else
            PerformHeavyAttack();

        lastHeavyTime     = Time.time;
        movement.canMove  = true;
        isHeavyAttacking  = false;
    }

    void PerformHeavyAttack()
    {
        Vector3 origin = GetForwardAttackOrigin(lightRange);
        DealDamage(origin, heavyRadius, heavyDamage);
        Debug.Log("Heavy Attack");
    }

    // ------------------------
    // SPIN ATTACK
    // ------------------------

    IEnumerator SpinAttackRoutine()
    {
        float elapsed = 0;
        Quaternion rotationBeforeSpin = transform.rotation; // save facing direction

        while (elapsed < spinDuration)
        {
            rb.linearVelocity = Vector3.zero;
            transform.Rotate(Vector3.up * 720 * Time.deltaTime);

            // Outline tracks player position live during spin
            ShowOutline(transform.position, spinRadius);

            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.rotation = rotationBeforeSpin; // restore facing direction

        HideOutline();
        DealDamage(transform.position, spinRadius, heavyDamage);
        Debug.Log("Spin Attack!");
    }

    // ------------------------
    // DAMAGE
    // ------------------------

    void DealDamage(Vector3 origin, float radius, int damage)
    {
        Collider[] hits = Physics.OverlapSphere(origin, radius);
        bool anyHit = false;

        foreach (Collider hit in hits)
        {
            if (hit.CompareTag("enemy"))
            {
                hit.GetComponent<Enemyheath>()?.takedamage(damage);
                anyHit = true;
            }
        }

        if (anyHit)
            RegisterFlash(origin, radius);
    }

    // ------------------------
    // VISUAL STATE HELPERS
    // ------------------------

    void ShowOutline(Vector3 origin, float radius)
    {
        drawOutline   = true;
        outlineOrigin = origin;
        outlineRadius = radius;
    }

    void HideOutline() => drawOutline = false;

    void RegisterFlash(Vector3 origin, float radius)
    {
        activeFlashes.Add(new FlashData
        {
            origin     = origin,
            radius     = radius,
            expiryTime = Time.time + flashDuration
        });
    }

    void CleanExpiredFlashes()
    {
        activeFlashes.RemoveAll(f => Time.time >= f.expiryTime);
    }

    // ------------------------
    // GL DRAWING
    // ------------------------

    void OnRenderObject()
    {
        if (glMat == null) return;

        glMat.SetPass(0);

        // Draw outline ring
        if (drawOutline)
            DrawCircleGL(outlineOrigin, outlineRadius, outlineColor, filled: false);

        // Draw hit flashes
        foreach (var flash in activeFlashes)
            DrawCircleGL(flash.origin, flash.radius, flashColor, filled: true);
    }

    void DrawCircleGL(Vector3 center, float radius, Color color, bool filled)
    {
        float y = center.y + 0.05f; // slight lift so it doesn't z-fight with floor

        GL.Begin(filled ? GL.TRIANGLES : GL.LINE_STRIP);
        GL.Color(color);

        if (filled)
        {
            // Fan triangles from center outward
            for (int i = 0; i < circleSegments; i++)
            {
                float a0 = (i)     / (float)circleSegments * Mathf.PI * 2f;
                float a1 = (i + 1) / (float)circleSegments * Mathf.PI * 2f;

                GL.Vertex3(center.x, y, center.z);
                GL.Vertex3(center.x + Mathf.Cos(a0) * radius, y, center.z + Mathf.Sin(a0) * radius);
                GL.Vertex3(center.x + Mathf.Cos(a1) * radius, y, center.z + Mathf.Sin(a1) * radius);
            }
        }
        else
        {
            // Closed loop of line segments
            for (int i = 0; i <= circleSegments; i++)
            {
                float a = i / (float)circleSegments * Mathf.PI * 2f;
                GL.Vertex3(center.x + Mathf.Cos(a) * radius, y, center.z + Mathf.Sin(a) * radius);
            }
        }

        GL.End();
    }

    // ------------------------
    // EDITOR GIZMOS (design time only)
    // ------------------------

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(GetForwardAttackOrigin(lightRange), lightRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(GetForwardAttackOrigin(lightRange), heavyRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, spinRadius);
    }

    // ------------------------
    // HELPERS
    // ------------------------

    Vector3 GetForwardAttackOrigin(float distance)
    {
        Vector3 dir = transform.forward;
        dir.y = 0;
        dir.Normalize();
        return transform.position + dir * distance;
    }

    void HandleComboReset()
    {
        if (Time.time > lastHeavyTime + comboResetTime)
            heavyComboCount = 0;
    }
}