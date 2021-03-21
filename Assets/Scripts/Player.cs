using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed = 3.5f;
    private const float BoundY = 6.5f;
    private const float WrapX = 13f;

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
}
