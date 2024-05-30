using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 5f; 

    // biến kiểm tra hướng di chuyển
    [SerializeField]
    private bool _isMovingRight = true;

    // tham chiếu đến rigidbody2D
    private Rigidbody2D _rigidbody2D;

    // tham chiếu đến collider2D
    private CapsuleCollider2D _capsuleCollider2D;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }
    private void Move()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        transform.localPosition += new Vector3(horizontalInput, 0, 0)
            * moveSpeed * Time.deltaTime;
        if (horizontalInput > 0)
        {
            _isMovingRight = true;
        }
        else if (horizontalInput < 0)
        {
            _isMovingRight = false;
        }
        // xoay nhân vật
        transform.localScale = _isMovingRight ?
            new Vector2(1.406425f, 1.172423f)
            : new Vector2(-1.406425f, 1.172423f);
    }
}
