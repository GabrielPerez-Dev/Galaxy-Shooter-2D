using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private int _lives = 3;
    [SerializeField] private float _speed = 3.5f;
    [SerializeField] private float _speedBoostAmount = 5f;
    [SerializeField] private float _fireRate = 0.15f;
    [SerializeField] private float powerDownTime = 5f;
    [SerializeField] private GameObject _laserPrefab = null;
    [SerializeField] private GameObject _triplShotPrefab = null;
    [SerializeField] private GameObject _shieldPrefab = null;

    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;

    private SpawnManager _spawnManager = null;
    private float _canFire = 0f;

    private const float BoundY = 6.5f;
    private const float WrapX = 13f;

    private void Awake()
    {
        _spawnManager = GameObject.Find("[Spawn_Manager]").GetComponent<SpawnManager>();

        if(_spawnManager == null)
            Debug.Log("SpawnManager is null");
    }

    private void Start()
    {
        transform.position = new Vector3(0, -4, 0);

        if(_shieldPrefab != null)
            _shieldPrefab.SetActive(false);
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

        Debug.Log(_canFire);
    }

    private void Shoot()
    {
        _canFire = Time.time + _fireRate;

        Vector3 offsetY = new Vector3(0, 1f, 0);

        if (_isTripleShotActive)
        {
            Instantiate(_triplShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(_laserPrefab, transform.position + offsetY, Quaternion.identity);
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

        if (_lives <= 0)
        {
            _spawnManager.StopSpawning();
            _lives = 0;
            Destroy(gameObject);
        }
    }

    public void TripleShotActive()
    {
        _isTripleShotActive = true;

        StartCoroutine(TripleShotPowerDownRoutine());
    }

    private IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(powerDownTime);

        _isTripleShotActive = false;
    }

    public void SpeedBoostActive()
    {
        _isSpeedBoostActive = true;

        _speed += _speedBoostAmount;

        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    private IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(powerDownTime);

        _speed -= _speedBoostAmount;

        _isSpeedBoostActive = false;
    }

    public void ShieldActive()
    {
        _isShieldActive = true;
        _shieldPrefab.SetActive(true);
    }
}
