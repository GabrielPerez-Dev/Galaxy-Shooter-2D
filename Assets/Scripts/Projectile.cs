using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;

    private void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);
    }
}
