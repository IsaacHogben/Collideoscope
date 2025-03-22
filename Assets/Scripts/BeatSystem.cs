using System;
using System.Collections.Generic;
using UnityEngine;

public enum eSpawnPattern
{
    Random,
    Spiral
}
public enum eBeatType
{
    halfTime,
    onBeat,
    offBeat,
    Double,
    Triplet,
    Triple
}
public enum eFadeMode
{ 
    steady,
    onBeat,
    none
}

[System.Serializable]
public class BeatChunk
{
    public string name = "New BeatChunk"; // Name for identification
    public int increaseCollidoscopeAmountBy = 0;
    public eFadeMode fadeMode;
    public int duration = 32; // how many beats this part goes for.
    public eBeatType beat = eBeatType.onBeat;
    public eSpawnPattern pattern = eSpawnPattern.Random;
    public float spawnAngle = 0; // in degrees
    public int numberOfMirrorProjectiles = 0; // number of additional projectiles to spawn, spawn angles evenly divided amoung the 360 degrees

}
[System.Serializable]
public class BeatSection
{
    public List<BeatChunk> chunks = new List<BeatChunk>();
}
public class BeatSystem : MonoBehaviour
{
    GameManager gameManager;
    AudioSource audioSource;
    MirrorControl mirrorControl;
    ImageControl imageControl;

    public int sectionLength; // Length of song part in beats
    public float bpm = 120f;  // Base beats per minute
    private float secondsPerBeat;
    private float globalBeatTimer = 0f;
    public List<BeatSection> coreography;
    public List<BeatSection> coreographyPart2;

    public bool isPlaying = false; // Determines with Update is ticking

    private List<BeatSection> currentChoreography;
    // Track song progress for section reference
    private float currentBeat = 0;
    private float beatsPlayedThisChunk = 0;
    private int currentChunk = 0;
    private int currentSection = 0;
    private float currentTimeMultiplyer = 1;

    // Spawn variables
    private float currentSpiralPosition = 0;


    private void Start()
    {
        gameManager = GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
        imageControl = GetComponent<ImageControl>();
        mirrorControl = FindFirstObjectByType<MirrorControl>();

        currentChoreography = coreography;

        int i = 1;
        foreach (BeatSection section in currentChoreography) // loop over all parts to verify formatting of coreography 
        {
            int totalBeats = 0;
            foreach(BeatChunk chunk in section.chunks)
                totalBeats += chunk.duration;
            if (totalBeats != sectionLength)
                throw new System.Exception("Wrong number of beats in part " + i);
            i++;
        }

        secondsPerBeat = 60f / bpm; // How long one beat takes in seconds
    }

    public void Update()
    {
        if (!isPlaying)
            return;

        // Increment global beat timer
        globalBeatTimer += Time.deltaTime / secondsPerBeat;

        currentBeat += (Time.deltaTime / secondsPerBeat) / currentTimeMultiplyer;
        if (currentBeat >= 1)
        {
            MakeSpawnRequest(currentSection, currentChunk);
            currentBeat = 0;
        }

        // Check if it's time for the next beat based on beatType
        if (globalBeatTimer >= 1)
        {
            globalBeatTimer = 0f; // Reset the beat timer
            beatsPlayedThisChunk++;

            // Move to next chunk if needed
            if (beatsPlayedThisChunk >= currentChoreography[currentSection].chunks[currentChunk].duration)
            {
                beatsPlayedThisChunk = 0; // Reset chunk counter
                currentChunk++; // Set Chunk to next chunk
                ResetChunk();

                // Move to next section if we run out of chunks ends
                if (currentChunk >= currentChoreography[currentSection].chunks.Count)
                {
                    currentChunk = 0;
                    currentSection++;

                    if (currentSection >= currentChoreography.Count)
                    {
                        Invoke("EndLevel", 2f); // Stop if we've reached the end
                        isPlaying = false;
                        return;
                    }
                }
                Invoke("FulfillChunkRequests", 0.5f); // Wait until next bar starts to set new chunk effects
                currentTimeMultiplyer = GetTimeMultiplier(); // set new chunk tempo to current tempo
            }
        }
        
    }

    private void EndLevel()
    {
        gameManager.EndLevel();
        // Reset
        currentBeat = 0;
        beatsPlayedThisChunk = 0;
        currentChunk = 0;
        currentSection = 0;
        currentTimeMultiplyer = 1;
        currentSpiralPosition = 0;
        mirrorControl.IncrementMirrorMode(-4); // return to 0 reflections
    }

