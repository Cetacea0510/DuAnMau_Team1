using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerControl : MonoBehaviour
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
        if (!isClimbing)
        {
            Climb();
        }

        if (Input.GetKeyDown(KeyCode.Space)) // Điều kiện để bắn, bạn có thể thay đổi phím bắn theo ý của mình
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
        if (collision.gameObject.CompareTag("Ladder")) // Kiểm tra xem collision có phải là thang không
        {
            _rigidbody2D.gravityScale = 0; // Ngừng gravity khi tiếp xúc với thang
            isClimbing = true;
        }
        else
        {
            isClimbing = false;
        }

        if (collision.gameObject.CompareTag("Coin"))
        {
            HandleCoinCollision(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleEnemyCollision();
        }
        else
        {
            // Xử lý va chạm với mặt đất hoặc các đối tượng khác ở đây
            _rigidbody2D.gravityScale = 1f;
        }

        // Xử lý va chạm với chướng ngại vật mất máu ở đây
        if (collision.gameObject.CompareTag("DamageObstacle"))
        {
            HandleDamageObstacleCollision();
        }
    }

    private void Climb()
    {
        if (!isClimbing)
        {
            _animator.SetBool("isClimbing", false);
            return;
        }

        float verticalInput = Input.GetAxis("Vertical");

        // Di chuyển nhân vật theo trục y dựa trên giá trị verticalInput
        transform.Translate(new Vector3(0, verticalInput * moveSpeed * Time.deltaTime, 0));

        // Thiết lập animation cho việc leo cầu thang
        if (verticalInput != 0)
        {
            _animator.SetBool("isClimbing", true);
            _animator.SetFloat("climbSpeed", Mathf.Abs(verticalInput));
        }
        else
        {
            _animator.SetBool("isClimbing", false);
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
