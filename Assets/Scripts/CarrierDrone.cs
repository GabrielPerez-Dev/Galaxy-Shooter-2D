using System.Collections;
using UnityEngine;

public class CarrierDrone : Enemy
{
    [SerializeField] private GameObject _explosionDronePrefab = null;

    private bool isCloseToPlayer = false;

    protected override void OnAttack()
    {
        if (_player == null)
        {
            DamageDrone();
        }

        if (_player != null)
        {
            if (Vector3.Distance(_player.transform.position, transform.position) < 3f && !isCloseToPlayer && _player != null)
            {
                isCloseToPlayer = true;
                StartCoroutine(DroneKamikazeRoutine());
            }
        }


    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _player.Damage(1);
            _player.AddScore(-15);
            DamageDrone();
        }

        if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);
            _player.AddScore(15);
            DamageDrone();
        }
    }

    public void DamageDrone()
    {
        Instantiate(_explosionDronePrefab, transform.position, Quaternion.identity);
        _audioManager.PlayDroneExplosionSound();
        Destroy(gameObject, 0.2f);
    }

    private IEnumerator DroneKamikazeRoutine()
    {
        _movement.Speed = 5f;
        yield return new WaitForSeconds(0.4f);

        DamageDrone();
    }
}
