using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Lives and Score Properties")]
    [SerializeField] private int            _lives              = 3;
    [SerializeField] private int            _score              = 0;

    [Header("Weapon Properties")]
    [SerializeField] private float          _fireRate           = 0.15f;
    [SerializeField] private int            _ammoCount          = 0;
    [SerializeField] private int            _maxAmmoCount       = 15;
    [SerializeField] private int            _missleCount        = 3;
    [SerializeField] private int            _maxMissleCount     = 3;

    [Header("Speed and Power-up time Properties")]
    [SerializeField] private float          _speed              = 3.5f;
    [SerializeField] private float          _thrustSpeed        = 7f;
    [SerializeField] private float          _speedBoostAmount   = 5f;
    [SerializeField] private float          _speedNegateAmount  = 2f;
    [SerializeField] private float          _powerDownTime      = 5f;

    [Header("Prefab References")]
    [SerializeField] private GameObject     _ExplosionPrefab    = null;
    [SerializeField] private GameObject     _laserPrefab        = null;
    [SerializeField] private GameObject     _triplShotPrefab    = null;
    [SerializeField] private GameObject     _godsWishPrefab     = null;
    [SerializeField] private GameObject     _misslePrefab       = null;
    [SerializeField] private GameObject     _missleUIPrefab     = null;
    [SerializeField] private GameObject     _shieldPrefab       = null;
    [SerializeField] private GameObject[]   _enginePrefabs      = null;

    [Header("Thruster Properties")]
    [SerializeField] private float          _thrustCharge           = 0f;
    [SerializeField] private float          _thrustMaxCharge        = 20f;
    [SerializeField] private float          _thrusterDepleteRate    = 0.05f;
    [SerializeField] private float          _thrusterRegenRate      = 0.05f;

    private bool _isTripleShotActive        = false;
    private bool _isGodsWishActive          = false;
    private bool _isEatThisActive           = false;
    private bool _isShieldActive            = false;
    private bool _isSpeedNegateActive       = false;
    private bool _isDead                    = false;
    private bool _isPickingUpPowerup        = false;
    private bool _isAmmoEmpty               = false;
    private bool _isThrusting               = false;

    private SpriteRenderer  _shieldRenderer = null;
    private AudioManager    _audioManager   = null;
    private SpawnManager    _spawnManager   = null;
    private Flash         _flashDamage    = null;
    private CameraShake     _cameraShake    = null;
    private Vector3         _movement       = Vector3.zero;
    private Color           _tempColor      = Color.clear;

    private float           _thrustDelay    = 0f;
    private float           _canFire        = -1f;
    private float           _canThrust      = -1f;
    private float           _pickUpSpeed    = 8f;

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

    public int MissleCount
    {
        get { return _missleCount; }
        set
        {
            _missleCount = value;

            if(_missleCount >= _maxMissleCount)
            {
                _missleCount = _maxMissleCount;
            }

            if(_missleCount <= 0)
            {
                _isEatThisActive = false;
                _missleUIPrefab.SetActive(false);
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

        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        if (_cameraShake == null)
            Debug.Log("CameraShake is null");

        _flashDamage = GetComponent<Flash>();
        if (_flashDamage == null)
            Debug.Log("FlashDamage is null");
    }

    private void Start()
    {
        transform.position = new Vector3(0, -4, 0);

        if(_shieldPrefab != null)
            _shieldPrefab.SetActive(false);

        _ammoCount = _maxAmmoCount;
        _missleCount = _maxMissleCount;
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
            FireProjectile();
        }

        if (Input.GetKeyDown(KeyCode.Q) && Time.time > _canFire && _isEatThisActive)
        {
            FireMissle();
        }

        if (Input.GetKey(KeyCode.C))
        {
            _isPickingUpPowerup = true;
            PickUpPowerUp();
        } 
        else if (Input.GetKeyUp(KeyCode.C))
        {
            _isPickingUpPowerup = false;
        }

        if (Input.GetKey(KeyCode.LeftShift) && Time.time > _canThrust && _thrustCharge > 0 && !_isSpeedNegateActive)
        {
            _isThrusting = true;
            Thrusting();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift) || _isSpeedNegateActive)
        {
            _isThrusting = false;
        }
    }

    private void PickUpPowerUp()
    {
        var powerups = GameObject.FindGameObjectsWithTag("Powerup");
        if (powerups == null) return;

        if(powerups != null)
        {
            foreach (var powerup in powerups)
            {
                Vector3 direction = powerup.transform.position - transform.position;
                powerup.transform.position -= direction.normalized * _pickUpSpeed * Time.deltaTime;
            }
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

    private void FireMissle()
    {
        _canFire = Time.time + _fireRate;

        if(_missleCount == 0)
        {
            _isEatThisActive = false;
            _missleCount = 0;
            _audioManager.PlayAmmoEmptySound();
            _missleUIPrefab.SetActive(false);

            return;
        }

        _missleCount -= 1;

        var missileInstance = Instantiate(_misslePrefab, transform.position, Quaternion.identity);
        Destroy(missileInstance.gameObject, 10f);
    }

    private void FireProjectile()
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
        _cameraShake.ActivateCameraShake();

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

        _flashDamage.WhenHit();

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

    public void SpeedNegateActive()
    {
        _isSpeedNegateActive = true;
        _speed -= _speedNegateAmount;
        StartCoroutine(SpeedNegatePowerDownRoutine());
    }

    private IEnumerator SpeedNegatePowerDownRoutine()
    {
        yield return new WaitForSeconds(_powerDownTime);

        _speed += _speedNegateAmount;

        _isSpeedNegateActive = false;
    }

    public void AmmoActive()
    {
        _ammoCount = _maxAmmoCount;
    }

    public void HealthActive()
    {
        Lives += 1;
    }

    public void EatThisActive()
    {
        _isEatThisActive = true;

        _missleUIPrefab.SetActive(true);

        _missleCount = _maxMissleCount;
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

    public int GetShields()
    {
        return _shieldStrength;
    }

    public int GetMissleCount()
    {
        return _missleCount;
    }

    public int GetMaxMissile()
    {
        return _maxMissleCount;
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

    public bool IsPickingUpPowerUp()
    {
        return _isPickingUpPowerup;
    }

    public bool IsThrusting()
    {
        return _isThrusting;
    }
}
