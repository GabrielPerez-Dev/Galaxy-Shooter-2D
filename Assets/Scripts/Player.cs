using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int            _lives              = 3;
    [SerializeField] private int            _score              = 0;
    [SerializeField] private int            _ammoCount          = 0;
    [SerializeField] private int            _maxAmmoCount       = 15;
    [SerializeField] private float          _speed              = 3.5f;
    [SerializeField] private float          _speedBoostAmount   = 5f;
    [SerializeField] private float          _fireRate           = 0.15f;
    [SerializeField] private float          _powerDownTime      = 5f;
    [SerializeField] private GameObject     _ExplosionPrefab    = null;
    [SerializeField] private GameObject     _laserPrefab        = null;
    [SerializeField] private GameObject     _triplShotPrefab    = null;
    [SerializeField] private GameObject     _shieldPrefab       = null;
    [SerializeField] private GameObject[]   _enginePrefabs      = null;

    private bool _isTripleShotActive    = false;
    private bool _isShieldActive        = false;
    private bool _isDead                = false;
    private bool _ammoEmpty             = false;

    private AudioManager    _audioManager   = null;
    private SpawnManager    _spawnManager   = null;
    private float           _canFire        = 0f;
    private const float     BoundY  = 6.5f;
    private const float     WrapX   = 13f;

    public int Lives
    {
        get { return _lives; }
        set
        {
            _lives = value;

            if (_lives >= 3)
                _lives = 3;

            if (_lives <= 0)
                _lives = 0;
        }
    }

    private void Awake()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if(_spawnManager == null)
            Debug.Log("SpawnManager is null");

        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if (_audioManager == null)
            Debug.Log("AudioManager is null");
    }

    private void Start()
    {
        transform.position = new Vector3(0, -4, 0);

        if(_shieldPrefab != null)
            _shieldPrefab.SetActive(false);

        _ammoCount = _maxAmmoCount;
    }

    private void Update()
    {
        PlayerMovement();
        VerticalScreenBounds();
        HorizontalScreenWrap();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            Shoot();
        }
    }

    private void Shoot()
    {
        if (_ammoCount == 0)
        {
            _ammoEmpty = true;
            _ammoCount = 0;
            _audioManager.PlayAmmoEmptySound();

            return;
        }
        else
        {
            _ammoEmpty = false;
        }

        _ammoCount -= 1;

        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive)
        {
            Instantiate(_triplShotPrefab, transform.position, Quaternion.identity);

            _audioManager.PlayTripleShotSound();
        }
        else
        {
            Vector3 offsetY = new Vector3(0, 1f, 0);
            Instantiate(_laserPrefab, transform.position + offsetY, Quaternion.identity);

            _audioManager.PlayLaserSound();
        }
    }

    private void HorizontalScreenWrap()
    {
        if (transform.position.x >= WrapX)
        {
            transform.position = new Vector3(-WrapX, transform.position.y, 0);
        }
        else if (transform.position.x <= -WrapX)
        {
            transform.position = new Vector3(WrapX, transform.position.y, 0);
        }
    }

    private void VerticalScreenBounds()
    {
        var clampYpos = Mathf.Clamp(transform.position.y, -BoundY, BoundY);

        transform.position = new Vector3(transform.position.x, clampYpos, 0);
    }

    private void PlayerMovement()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(xInput, yInput, 0);

        transform.Translate(movement.normalized * _speed * Time.deltaTime);
    }

    public void Damage(int damageAmount)
    {
        if (_isShieldActive)
        {
            _isShieldActive = false;
            _shieldPrefab.SetActive(false);
            return;
        }

        _lives -= damageAmount;

        if (_lives == 2)
            _enginePrefabs[0].SetActive(true);
        else if (_lives == 1)
            _enginePrefabs[1].SetActive(true);

        if (_lives <= 0)
        {
            _isDead = true;
            _spawnManager.StopSpawning();
            _lives = 0;

            Instantiate(_ExplosionPrefab, transform.position, Quaternion.identity);

            _audioManager.PlayExplosionSound();

            Destroy(gameObject);
        }
    }

    public void AmmoActive()
    {
        _ammoCount = _maxAmmoCount;
    }

    public void HealthActive()
    {
        Lives += 1;
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;

        _ammoCount = _maxAmmoCount;

        StartCoroutine(TripleShotPowerDownRoutine());
    }

    private IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(_powerDownTime);

        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _speed += _speedBoostAmount;

        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    private IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(_powerDownTime);

        _speed -= _speedBoostAmount;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldPrefab.SetActive(true);
    }

    public void AddScore(int amount)
    {
        _score += amount;
    }

    public void SetScore(int amount)
    {
        _score = amount;
    }

    public int GetScore()
    {
        return _score;
    }

    public int GetLives()
    {
        return _lives;
    }

    public int GetAmmo()
    {
        return _ammoCount;
    }

    public int GetMaxAmmo()
    {
        return _maxAmmoCount;
    }

    public void AddAmmo(int amount)
    {
        _ammoCount += amount;
    }

    public bool IsAmmoEmpty()
    {
        return _ammoEmpty;
    }

    public bool IsDead()
    {
        return _isDead;
    }
}
