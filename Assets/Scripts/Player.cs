using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;

    private void Start()
    {
        transform.position = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        PlayerMovement();
        VerticalScreenBounds();
        HorizontalScreenWrap();
    }

    private void HorizontalScreenWrap()
    {
        if (transform.position.x >= 13f)
        {
            transform.position = new Vector3(-13f, transform.position.y, 0);
        }
        else if (transform.position.x <= -13f)
        {
            transform.position = new Vector3(13f, transform.position.y, 0);
        }
    }

    private void VerticalScreenBounds()
    {
        if (transform.position.y >= 6.5f)
        {
            transform.position = new Vector3(transform.position.x, 6.5f, 0);
        }
        else if (transform.position.y <= -6.5)
        {
            transform.position = new Vector3(transform.position.x, -6.5f, 0);
        }
    }

    private void PlayerMovement()
    {
        float xInput = Input.GetAxisRaw("Horizontal");
        float yInput = Input.GetAxisRaw("Vertical");

        Vector3 movement = new Vector3(xInput, yInput, 0);

        transform.Translate(movement.normalized * _speed * Time.deltaTime);
    }
}
