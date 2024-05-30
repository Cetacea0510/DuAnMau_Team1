using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    // vận tốc chuyển động 
    // public, private, protected, internal, protected internal
    [SerializeField]
    private float moveSpeed = 5f; // 5m/s
                                  // Start is called before the first frame update
                                  // hàm chạy 1 lần duy nhất khi game bắt đầu
                                  // dùng để khởi tạo giá trị

    // biến kiểm tra hướng di chuyển
    [SerializeField]
    private bool _isMovingRight = true;

    // tham chiếu đến rigidbody2D
    private Rigidbody2D _rigidbody2D;
    // giá trị của lực nhảy
    [SerializeField]
    private float _jumpForce = 20f;

    // tham chiếu đến collider2D
    private CapsuleCollider2D _capsuleCollider2D;

    //tham chieu den animator
    private Animator _animator;
    // Start is called before the first frame update
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
        // left, right, a, d
        var horizontalInput = Input.GetAxis("Horizontal");
        // 0: không nhấn, âm: trái, dương: phải
        // điều khiển phải trái
        // x=1.5 ----> x=1.5+1=2.5
        transform.localPosition += new Vector3(horizontalInput, 0, 0)
            * moveSpeed * Time.deltaTime;
        // localPosition: vị trí tương đối so với cha
        // position: vị trí tuyệt đối so với thế giới
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
