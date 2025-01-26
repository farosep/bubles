using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Движение")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 12f;
    
    [Header("Стрельба")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private Vector2 moveDirection;
    private PlayerInput playerInput;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();
    }
    
    private void Start()
    {
        if (playerInput == null)
        {
            playerInput = gameObject.AddComponent<PlayerInput>();
        }
        
        playerInput.fireAction.performed += OnFire;
        playerInput.moveAction.performed += OnMove;
        playerInput.moveAction.canceled += OnMove;
    }
    
    private void Update()
    {
        // Горизонтальное движение
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocity.y);
        
        // Прыжок
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        
        // Поворот спрайта
        if (moveInput != 0)
        {
            transform.localScale = new Vector3(Mathf.Sign(moveInput), 1, 1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
    
    private void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }
    
    private void OnFire(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            FireBullet();
        }
    }
    
    private void FireBullet()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Ball ballComponent = bullet.GetComponent<Ball>();
        
        if (ballComponent != null)
        {
            Vector2 shootDirection = transform.localScale.x > 0 ? Vector2.right : Vector2.left;
            ballComponent.Initialize(shootDirection);
        }
    }

    private void OnDestroy()
    {
        if (playerInput != null)
        {
            playerInput.fireAction.performed -= OnFire;
            playerInput.moveAction.performed -= OnMove;
            playerInput.moveAction.canceled -= OnMove;
        }
    }
} 