using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int            _lives              = 3;
    [SerializeField] private int            _score              = 0;

    [SerializeField] private float          _fireRate           = 0.15f;
    [SerializeField] private int            _ammoCount          = 0;
    [SerializeField] private int            _maxAmmoCount       = 15;

    [SerializeField] private float          _speed              = 3.5f;
    [SerializeField] private float          _thrustSpeed        = 7f;
    [SerializeField] private float          _speedBoostAmount   = 5f;
    [SerializeField] private float          _powerDownTime      = 5f;

    [SerializeField] private GameObject     _ExplosionPrefab    = null;
    [SerializeField] private GameObject     _laserPrefab        = null;
    [SerializeField] private GameObject     _triplShotPrefab    = null;
    [SerializeField] private GameObject     _godsWishPrefab     = null;
    [SerializeField] private GameObject     _shieldPrefab       = null;
    [SerializeField] private GameObject[]   _enginePrefabs      = null;

    [SerializeField] private float          _thrustCharge           = 0f;
    [SerializeField] private float          _thrustMaxCharge        = 20f;
    [SerializeField] private float          _thrusterDepleteRate    = 0.05f;
    [SerializeField] private float          _thrusterRegenRate      = 0.05f;

    private bool _isTripleShotActive        = false;
    private bool _isGodsWishActive          = false;
    private bool _isShieldActive            = false;
    private bool _isDead                    = false;
    private bool _isAmmoEmpty               = false;
    private bool _isThrusting               = false;

    private SpriteRenderer  _shieldRenderer = null;

    private AudioManager    _audioManager   = null;
    private SpawnManager    _spawnManager   = null;
    private Vector3         _movement       = Vector3.zero;
    private Color           _tempColor      = Color.clear;

    private float           _thrustDelay    = 0f;
    private float           _canFire        = -1f;
    private float           _canThrust      = -1f;
    private int             _shieldStrength = 0;
    private const float     BoundY          = 6.5f;
    private const float     WrapX           = 13f;

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

    public int Ammo
    {
        get { return _ammoCount; }
        set
        {
            _ammoCount = value;

            if(_ammoCount == _maxAmmoCount)
            {
                _ammoCount = _maxAmmoCount;
            }
        }
    }

    public float ThrusterCharge
    {
        get { return _thrustCharge; }
        set 
        { 
            _thrustCharge = value;

            if (_thrustCharge >= _thrustMaxCharge)
                _thrustCharge = _thrustMaxCharge;

            if (_thrustCharge <= 0)
            {
                _isThrusting = false;
                _thrustCharge = 0;
            }
        }
    }

    public float ThrusterMaxCharge
    {
        get { return _thrustMaxCharge; }
        set { _thrustMaxCharge = value; }
    }

    private void Awake()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if(_spawnManager == null)
            Debug.Log("SpawnManager is null");

        _audioManager = GameObject.Find("Audio_Manager").GetComponent<AudioManager>();
        if (_audioManager == null)
            Debug.Log("AudioManager is null");

        _shieldRenderer = _shieldPrefab.GetComponent<SpriteRenderer>();
        if (_shieldRenderer == null)
            Debug.Log("Shield Sprite Renderer is null");
    }

    private void Start()
    {
        transform.position = new Vector3(0, -4, 0);

        if(_shieldPrefab != null)
            _shieldPrefab.SetActive(false);

        _ammoCount = _maxAmmoCount;
        _thrustCharge = _thrustMaxCharge;
    }

    private void Update()
    {
        PlayerMovement();
        VerticalScreenBounds();
        HorizontalScreenWrap();
        ThrusterRegeneration();
        ShieldStrengthColor();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            Shoot();
        }

        if (Input.GetKey(KeyCode.LeftShift) && Time.time > _canThrust && _thrustCharge > 0)
        {
            _isThrusting = true;
            Thrusting();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            _isThrusting = false;
        }
    }

    private void ThrusterRegeneration()
    {
        if (!_isThrusting && _thrustCharge < _thrustMaxCharge)
        {
            _thrustCharge += _thrusterRegenRate;
        }
    }

    private void Thrusting()
    {
        _canThrust = Time.time + _thrustDelay;
        ThrusterCharge -= _thrusterDepleteRate;
    }

    private void Shoot()
    {
        if (_ammoCount == 0)
        {
            _isAmmoEmpty = true;
            _ammoCount = 0;
            _audioManager.PlayAmmoEmptySound();

            return;
        }
        else
        {
            _isAmmoEmpty = false;
        }

        _ammoCount -= 1;

        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive)
        {
            Instantiate(_triplShotPrefab, transform.position, Quaternion.identity);

            _audioManager.PlayTripleShotSound();
        }
        else if (_isGodsWishActive)
        {
            int numberOfProjectiles = 20;
            float radius = 5f;

            float angleStep = 360f / numberOfProjectiles;
            float angle = 0f;

            for (int i = 0; i <= numberOfProjectiles; i++)
            {
                float projectileDirXposition = transform.position.x + Mathf.Sin((angle * Mathf.PI) / 180) * radius;
                float projectileDirYposition = transform.position.y + Mathf.Cos((angle * Mathf.PI) / 180) * radius;

                Vector3 projectileVector = new Vector3(projectileDirXposition, projectileDirYposition, 0);
                Vector3 projectileMoveDir = (projectileVector - transform.position).normalized * 5f;

                var projectile = Instantiate(_godsWishPrefab, transform.position, Quaternion.identity);
                projectile.GetComponent<Rigidbody2D>().velocity = new Vector3(projectileMoveDir.x, projectileMoveDir.y, 0);

                angle += angleStep;
            }

            _audioManager.PlayGodsWishSound();
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

        _movement = new Vector3(xInput, yInput, 0);

        if (_isThrusting)
        {
            transform.Translate(_movement.normalized * (_speed + _thrustSpeed) * Time.deltaTime);
        }
        else if(!_isThrusting)
        {
            transform.Translate(_movement.normalized * _speed * Time.deltaTime);
        }

    }

    public void Damage(int damageAmount)
    {
        CameraShake camera = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        camera.ActivateCameraShake();

        if (_isShieldActive)
        {
            _shieldStrength--;

            if(_shieldStrength == 0)
            {
                _isShieldActive = false;
                _shieldPrefab.SetActive(false);
            }

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

    public void GodsWishActive()
    {
        _isGodsWishActive = true;

        _ammoCount = _maxAmmoCount;

        StartCoroutine(GodsWishPowerDownRoutine());
    }

    private IEnumerator GodsWishPowerDownRoutine()
    {
        yield return new WaitForSeconds(_powerDownTime);

        _isGodsWishActive = false;
    }

    public bool IsGodsWishActive()
    {
        return _isGodsWishActive;
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

        if (_shieldStrength < 3)
            _shieldStrength++;

        _shieldPrefab.SetActive(true);
    }

    private void ShieldStrengthColor()
    {
        _tempColor = _shieldRenderer.material.color;
        _shieldRenderer.material.color = _tempColor;

        if (_shieldStrength == 1)
        {
            _tempColor.a = 0.3f;
        }
        
        if (_shieldStrength == 2)
        {
            _tempColor.a = 0.75f;
        }
        
        if (_shieldStrength == 3)
        {
            _tempColor.a = 1.1f;
        }

        _shieldRenderer.material.color = _tempColor;
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
        if(_ammoCount < _maxAmmoCount)
            Ammo += amount;
    }

    public bool IsAmmoEmpty()
    {
        return _isAmmoEmpty;
    }

    public bool IsDead()
    {
        return _isDead;
    }
}
