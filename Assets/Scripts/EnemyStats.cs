using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Physics")]
    [Tooltip("Heavier enemies get pushed less. Light ghost = 1, Heavy zombie = 5+")]
    public float weight = 3f;
}