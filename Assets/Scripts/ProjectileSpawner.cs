using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

public class ProjectileSpawner : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject destroyPrefab; // Incomming to destroy
    public GameObject avoidPrefab; // Incoming to avoid
    private float spawnRadius = 5f; // Radius of the circle

    public void SpawnProjectile(ProjectileType type, float speed, float spawnAngle)
    {
        // Calculate spawn position
        float radian = (spawnAngle + 90) * Mathf.Deg2Rad;
        Vector2 spawnPos = new Vector2(Mathf.Cos(radian), Mathf.Sin(radian)) * spawnRadius;

        // Instantiate proj
        GameObject proj = null;
        switch (type)
        {
            case ProjectileType.Destroy:
                proj = Instantiate(destroyPrefab, spawnPos, Quaternion.identity);
                break;
            case ProjectileType.Avoid:
                proj = Instantiate(avoidPrefab, spawnPos, Quaternion.identity);
                break;
        }

        // Set proj properties
        Projectile newProjectile = proj.AddComponent<Projectile>();
        newProjectile.Initialize(speed, spawnRadius, radian, gameManager, type);
    }

    public void SetInstance(GameManager _gameManager)
    {
        gameManager = _gameManager;
    }
}

public class Projectile : MonoBehaviour
{
    public float moveSpeed;
    private float startRadius;
    private float angleWidth;
    private Vector3 center = Vector3.zero;
    private GameManager gameManager;
    private ProjectileType thisType;
    public void Initialize(float speed, float radius, float radian, GameManager _gameManager, ProjectileType type)
    {
        moveSpeed = speed;
        startRadius = radius;
        gameManager = _gameManager;
        thisType = type;
        transform.position = center + new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0) * radius;
        transform.rotation = Quaternion.Euler(0, 0, radian);
        //collider = GetComponent<Collider2D>();
        //transform.localScale = new Vector3(angleWidth / 360f * 2f * Mathf.PI * radius, 1, 1);
    }

    void Update()
    {
        float shrinkFactor = moveSpeed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, center, shrinkFactor);
        transform.localScale = new Vector3(transform.localScale.x * (1 - shrinkFactor / startRadius), transform.localScale.y * (1 - shrinkFactor / startRadius), transform.localScale.z);
        if (Vector3.Distance(transform.position, center) < 0.1f)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        float normalizedDistance = Mathf.Clamp01(Vector3.Distance(center, transform.position) / startRadius); // Get the distance travelled to calculate score.
        int displayValue = gameManager.UpdateScore(thisType, normalizedDistance);
        // use display value to spawn anumber showing score if time permits
        Destroy(gameObject);
    }
}