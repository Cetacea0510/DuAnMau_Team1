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
        if (collision.gameObject.CompareTag("Ladder"))
        {
            _rigidbody2D.gravityScale = 0;
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
        transform.Translate(new Vector3(0, verticalInput * moveSpeed * Time.deltaTime, 0));

        if (verticalInput != 0)
        {
            _animator.SetBool("isClimbing", true);
            _animator.SetFloat("climbSpeed", verticalInput);
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
}
