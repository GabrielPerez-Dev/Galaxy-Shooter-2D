using System.Collections;
using UnityEngine;

public class EnemyBoss : Enemy
{
    [Header("Boss Settings")]
    [SerializeField] public Transform       _firePoint              = null;

    [Header("Spawn Drone Settings")]
    [SerializeField] private GameObject     _dronePrefab            = null;
    [SerializeField] private float          _spawnDroneTime         = 6f;
    [SerializeField] private Transform[]    _spawnLocations         = null;

    [Header("Projectile Settings")]
    [SerializeField] private GameObject     _firePointOnePrefab     = null;
    [SerializeField] private GameObject     _finalShotsPrefab       = null;
    [SerializeField] private int            _numberOfProjectiles    = 20;
    [SerializeField] private float          _radius                 = 5f;
    [Range(0, 180)] 
    [SerializeField] private float          _angle                  = 0;
    private float                           _droneRate              = 8f;
    private float                           _canSpawnDrone          = 0;

    private void Start()
    {
        transform.position = new Vector3(0, 9, 0);
        _movement.SetMovementType(EnemyMovementType.ToAnchorPoint);
    }

    private void Update()
    {
        OnAttack();

        if (_shieldStrength == 0)
        {
            _movement.SetMovementType(EnemyMovementType.Panic);
        }

        _movement.GetMovementType();
    }

    protected override void OnAttack()
    {
        if (Time.time > _canFire && _movement.IsPatrolling())
        {
            float randomFireRate = UnityEngine.Random.Range(1.5f, 2f);

            _canFire = Time.time + randomFireRate;

            _radius = 5f;

            float angleStep = 360f / _numberOfProjectiles;
            _angle = 0f;

            for (int i = 0; i < _numberOfProjectiles; i++)
            {
                float projectileDirXpos = transform.position.x + Mathf.Sin((_angle * Mathf.PI) / 180) * _radius;
                float projectileDirYpos = transform.position.y + Mathf.Cos((_angle * Mathf.PI) / 180) * _radius;

                Vector3 projectilVector = new Vector3(projectileDirXpos, projectileDirYpos, 0);
                Vector3 projectileMoveDir = (projectilVector - transform.position).normalized * 3f;

                GameObject projectile = Instantiate(_firePointOnePrefab, _firePoint.position, Quaternion.identity);

                if (projectile != null)
                    projectile.GetComponent<Rigidbody2D>().velocity = new Vector3(projectileMoveDir.x, projectileMoveDir.y, 0);

                _angle += angleStep;

                Projectile[] projectiles = projectile.GetComponents<Projectile>();

                for (int x = 0; x < projectiles.Length; x++)
                {
                    projectiles[x].IsEnemyLaser();
                }
            }
        } 
        else if(Time.time > _canFire && !_movement.IsPatrolling())
        {
            _fireRate = 1.5f;
            _canFire = Time.time + _fireRate;
            var enemyProjectile = Instantiate(_finalShotsPrefab, transform.position, Quaternion.identity);

            Projectile[] projectiles = enemyProjectile.GetComponents<Projectile>();
            for (int i = 0; i < projectiles.Length; i++)
            {
                projectiles[i].IsEnemyLaser();
            }
        }

        if (Time.time > _canSpawnDrone)
        {
            _canSpawnDrone = Time.time + _droneRate;
            StartCoroutine(SpawnDronesRoutine());
        }
    }

    private IEnumerator SpawnDronesRoutine()
    {
        int count = 0;
        count++;

        yield return new WaitForSeconds(_spawnDroneTime);

        for (int i = 0; i < _spawnLocations.Length; i++)
        {
            if (_player == null) break;

            Instantiate(_dronePrefab, _spawnLocations[i].position, Quaternion.identity);
        }

        yield return null;
    }
}
