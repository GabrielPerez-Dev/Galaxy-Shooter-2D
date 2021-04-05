using UnityEngine;

public enum PowerupType 
{ 
    None, 
    TripleShot, 
    Speed, 
    Shield,
    Health,
    Ammo,
}

public class PowerUp : MonoBehaviour
{
    [SerializeField] private PowerupType _powerupType = PowerupType.None;
    [SerializeField] private float _speed = 3f;

    private AudioManager _audioManager = null;

    private void Awake()
    {
        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
    }

    private void Update()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if(transform.position.y < -8f)
        {
            Destroy(gameObject);
        }
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
