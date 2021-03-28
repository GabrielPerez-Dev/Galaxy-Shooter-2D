using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed = 8f;

    private void Update()
    {
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        if(transform.position.y >= 8f)
        {
            if (transform.parent)
                Destroy(transform.parent.gameObject);
            else
                Destroy(gameObject);
        }
    }
}
