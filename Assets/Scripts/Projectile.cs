using System;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private ProjectileType _type = ProjectileType.None;
    [SerializeField] private bool _isEnemyLaser = false;
    [SerializeField] private float _speed = 8f;

    private GameObject[] _enemies = null;
    private Rigidbody2D _rigidBody = null;
    private float _destroyOnYAxis = 7.5f;
    private float _rotateSpeed = 200f;

    private void Awake()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        if (_rigidBody == null)
            Debug.Log("Rigidbody is null");
    }

    private void Update()
    {
        if(_type == ProjectileType.Laser)
        {
            if (!_isEnemyLaser)
                MoveUp();
            else
                MoveDown();
        }

        if (_type == ProjectileType.Spread)
            DestroySpreadProjectile();

        if(_type == ProjectileType.Homing)
        {
            _enemies = GameObject.FindGameObjectsWithTag("Enemy");

            foreach (var enemy in _enemies)
            {

                var enemyInstance = GetClosestEnemy(_enemies);

                if (enemyInstance != null)
                {
                    Vector3 direction = (Vector2)enemyInstance.transform.position - _rigidBody.position;
                    float rotateAmount = Vector3.Cross(direction.normalized, transform.up).z;
                    _rigidBody.angularVelocity = -rotateAmount * _rotateSpeed;
                }
            }

            _rigidBody.velocity = transform.up * _speed;
        }
    }

    private Transform GetClosestEnemy(GameObject[] enemies)
    {
        GameObject closestTarget = null; 
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position; 
        foreach (var potentialTarget in enemies)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float distanceSqrToTarget = directionToTarget.sqrMagnitude;
            if (distanceSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = distanceSqrToTarget;
                closestTarget = potentialTarget;
            }
        }

        return closestTarget.transform;
    }

    private void MoveUp()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if (transform.position.y >= _destroyOnYAxis)
        {
            if (transform.parent)
                Destroy(transform.parent.gameObject);
            else
                Destroy(gameObject);
        }
    }

    private void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        if (transform.position.y <= -_destroyOnYAxis)
        {
            if (transform.parent)
                Destroy(transform.parent.gameObject);
            else
                Destroy(gameObject);
        }
    }

    private void DestroySpreadProjectile()
    {
        if(transform.position.x > 13f || transform.position.x < -13f || 
            transform.position.y > _destroyOnYAxis || transform.position.y < -_destroyOnYAxis)
        {
            if (transform.parent)
                Destroy(transform.parent.gameObject);
            else
                Destroy(gameObject);
        }
    }

    public void IsEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    public void IsNotEnemyLaser()
    {
        _isEnemyLaser = false;
    }

    public bool GetIsEnemyLaser()
    {
        return _isEnemyLaser;
    }

    public bool SetIsEnemyLaser(bool isEnemyLaser)
    {
        _isEnemyLaser = isEnemyLaser;
        return _isEnemyLaser;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _isEnemyLaser)
        {
            Player player = other.GetComponent<Player>();
            if(player != null)
                player.Damage(1);

            Destroy(gameObject);
        }
    }

    public ProjectileType GetProjectileType()
    {
        return _type;
    }
}