    private void FulfillChunkRequests()
    {
        mirrorControl.IncrementMirrorMode(currentChoreography[currentSection].chunks[currentChunk].increaseCollidoscopeAmountBy);
        SetFadeForChunk();
    }

    private void SetFadeForChunk()
    {
        // strobe effect that last for the duration of the chunk and happens twice per second
        eFadeMode fadeMode = currentChoreography[currentSection].chunks[currentChunk].fadeMode;
        if (fadeMode == eFadeMode.onBeat)
            imageControl.StartFadeLines(currentChoreography[currentSection].chunks[currentChunk].duration / 2,
                currentChoreography[currentSection].chunks[currentChunk].duration / GetTimeMultiplier(),
                    0.33f);
        else if (fadeMode == eFadeMode.steady)
            imageControl.StartFadeLines(currentChoreography[currentSection].chunks[currentChunk].duration / 2,
                currentChoreography[currentSection].chunks[currentChunk].duration * 4,
                    0.04f);

    }

    private void ResetChunk()
    {
        currentSpiralPosition = 0;
    }

    private void MakeSpawnRequest(int currentSection, int currentChunk)
    {
        float angleToSpawn = 0;
        eSpawnPattern pattern = currentChoreography[currentSection].chunks[currentChunk].pattern;

        switch (pattern)
        {
            case eSpawnPattern.Random:
                angleToSpawn = UnityEngine.Random.Range(0f, 24f) * 15; // Generate random spawn angle if section requires
                break;
            case eSpawnPattern.Spiral:
                angleToSpawn = currentChoreography[currentSection].chunks[currentChunk].spawnAngle - currentSpiralPosition; // Else use specified spawn angle
                currentSpiralPosition += (360 / currentChoreography[currentSection].chunks[currentChunk].duration * GetTimeMultiplier()) * 3; // increment postion to create spiral
                break;
        }

        // Spawn the original projectile
        gameManager.SpawnProjectile(angleToSpawn);

        // Spawn mirrored projectiles
        int mirrorAmount = currentChoreography[currentSection].chunks[currentChunk].numberOfMirrorProjectiles;
        for (int i = 1; i < mirrorAmount; i++)
        {
            float mirroredAngle = GetMirroredAngle(angleToSpawn, i, mirrorAmount);
            gameManager.SpawnProjectile(mirroredAngle);
        }
    }

    // Adjusts beat frequency without affecting tempo
    private float GetTimeMultiplier()
    {
        eBeatType time = currentChoreography[currentSection].chunks[currentChunk].beat;

        switch (time)
        {
            case eBeatType.halfTime:
                return 2f;  // Every 2 beats (slower)
            case eBeatType.Double:
                return 0.5f; // Twice as fast
            case eBeatType.Triple:
                return 1f / 3f; // Thrice as fast
            case eBeatType.Triplet:
                return 2f / 3f; // Triplet timing
            case eBeatType.offBeat:
                return 1.5f; // Between normal and double time
            default:
                return 1f; // Normal beat timing
        }
    }

    public void Play()
    {
        InitializeChunk(currentChunk, currentSection);
        FulfillChunkRequests();
        isPlaying = true;
        audioSource.Play();
    }

    private void InitializeChunk(int currentChunk, int currentSection)
    {
        currentTimeMultiplyer = GetTimeMultiplier();
    }

    public void Pause()
    {
        isPlaying = false;
        audioSource.Pause();
    }

    float GetMirroredAngle(float originalAngle, int index, int mirrorMode)
    {
        float mirroredAngle = originalAngle;

        if (mirrorMode == 2) // Formerly mode 1
        {
            if (index == 1) mirroredAngle = -originalAngle; // Flip across the vertical axis
        }
        else if (mirrorMode == 4)
        {
            if (index == 1) mirroredAngle = 180 - originalAngle;
            if (index == 2) mirroredAngle = -originalAngle;
            if (index == 3) mirroredAngle = 180 + originalAngle;
        }
        else if (mirrorMode == 8)
        {
            int section = index % 4;
            mirroredAngle = originalAngle + (section * 90);
            if ((index & 1) != 0) mirroredAngle = -mirroredAngle;
        }
        else if (mirrorMode == 16)
        {
            float angleStep = 360f / 16; // 22.5-degree segments
            mirroredAngle = originalAngle + (index * angleStep);
            if (index % 2 == 1) mirroredAngle = -mirroredAngle; // Flip every other segment
        }

        return mirroredAngle % 360; // Keep it within 0-360 range
    }
}
