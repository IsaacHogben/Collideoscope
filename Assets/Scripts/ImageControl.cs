using UnityEngine;
using System.Collections;

public class ImageControl : MonoBehaviour
{
    public static ImageControl Instance { get; private set; }

    public RenderTexture renderTexture; // Assign in Inspector
    public Material drawingMaterial; // Assign in Inspector
    private Texture2D sharedTexture;
    private Coroutine fadeRoutine;

    public bool lockImage = true;

    //public float fadeAmount = 0.1f;
    private void Awake()
    {
        //if (Instance == null)
        //{
            Instance = this;
            sharedTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            ClearTexture();
        //}
        //else
        //{
            //Destroy(gameObject);
        //}
        //StartFadeLines(162, 4, 0.1f);
    }

    private void Update()
    {
        
    }

    public void StartFadeLines(float duration, float steps, float fadeAmount)
    {
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeLinesOverTime(duration, steps, fadeAmount));
    }

    private IEnumerator FadeLinesOverTime(float duration, float steps, float fadeAmount)
    {
        float stepTime = duration / steps; // Time per fade step

        for (int step = 0; step < steps; step++)
        {
            Color[] pixels = sharedTexture.GetPixels();
            Color backgroundColor = Color.black; // Adjust to match your background

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.Lerp(pixels[i], backgroundColor, fadeAmount);
            }

            sharedTexture.SetPixels(pixels);
            sharedTexture.Apply();

            yield return new WaitForSeconds(stepTime);
        }
    }
    public void FadeLines()
    {
        Color[] pixels = sharedTexture.GetPixels();
        Color backgroundColor = Color.black; // Change to match your background

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.Lerp(pixels[i], backgroundColor, 0.2f);
        }

        sharedTexture.SetPixels(pixels);
        sharedTexture.Apply();
    }

    public Texture2D GetSharedTexture()
    { 
        return sharedTexture;
    }
    public void ApplyTexture()
    {
        sharedTexture.Apply();
    }

    public void SetPixel(int x, int y, Color color)
    {
        if (!lockImage)
            sharedTexture.SetPixel(x, y, color);
    }

    public void ClearTexture()
    {
        Color[] clearPixels = new Color[sharedTexture.width * sharedTexture.height];
        for (int i = 0; i < clearPixels.Length; i++) clearPixels[i] = Color.clear;
        sharedTexture.SetPixels(clearPixels);
        sharedTexture.Apply();
    }

    public void UpdateRenderTexture()
    {
        Graphics.Blit(sharedTexture, renderTexture, drawingMaterial);
    }
}