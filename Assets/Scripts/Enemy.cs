using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _speed = 4f;
    [SerializeField] private int giveDamage = 1;
    [SerializeField] private int _givePoints = 10;

    private AudioManager _audioManager = null;
    private Animator _animator = null;
    private Player _player = null;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null) 
            Debug.LogError("The Animator is NULL");

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) 
            Debug.LogError("The Player is NULL");

        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if (_audioManager == null)
            Debug.LogError("AudioManager is null");
    }

    private void Start()
    {
        float randomValue = Random.Range(3f, 5f);
        _speed = randomValue;
    }

    private void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if(transform.position.y < -8f)
        {
            if (_player.IsDead()) return;

            float randomXposition = Random.Range(-11, 11);
            transform.position = new Vector3(randomXposition, 8f, 0);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage(giveDamage);
                player.AddScore(-10);

                if(player.GetScore() <= 0)
                {
                    player.SetScore(0);
                }
            }

            _animator.SetTrigger("isDestroyed");
            _speed = 1f;
            _audioManager.PlayExplosionSound();
            Destroy(gameObject, 2f);
        }

        if (other.gameObject.CompareTag("Projectile"))
        {
            Destroy(other.gameObject);

            if (_player != null)
                _player.AddScore(_givePoints);

            _animator.SetTrigger("isDestroyed");
            _speed = 1f;
            _audioManager.PlayExplosionSound();
            Destroy(gameObject, 2f);
        }
    }
}
