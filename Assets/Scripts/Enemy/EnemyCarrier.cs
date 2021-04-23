using System.Collections;
using UnityEngine;

public class EnemyCarrier : Enemy
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject _carrierQuadPrefab = null;

    [Header("Spawn Drone Settings")]
    [SerializeField] private GameObject _dronePrefab = null;
    [SerializeField] private float _spawnDroneTime = 24f;
    [SerializeField] protected Transform[] _spawnLocations = null;

    private void Start()
    {
        StartCoroutine(SpawnDronesRoutine());
    }

    protected override void OnAttack()
    {
        if(Time.time > _canFire)
        {
            _canFire = Time.time + _fireRate;
            var enemyProjectile = Instantiate(_carrierQuadPrefab, transform.position, Quaternion.identity);

            Projectile[] projectiles = enemyProjectile.GetComponents<Projectile>();
            for (int i = 0; i < projectiles.Length; i++)
            {
                projectiles[i].IsEnemyLaser();
            }
        }
    }

    protected IEnumerator SpawnDronesRoutine()
    {
        int count = 0;

        while (count <= 6 && !_player.IsDead())
        {
            count++;

            yield return new WaitForSeconds(_spawnDroneTime);

            for (int i = 0; i < _spawnLocations.Length; i++)
            {
                if (_player == null) break;

                Instantiate(_dronePrefab, _spawnLocations[i].position, Quaternion.identity);
            }
        }
    }
}
