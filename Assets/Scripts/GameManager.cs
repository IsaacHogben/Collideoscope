using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;
using System.Collections;

public class GameManager : MonoBehaviour
{
    private ProjectileSpawner projectileSpawner;
    private UIController uIController;
    private BeatSystem beatSystem;
    private ImageControl imageControl; 
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
        imageControl = gameObject.GetComponent<ImageControl>();
        uIController = FindFirstObjectByType<UIDocument>().GetComponent<UIController>();
        projectileSpawner = GetComponent<ProjectileSpawner>();
        projectileSpawner.SetInstance(this);
        //InvokeRepeating(nameof(SpawnRandomWall), 0f, spawnInterval);
    }

    public void StartGame()
    {
        beatSystem.Play();
        imageControl.StartFadeLines(162, 4, 0.1f);
    }

    public void SpawnProjectile(float spawnAngle)
    {
        //float randomSpawn = Random.Range(0f, 6f);
        //if (randomSpawn > 1)
            projectileSpawner.SpawnProjectile(ProjectileType.Destroy, 2f, spawnAngle);
        //else
            //projectileSpawner.SpawnProjectile(ProjectileType.Avoid, 1f, spawnAngle);
    }

    private int maxSimultaneousSounds = 3; // Set your limit
    private int currentPlayingSounds = 0;

    public int UpdateScore(ProjectileType type, float scoreUpdate)
    {
        int scoreToAdd = Mathf.CeilToInt(scoreUpdate * maxScore); // normalise score to present values
        if (type == ProjectileType.Avoid) // lose points for hitting avoid object
            scoreToAdd *= -2; // negative multiplayer for hitting wrong thing
        score += scoreToAdd;
        uIController.UpdateScore(score);


        // Some audio features that should be part of a different controller
        float SFXPitchVariantion = 0.2f;
        SFXSource.pitch = 1 + Random.Range(-SFXPitchVariantion, SFXPitchVariantion);
        // Only play sound if limit is not exceeded
        if (currentPlayingSounds < maxSimultaneousSounds)
        {
            if (scoreUpdate > 0)
                PlayLimitedSound(plusScore);
            else
                PlayLimitedSound(minusScore);
        }

        return scoreToAdd;
    }
    private void PlayLimitedSound(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
        currentPlayingSounds++;

        // Decrease count after the clip duration
        StartCoroutine(ResetSoundCount(clip.length));
    }

    private IEnumerator ResetSoundCount(float delay)
    {
        yield return new WaitForSeconds(delay);
        currentPlayingSounds = Mathf.Max(0, currentPlayingSounds - 1);
    }
    public void MenuClickSound()
    {
        SFXSource.PlayOneShot(menuClick);
    }

    public void EndLevel()
    {
        score = 0;
        uIController.ReturnToMenu();
    }
}
