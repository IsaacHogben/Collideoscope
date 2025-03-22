using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    private GameManager gameManager;
    public GameObject destroyPrefab; // Incomming to destroy
    public GameObject avoidPrefab; // Incoming to avoid
    public GameObject scorePrefab;
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
        newProjectile.Initialize(speed, spawnRadius, radian, gameManager, type, scorePrefab);
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
    private GameObject scorePrefab;
    public void Initialize(float speed, float radius, float radian, GameManager _gameManager, ProjectileType type, GameObject scoreObj)
    {
        moveSpeed = speed;
        startRadius = radius;
        gameManager = _gameManager;
        thisType = type;
        scorePrefab = scoreObj;
        transform.position = center + new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0) * radius;
        float randAngle = UnityEngine.Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, 0, randAngle * Mathf.Deg2Rad);
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
            
            int displayValue = gameManager.UpdateScore(thisType, -0.5f);
            ShowScoreText(displayValue);
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        float normalizedDistance = Mathf.Clamp01(Vector3.Distance(center, transform.position) / startRadius); // Get the distance travelled to calculate score.
        int displayValue = gameManager.UpdateScore(thisType, normalizedDistance);
        ShowScoreText(displayValue);
        Destroy(gameObject);
    }

    void ShowScoreText(int score)
    {
        GameObject scoreText = Instantiate(scorePrefab, GameObject.Find("Canvas").transform);

        // Convert world position to UI position
        Vector2 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
        screenPosition.y += 10; // Place above cursor

        // Set anchoredPosition for UI alignment
        RectTransform rect = scoreText.GetComponent<RectTransform>();
        rect.position = screenPosition;
        scoreText.GetComponent<FloatingScore>().SetScore(score);
    }
}