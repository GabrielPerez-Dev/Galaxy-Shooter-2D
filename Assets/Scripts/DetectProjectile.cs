using UnityEngine;

public class DetectProjectile : MonoBehaviour
{
    private EnemyMovement _movement = null;

    private void Awake()
    {
        _movement = GetComponentInParent<EnemyMovement>();
        if (_movement == null)
            Debug.Log("EnemyMovement is null");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _movement.SetIsSwitching(true);
        _movement.SetMovementType(EnemyMovementType.Juke);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _movement.SetIsSwitching(false);
        _movement.SetMovementType(EnemyMovementType.Default);
    }
}
