using UnityEngine;

public enum PowerupType 
{ 
    None, 
    TripleShot, 
    Speed, 
    Shield,
    Health,
    Ammo,
    GodsWish,
    EatThis,
    NegateSpeed,
}

public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerupType _powerupType = PowerupType.None;
    [SerializeField] private float _spawnRate = 0f;
    [SerializeField] private float _speed = 3f;

    private AudioManager _audioManager = null;
    private Player _player = null;

    private void Awake()
    {
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if (_audioManager == null)
            Debug.Log("AudioManager is null");

        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
            Debug.Log("Player is null");
    }

    private void Update()
    {
        if(!_player.IsPickingUpPowerUp())
            transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if(transform.position.y < -8f)
        {
            Destroy(gameObject);
        }
    }

    public float GetSpawnRate()
    {
        return _spawnRate;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.transform.GetComponent<Player>();

            if(player != null)
            {
                switch (_powerupType)
                {
                    case PowerupType.TripleShot:
                        player.TripleShotActive();
                        break;
                    case PowerupType.Speed:
                        player.SpeedBoostActive();
                        break;
                    case PowerupType.Shield:
                        player.ShieldActive();
                        break;
                    case PowerupType.Health:
                        player.HealthActive();
                        break;
                    case PowerupType.Ammo:
                        player.AmmoActive();
                        break;
                    case PowerupType.GodsWish:
                        player.GodsWishActive();
                        break;
                    case PowerupType.EatThis:
                        player.EatThisActive();
                        break;
                    case PowerupType.NegateSpeed:
                        player.SpeedNegateActive();
                        break;
                    default:
                        _powerupType = PowerupType.None;
                        break;
                }
            }

            _audioManager.PlayPowerupSound();

            Destroy(gameObject);
        }
    }
}
