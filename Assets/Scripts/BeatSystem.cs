using System;
using System.Collections.Generic;
using UnityEngine;

public enum spawnPattern
{
    Random,
    Spiral
}
public enum beatType
{
    halfTime,
    onBeat,
    offBeat,
    Double,
    Triplet,
    Triple
}

[System.Serializable]
public class BeatChunk
{
    public string name = "New BeatChunk"; // Name for identification
    public int increaseCollidoscopeAmountBy = 0;
    public bool intiateFade = false;
    public int duration = 32; // how many beats this part goes for.
    public beatType beat = beatType.onBeat;
    public spawnPattern pattern = spawnPattern.Random;
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

    public int sectionLength; // Length of song part in beats
    public float bpm = 120f;  // Base beats per minute
    private float secondsPerBeat;
    private float globalBeatTimer = 0f;
    public List<BeatSection> coreography;

    public bool isPlaying = false; // Determines with Update is ticking
    
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
        mirrorControl = FindFirstObjectByType<MirrorControl>();

        int i = 1;
        foreach (BeatSection section in coreography) // loop over all parts to verify formatting of coreography 
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
            if (beatsPlayedThisChunk >= coreography[currentSection].chunks[currentChunk].duration)
            {
                beatsPlayedThisChunk = 0; // Reset chunk counter
                currentChunk++; // Set Chunk to next chunk
                ResetChunk();

                // Move to next section if we run out of chunks ends
                if (currentChunk >= coreography[currentSection].chunks.Count)
                {
                    currentChunk = 0;
                    currentSection++;

                    if (currentSection >= coreography.Count)
                    {
                        isPlaying = false; // Stop if we've reached the end
                        return;
                    }
                }
                FulfillChunkRequests(currentSection, currentChunk);
                currentTimeMultiplyer = GetTimeMultiplier(); // set new chunk tempo to current tempo
            }
        }
        
    }

    private void FulfillChunkRequests(int currentSection, int currentChunk)
    {
        Debug.Log(currentSection + " " + currentChunk);
        mirrorControl.IncrementMirrorMode(coreography[currentSection].chunks[currentChunk].increaseCollidoscopeAmountBy);
    }

    private void ResetChunk()
    {
        currentSpiralPosition = 0;
    }

    private void MakeSpawnRequest(int currentSection, int currentChunk)
    {
        float angleToSpawn = 0;
        spawnPattern pattern = coreography[currentSection].chunks[currentChunk].pattern;

        switch (pattern)
        {
            case spawnPattern.Random:
                angleToSpawn = UnityEngine.Random.Range(0f, 24f) * 15; // Generate random spawn angle if section requires
                break;
            case spawnPattern.Spiral:
                angleToSpawn = coreography[currentSection].chunks[currentChunk].spawnAngle - currentSpiralPosition; // Else use specified spawn angle
                currentSpiralPosition += 360 / coreography[currentSection].chunks[currentChunk].duration * GetTimeMultiplier(); // increment postion to create spiral
                break;
        }

        gameManager.SpawnProjectile(angleToSpawn); // Send request to game manager
    }

    // Adjusts beat frequency without affecting tempo
    private float GetTimeMultiplier()
    {
        beatType time = coreography[currentSection].chunks[currentChunk].beat;

        switch (time)
        {
            case beatType.halfTime:
                return 2f;  // Every 2 beats (slower)
            case beatType.Double:
                return 0.5f; // Twice as fast
            case beatType.Triple:
                return 1f / 3f; // Thrice as fast
            case beatType.Triplet:
                return 2f / 3f; // Triplet timing
            case beatType.offBeat:
                return 1.5f; // Between normal and double time
            default:
                return 1f; // Normal beat timing
        }
    }

    public void Play()
    {
        InitializeChunk(currentChunk, currentSection);
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
}
