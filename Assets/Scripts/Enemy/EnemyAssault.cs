using UnityEngine;

public class EnemyAssault : Enemy
{
    [Header("Projectile Settings")]
    [SerializeField] protected GameObject _assaultProjectilePrefab = null;
    [SerializeField] private int numberOfProjectiles = 20;
    [SerializeField] private float radius = 5f;
    [Range(0, 180)] [SerializeField] private float angle = 0;

    protected override void OnAttack()
    {
        if(Time.time > _canFire && _movement.IsHovering() == true)
        {
            float randomFireRate = Random.Range(1.5f, 3f);

            _canFire = Time.time + randomFireRate;

            radius = 5f;

            float angleStep = 180f / numberOfProjectiles;
            angle = 100f;

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                float projectileDirXpos = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
                float projectileDirYpos = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180) * radius;

                Vector3 projectilVector = new Vector3(projectileDirXpos, projectileDirYpos, 0);
                Vector3 projectileMoveDir = (projectilVector - transform.position).normalized * 3f;

                var projectile = Instantiate(_assaultProjectilePrefab, transform.position, Quaternion.identity);
                if(projectile != null)
                    projectile.GetComponent<Rigidbody2D>().velocity = new Vector3(projectileMoveDir.x, projectileMoveDir.y, 0);

                angle += angleStep;

                Projectile[] projectiles = projectile.GetComponents<Projectile>();

                for (int x = 0; x < projectiles.Length; x++)
                {
                    projectiles[x].IsEnemyLaser();
                }
            }
        }

        if(Time.time > _canFire && !_movement.IsHovering())
        {
            float randomFireRate = Random.Range(1f, 2f);

            _canFire = Time.time + randomFireRate;

            int numberOfProjectiles = 4;

            float radius = 5f;

            float angleStep = 45f / numberOfProjectiles;
            float angle = 160f;

            for (int i = 0; i < numberOfProjectiles; i++)
            {
                float projectileDirXpos = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
                float projectileDirYpos = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180) * radius;

                Vector3 projectilVector = new Vector3(projectileDirXpos, projectileDirYpos, 0);
                Vector3 projectileMoveDir = (projectilVector - transform.position).normalized * 3f;

                var projectile = Instantiate(_assaultProjectilePrefab, transform.position, Quaternion.identity);
                if(projectile != null)
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
