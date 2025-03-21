using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float radius = 5f; // The radius of the playable circle
    public float speed = 10f; // Movement speed
    public float speedOfRotation = 10f; // Rotation Speed

    private Vector2 targetPosition;

    void Update()
    {
        // Convert mouse position to world position
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Vector3 mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        //mousePos.z = 0f; // Ensure it's in 2D space

        // Clamp position within the circular boundary
        targetPosition = Vector2.ClampMagnitude(mousePos, radius);

        // Move smoothly towards target
        transform.position = Vector2.Lerp(transform.position, targetPosition, speed * Time.deltaTime);

        // Rotate towards target
        Vector2 direction = targetPosition - (Vector2)transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle), speedOfRotation * Time.deltaTime);
    }

    void OnDrawGizmos() //Clean up - Delete
    {
        // Draw the playable circle in the Scene view
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(Vector3.zero, radius);
    }
}
