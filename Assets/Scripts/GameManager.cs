using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    private ProjectileSpawner projectileSpawner;
    private UIController uIController;
    private BeatSystem beatSystem;
    public AudioSource SFXSource;
    public AudioClip menuClick;
    public AudioClip plusScore;
    public AudioClip minusScore;

    public float spawnInterval = 1f; // Time between spawns

    private int score; // Score accumulated over time
    public int maxScore = 10; // Score given if projectile has recently spawned, this values reduces as the projectile gets closer to the center

    private void Start()
    {
        beatSystem = gameObject.GetComponent<BeatSystem>();
        uIController = FindFirstObjectByType<UIDocument>().GetComponent<UIController>();
        projectileSpawner = GetComponent<ProjectileSpawner>();
        projectileSpawner.SetInstance(this);
        //InvokeRepeating(nameof(SpawnRandomWall), 0f, spawnInterval);
    }

    public void StartGame()
    {
        beatSystem.Play();
    }

    public void SpawnProjectile(float spawnAngle)
    {
        //float randomSpawn = Random.Range(0f, 6f);
        //if (randomSpawn > 1)
            projectileSpawner.SpawnProjectile(ProjectileType.Destroy, 2f, spawnAngle);
        //else
            //projectileSpawner.SpawnProjectile(ProjectileType.Avoid, 1f, spawnAngle);
    }

    public int UpdateScore(ProjectileType type, float scoreUpdate)
    {
        int scoreToAdd = Mathf.CeilToInt(scoreUpdate * maxScore); // normalise score to present values
        if (type == ProjectileType.Avoid) // lose points for hitting avoid object
            scoreToAdd *= -2; // negative multiplayer for hitting wrong thing
        score += scoreToAdd;
        uIController.UpdateScore(score);

        if (scoreUpdate > 0) // Play SFX for score update
            SFXSource.PlayOneShot(plusScore);
        else
            SFXSource.PlayOneShot(minusScore);
        return scoreToAdd;
    }

    public void MenuClickSound()
    {
        SFXSource.PlayOneShot(menuClick);
    }
}
