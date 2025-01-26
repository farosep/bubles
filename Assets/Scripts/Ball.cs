using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
    [SerializeField] private float constantSpeed = 10f;
    [SerializeField] private float lifeTime = 3f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isInitialized = false;
    private Wind currentWindZone = null;
    private bool canBeAffectedByWind = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreLayerCollision(gameObject.layer, gameObject.layer);
    }

    public void Initialize(Vector2 direction)
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        
        moveDirection = direction.normalized;
        rb.linearVelocity = moveDirection * constantSpeed;
        isInitialized = true;
        canBeAffectedByWind = false;
        
        StartCoroutine(EnableWindEffect());
        Destroy(gameObject, lifeTime);
    }

    private IEnumerator EnableWindEffect()
    {
        yield return new WaitForSeconds(1f);
        canBeAffectedByWind = true;
        CheckForWindZone();
    }

    private void CheckForWindZone()
    {
        if (!canBeAffectedByWind) return;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        foreach (Collider2D collider in colliders)
        {
            Wind wind = collider.GetComponent<Wind>();
            if (wind != null && wind != currentWindZone)
            {
                currentWindZone = wind;
                rb.linearVelocity = wind.GetWindForce();
                return;
            }
        }
        
        // Если не нашли новую зону ветра
        rb.linearVelocity = Vector2.up * constantSpeed * 0.5f;
    }

    private void FixedUpdate()
    {
        if (!isInitialized) return;

        if (!canBeAffectedByWind)
        {
            rb.linearVelocity = moveDirection * constantSpeed;
            return;
        }

        if (currentWindZone != null)
        {
            rb.linearVelocity = currentWindZone.GetWindForce();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Теперь просто игнорируем вход в новую зону ветра
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Wind wind = other.GetComponent<Wind>();
        if (wind != null && wind == currentWindZone)
        {
            currentWindZone = null;
            CheckForWindZone();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            return;
        }

        if (collision.gameObject.CompareTag("Ground"))
        {
            canBeAffectedByWind = true;
            CheckForWindZone();
            return;
        }

        if (!canBeAffectedByWind)
        {
            Vector2 normal = collision.contacts[0].normal;
            moveDirection = Vector2.Reflect(moveDirection, normal);
            rb.linearVelocity = moveDirection * constantSpeed;
        }
    }
} 