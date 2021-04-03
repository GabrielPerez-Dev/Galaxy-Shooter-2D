using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float _rotateSpeed = 1f;
    [SerializeField] private float _speed = 2f;
    [SerializeField] private GameObject _childObject = null;
    [SerializeField] private GameObject _explosionPrefab = null;
    [SerializeField] private Collider2D _collider = null;

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
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            //Disable the Collider so we don't Instantiate the Explosion more than once.
            _collider.enabled = false;

            Destroy(other.gameObject);
            Destroy(gameObject, 0.3f);
        }
    }
}
