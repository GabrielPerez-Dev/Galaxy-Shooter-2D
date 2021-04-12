using UnityEngine;

public class CarrierDrone : Enemy
{
    protected override void OnAttack()
    {
        //When it comes close to the player
        //speed up
        //self-destruct
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _player.Damage(1);
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject, 0.2f);
        }

        if (other.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject, 0.2f);
        }
    }
}
