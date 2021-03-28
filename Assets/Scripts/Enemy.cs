using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;
    [SerializeField] private int giveDamage = 1;

    private void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if(transform.position.y < -8f)
        {
            float randomXposition = Random.Range(-11, 11);
            transform.position = new Vector3(randomXposition, 8f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();

            if(player != null)
                player.Damage(giveDamage);

            Destroy(gameObject);
        }

        if (other.gameObject.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);
            Destroy(gameObject);
        }
    }
}
