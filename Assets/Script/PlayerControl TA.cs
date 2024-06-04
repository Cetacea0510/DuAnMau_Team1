using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControlTA : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    private bool isClimbing;
    [SerializeField] private bool _isMovingRight = true;
    private Rigidbody2D _rigidbody2D;
    [SerializeField] private float _jumpForce = 20f;
    private CapsuleCollider2D _capsuleCollider2D;
    private Animator _animator;
    [SerializeField] private TextMeshProUGUI _scoreText;
    private static int _score = 0;
    private static int _lives = 3;
    [SerializeField] private TextMeshProUGUI _livesText;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 10f;

    void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        _animator = GetComponent<Animator>();
        _scoreText.text = _score.ToString();
        _livesText.text = _lives.ToString();
    }

    void Update()
    {
        Move();
        Jump();
        if (isClimbing)
        {
            Climb();
        }

        if (Input.GetKeyDown(KeyCode.Space) && !_capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Tilemap"))) // Điều kiện để bắn, bạn có thể thay đổi phím bắn theo ý của mình
        {
            Shoot();
        }
    }

    private void Move()
    {
        var horizontalInput = Input.GetAxis("Horizontal");
        transform.localPosition += new Vector3(horizontalInput * moveSpeed * Time.deltaTime, 0, 0);

        if (horizontalInput > 0)
        {
            _isMovingRight = true;
            _animator.SetBool("isRunning", true);
        }
        else if (horizontalInput < 0)
        {
            _isMovingRight = false;
            _animator.SetBool("isRunning", true);
        }
        else
        {
            _animator.SetBool("isRunning", false);
        }
        transform.localScale = _isMovingRight ? new Vector3(1f, 1f, 1f) : new Vector3(-1f, 1f, 1f);
    }

    private void Jump()
    {
        var check = _capsuleCollider2D.IsTouchingLayers(LayerMask.GetMask("Tilemap"));
        _animator.SetBool("isJump", check);  // Update the animator with grounded status

        if (!check)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidbody2D.AddForce(Vector2.up * _jumpForce, ForceMode2D.Impulse);
            _animator.SetTrigger("Jump");  // Trigger the jump animation
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder")) // Kiểm tra xem collision có phải là thang không
        {
            _rigidbody2D.gravityScale = 0; // Ngừng gravity khi tiếp xúc với thang
            isClimbing = true;
            _animator.SetBool("isClimbing", true); // Kích hoạt hoạt ảnh leo thang
        }
        else if (collision.gameObject.CompareTag("Coin"))
        {
            HandleCoinCollision(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleEnemyCollision();
        }
        else if (collision.gameObject.CompareTag("DamageObstacle"))
        {
            HandleDamageObstacleCollision();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder")) // Kiểm tra xem collision có phải là thang không
        {
            _rigidbody2D.gravityScale = 1; // Khôi phục gravity khi rời khỏi thang
            isClimbing = false;
            _animator.SetBool("isClimbing", false); // Ngừng hoạt ảnh leo thang
        }
    }

    private void Climb()
    {
        float verticalInput = Input.GetAxis("Vertical");

        // Di chuyển nhân vật theo trục y dựa trên giá trị verticalInput
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, verticalInput * moveSpeed);

        // Thiết lập animation cho việc leo cầu thang
        if (verticalInput != 0)
        {
            _animator.SetFloat("climbSpeed", Mathf.Abs(verticalInput));
        }
    }

    private void HandleCoinCollision(GameObject coin)
    {
        Destroy(coin);
        _score += coin.GetComponent<Coin>().coinValue;
        _scoreText.text = _score.ToString();
    }

    private void HandleEnemyCollision()
    {
        _lives -= 1;
        if (_lives > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            _livesText.text = _lives.ToString();
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    private void HandleDamageObstacleCollision()
    {
        _lives -= 1;
        if (_lives > 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            _livesText.text = _lives.ToString();
        }
        else
        {
            Time.timeScale = 0;
        }
    }

    private void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (_isMovingRight)
        {
            rb.velocity = transform.right * bulletSpeed;
        }
        else
        {
            rb.velocity = -transform.right * bulletSpeed;
        }
        // Đảm bảo đạn sẽ không va chạm với nhân vật bắn
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
    }
}
