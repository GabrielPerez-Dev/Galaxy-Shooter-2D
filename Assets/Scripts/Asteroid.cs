using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 1f;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private GameObject _childObject = null;
    [SerializeField] private GameObject _explosionPrefab = null;
    [SerializeField] private Collider2D _collider = null;

    private AudioManager _audioManager = null;
    private Player _player = null;

    private void Awake()
    {
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if (_audioManager == null)
            Debug.LogError("AudioManager is null");

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
            Debug.LogError("Player script is null");
    }

    private void Start()
    {
        float randomValue = Random.Range(1f, 5f);
        _speed = randomValue;
    }

    private void Update()
    {
        _childObject.transform.Rotate(Vector3.forward * _rotateSpeed * Time.deltaTime);
        transform.Translate(Vector3.down * _speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Projectile"))
        {
            Vector3 offsetY = new Vector3(0, -1f, 0);
            Instantiate(_explosionPrefab, transform.position + offsetY, Quaternion.identity);

            _player.AddScore(13);

            //Disable the Collider so we don't Instantiate the Explosion more than once.
            _collider.enabled = false;

            _audioManager.PlayExplosionSound();

            Destroy(other.gameObject);
            Destroy(gameObject, 0.3f);
        }

        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();

            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            player.Damage(1);

            _audioManager.PlayExplosionSound();

            Destroy(gameObject, 0.3f);
        }
    }
}
