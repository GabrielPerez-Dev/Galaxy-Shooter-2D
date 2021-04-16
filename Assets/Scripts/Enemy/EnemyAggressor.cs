using UnityEngine;

public class EnemyAggressor : Enemy
{
    [Header("Aggressor Properties")]
    [SerializeField] private GameObject _aggressorProjectilePrefab = null;
    [SerializeField] private int numberOfProjectiles = 20;
    [SerializeField] private float radius = 5f;
    [Range(0, 180)] [SerializeField] private float angle = 0;

    protected override void OnAttack()
    {
        if (_movement.IsAggressive() == true) return;

        if (Time.time > _canFire)
        {
            float randomFireRate = Random.Range(1.5f, 3f);

            _canFire = Time.time + randomFireRate;

            radius = 5f;

            float angleStep = 45f / numberOfProjectiles;
            angle = 165f;

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                float projectileDirXpos = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
                float projectileDirYpos = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180) * radius;

                Vector3 projectilVector = new Vector3(projectileDirXpos, projectileDirYpos, 0);
                Vector3 projectileMoveDir = (projectilVector - transform.position).normalized * 4f;

                var projectile = Instantiate(_aggressorProjectilePrefab, transform.position, Quaternion.identity);
                if (projectile != null)
                    projectile.GetComponent<Rigidbody2D>().velocity = new Vector3(projectileMoveDir.x, projectileMoveDir.y, 0);

                angle += angleStep;

                Projectile[] projectiles = projectile.GetComponents<Projectile>();

                for (int x = 0; x < projectiles.Length; x++)
                {
                    projectiles[x].IsEnemyLaser();
                }
            }
        }
    }
}
