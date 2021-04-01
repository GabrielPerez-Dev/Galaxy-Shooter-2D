using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField] private float _speed = 1f;

    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, _speed * Time.deltaTime);
    }
}
