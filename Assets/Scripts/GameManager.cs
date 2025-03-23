using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviour
{
    private ProjectileSpawner projectileSpawner;
    private UIController uIController;
    private BeatSystem beatSystem;
    private MirrorControl mirrorControl;
    private ImageControl imageControl; 
    public AudioSource SFXSource;
    public AudioClip menuClick;
    public AudioClip plusScore;
    public AudioClip minusScore;
    public bool inFreeDrawMode = false;

    public float spawnInterval = 1f; // Time between spawns

    private int score; // Score accumulated over time
    public int maxScore = 10; // Score given if projectile has recently spawned, this values reduces as the projectile gets closer to the center

    private void Start()
    {
        beatSystem = gameObject.GetComponent<BeatSystem>();
        mirrorControl = FindFirstObjectByType<MirrorControl>().GetComponent<MirrorControl>();
        imageControl = gameObject.GetComponent<ImageControl>();
        uIController = FindFirstObjectByType<UIDocument>().GetComponent<UIController>();
        projectileSpawner = GetComponent<ProjectileSpawner>();
        projectileSpawner.SetInstance(this);
        //InvokeRepeating(nameof(SpawnRandomWall), 0f, spawnInterval);
        mirrorControl.SetMirrorMode(4); // Make the menu more interesting
    }

    public void StartGame(int level)
    {
        beatSystem.Play(level);
        imageControl.StartFadeLines(162, 4, 0.1f);
        UnlockDrawing();
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
        float SFXPitchVariantion = 0.15f;
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
        StartCoroutine(ResetSoundCount(clip.length / 2));
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
        uIController.hardModeUnlocked = true;
        uIController.ReturnToMenu();
    }
    private void Update()
    {
        if (inFreeDrawMode && Input.anyKeyDown)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) mirrorControl.SetMirrorMode(1); // No Mirror
            if (Input.GetKeyDown(KeyCode.Alpha2)) mirrorControl.SetMirrorMode(2); // One mirrored half
            if (Input.GetKeyDown(KeyCode.Alpha3)) mirrorControl.SetMirrorMode(3); // Four mirrored quadrants
            if (Input.GetKeyDown(KeyCode.Alpha4)) mirrorControl.SetMirrorMode(4); // Eight-way true kaleidoscope
            if (Input.GetKeyDown(KeyCode.Alpha5)) mirrorControl.SetMirrorMode(5); // Double Eight-way true kaleidoscope
            if (Input.GetKeyDown(KeyCode.C)) imageControl.ClearTexture();
            if (Input.GetKeyDown(KeyCode.F)) imageControl.FadeLines();
        }
    }

    public void UnlockDrawing()
    {
        imageControl.lockImage = false;
    }
}
