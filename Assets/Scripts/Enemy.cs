using System.Collections;
using UnityEngine;

public enum EnemyType { Default, }

public class Enemy : MonoBehaviour
{
    [SerializeField] private EnemyType  _enemyType      = EnemyType.Default;
    [SerializeField] private GameObject _laserPrefab    = null;
    [SerializeField] private int        _giveDamage     = 1;
    [SerializeField] private int        _givePoints     = 10;
    [SerializeField] private int        _giveAmmo       = 1;
    [SerializeField] private Collider2D _collider       = null;

    private EnemyMovement _movement = null;
    private AudioManager _audioManager = null;
    private Animator _animator = null;
    private Player _player = null;

    private float _fireRate = 3.0f;
    private float _canFire = -1f;

    private bool _isDead = false;

    private void Awake()
    {
        _movement = GetComponent<EnemyMovement>();
        if (_movement == null)
            Debug.Log("EnemyMovement is null");

        _animator = GetComponentInChildren<Animator>();
        if (_animator == null) 
            Debug.LogError("The Animator is NULL");

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null) 
            Debug.LogError("The Player is NULL");

        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if (_audioManager == null)
            Debug.LogError("AudioManager is null");
    }

    private void Update()
    {
        if(Time.time > _canFire && !_isDead)
        {
            _fireRate = Random.Range(3f, 6f);
            _canFire = Time.time + _fireRate;

            Vector3 offsetY = new Vector3(0, -0.5f, 0);
            var enemeyLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);
            Projectile[] lasers = enemeyLaser.GetComponentsInChildren<Projectile>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].IsEnemyLaser();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _isDead = true;

            Player player = other.transform.GetComponent<Player>();

            if (player != null)
            {
                player.Damage(_giveDamage);
                player.AddScore(-10);

                if(player.GetScore() <= 0)
                {
                    player.SetScore(0);
                }
            }

            _animator.SetTrigger("isDestroyed");
            _movement.Speed = 1f;
            _audioManager.PlayExplosionSound();
            Destroy(gameObject, 2f);
        }

        if (other.gameObject.CompareTag("Projectile"))
        {
            _isDead = true;

            Destroy(other.gameObject);

            if (_player != null)
            {
                _player.AddScore(_givePoints);
                _player.AddAmmo(_giveAmmo);
            }

            _animator.SetTrigger("isDestroyed");
            _movement.Speed = 1f;
            _audioManager.PlayExplosionSound();

            _collider.enabled = false;

            Destroy(gameObject, 2f);
        }

        if(other.gameObject.CompareTag("Enemy Projectile"))
        {
            return;
        }
    }
}
