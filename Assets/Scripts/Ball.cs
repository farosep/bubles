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
    private bool isWindEffectEnabled = false;

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
        isWindEffectEnabled = false;
        
        StartCoroutine(EnableWindEffect());
        Destroy(gameObject, lifeTime);
    }

    private IEnumerator EnableWindEffect()
    {
        yield return new WaitForSeconds(1f);
        isWindEffectEnabled = true;
    }

    private void FixedUpdate()
    {
        if (!isInitialized) return;

        Vector2 finalVelocity = Vector2.zero;

        if (canBeAffectedByWind && isWindEffectEnabled && currentWindZone != null)
        {
            finalVelocity = currentWindZone.GetWindForce();
        }
        else if (!canBeAffectedByWind)
        {
            finalVelocity = moveDirection * constantSpeed;
        }
        else if (canBeAffectedByWind && rb.linearVelocity.magnitude < 0.1f)
        {
            // Если шарик почти остановился, даём ему небольшой импульс вверх
            finalVelocity = Vector2.up * constantSpeed * 0.5f;
        }

        if (finalVelocity != Vector2.zero)
        {
            rb.linearVelocity = finalVelocity;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Wind wind = other.GetComponent<Wind>();
        if (wind != null && canBeAffectedByWind && isWindEffectEnabled)
        {
            currentWindZone = wind;
            rb.linearVelocity = wind.GetWindForce();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<Wind>() == currentWindZone)
        {
            currentWindZone = null;
            if (canBeAffectedByWind)
            {
                rb.linearVelocity = Vector2.up * constantSpeed * 0.5f;
            }
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
            moveDirection = Vector2.zero;
            
            if (isWindEffectEnabled && currentWindZone != null)
            {
                rb.linearVelocity = currentWindZone.GetWindForce();
            }
            else
            {
                rb.linearVelocity = Vector2.up * constantSpeed * 0.5f;
            }
            return;
        }

        Vector2 normal = collision.contacts[0].normal;
        moveDirection = Vector2.Reflect(moveDirection, normal);
        rb.linearVelocity = moveDirection * constantSpeed;
    }
} 