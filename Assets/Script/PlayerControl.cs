using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    // Biến kiểm tra xem nhân vật có đang leo thang hay không
    private bool isClimbing;
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

    //tham chiếu đên TMP để hiển thị điểm
    [SerializeField]
    private TextMeshProUGUI _scoreText;
    private static int _score = 0;

    private static int _lives = 3;
    [SerializeField]
    private TextMeshProUGUI _livesText;
    // Start is called before the first frame update
    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _animator = GetComponent<Animator>();
        //hiển thị điểm
        _scoreText.text = _score.ToString();
        _livesText.text = _lives.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Jump();
        if (!isClimbing)
        {
            Climb();
        }
    }
    private void Move()
    {
        //left, right, a,d 
        var horizontalInput = Input.GetAxis("Horizontal");
        transform.localPosition += new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0, 0);

        if (horizontalInput > 0)
        {
            //qua phải 
            _isMovingRight = true;
            _animator.SetBool("isRunning", true);
        }
        else if (horizontalInput < 0)
        {
            //qua trái
            _isMovingRight = false;
            _animator.SetBool("isRunning", true);
        }
        else
        {
            //đứng yên
            _animator.SetBool("isRunning", false);
        }
        transform.localScale = _isMovingRight ? new Vector3(1f, 1f, 1f) : new Vector3(-1f, 1f, 1f);
    }
    private void Jump()
    {
        var check = _capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Tilemap"));
        if (check == false)
        {
            return;
        }
        var verticalInput = Input.GetAxis("Jump");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            _rigidbody2D.gravityScale = 0;
            isClimbing = true;
        }
        else
        {
            isClimbing = false;
        }
        //nếu chạm với xu
        if (collision.gameObject.CompareTag("Coin"))
        {
            //biến mất xu
            Destroy(collision.gameObject);
            //tăng điểm
            _score += collision.gameObject.GetComponent<Coin>().coinValue;
            //hiển thị điểm
            _scoreText.text = _score.ToString();

        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            //nếu va chạm với quái
            _lives -= 1;

            if (_lives > 0)
            {
                //reload game tại màn chơi hiện tại 
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                _livesText.text = _lives.ToString();
            }
            else
            {
                //dừng game
                Time.timeScale = 0;

            }
        }
    }

    // Hàm mới để xử lý việc leo thang
    private void Climb()
    {
        if (!isClimbing)
        {
            _animator.SetBool("isClimbing", false);
            return;
        }

        float verticalInput = Input.GetAxis("Vertical");

        // Di chuyển lên hoặc xuống thang
        transform.Translate(new Vector3(0, verticalInput * moveSpeed * Time.deltaTime, 0));

        // Cập nhật animation
        if (verticalInput != 0)
        {
            _animator.SetBool("isClimbing", true);
            _animator.SetFloat("climbSpeed", verticalInput); // Set speed của animation leo thang
        }
        else
        {
            _animator.SetBool("isClimbing", false);
        }
    }
}