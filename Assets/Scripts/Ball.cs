using UnityEngine;
using System.Collections;

public class Ball : MonoBehaviour
{
    [SerializeField] private float constantSpeed = 10f;
    [SerializeField] private float lifeTime = 3f;
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private bool isInitialized = false;
    private bool shouldMove = true;
    private Wind currentWindZone = null;
    private bool canBeAffectedByWind = false;
    private Vector2 lastWindVelocity;

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
        rb.velocity = moveDirection * constantSpeed;
        isInitialized = true;
        canBeAffectedByWind = false;
        shouldMove = true;
        
        StartCoroutine(EnableWindEffect());
        Destroy(gameObject, lifeTime);
    }

    private IEnumerator EnableWindEffect()
    {
        yield return new WaitForSeconds(1f);
        canBeAffectedByWind = true;
    }

    private void FixedUpdate()
    {
        if (!isInitialized || !shouldMove) return;

        if (canBeAffectedByWind)
        {
            if (currentWindZone != null)
            {
                rb.velocity = currentWindZone.GetWindForce();
            }
        }
        else
        {
            rb.velocity = moveDirection * constantSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Wind wind = other.GetComponent<Wind>();
        if (wind != null && canBeAffectedByWind)
        {
            currentWindZone = wind;
            rb.velocity = wind.GetWindForce();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Wind>() == currentWindZone)
        {
            lastWindVelocity = currentWindZone.GetWindForce();
            currentWindZone = null;
            rb.velocity = lastWindVelocity;
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
            shouldMove = false;
            rb.velocity = Vector2.zero;
            return;
        }

        Vector2 normal = collision.contacts[0].normal;
        moveDirection = Vector2.Reflect(moveDirection, normal);
        rb.velocity = moveDirection * constantSpeed;
    }
} 