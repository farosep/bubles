using UnityEngine;

public class Wind : MonoBehaviour
{
    [SerializeField] private float windForce = 5f;
    [SerializeField] private Vector2 windDirection = Vector2.right;
    [SerializeField] private Vector2 zoneSize = new Vector2(5f, 3f);
    private BoxCollider2D windZone;

    private void Awake()
    {
        windZone = gameObject.AddComponent<BoxCollider2D>();
        windZone.isTrigger = true;
        windZone.size = zoneSize;
    }

    private void OnDrawGizmos()
    {
        // Визуализация зоны ветра
        Gizmos.color = new Color(0, 1, 1, 0.2f);
        Gizmos.DrawCube(transform.position, new Vector3(zoneSize.x, zoneSize.y, 1));
        
        // Визуализация направления
        Gizmos.color = Color.cyan;
        Vector3 position = transform.position;
        Gizmos.DrawLine(position, position + (Vector3)(windDirection.normalized * 2));
    }

    public Vector2 GetWindForce()
    {
        return windDirection.normalized * windForce;
    }

    public bool IsCompletelyInside(Collider2D other)
    {
        Bounds windBounds = windZone.bounds;
        Bounds otherBounds = other.bounds;
        
        return windBounds.Contains(otherBounds.min) && windBounds.Contains(otherBounds.max);
    }
} 