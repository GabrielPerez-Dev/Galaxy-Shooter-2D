using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfantryVariant : Enemy
{
    [Header("Enemy Infantry Variant Settings")]
    [SerializeField] private GameObject _laserPrefab = null;

    protected override void OnAttack()
    {
        if (Time.time > _canFire && !_isDead)
        {
            _fireRate = Random.Range(3f, 6f);
            _canFire = Time.time + _fireRate;

            //Vector3 offsetY = new Vector3(0, -0.3f, 0);
            var enemeyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Projectile[] lasers = enemeyLaser.GetComponents<Projectile>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].IsEnemyLaser();
            }
        }
    }
}
