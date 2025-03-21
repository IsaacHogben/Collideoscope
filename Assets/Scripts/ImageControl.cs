using UnityEngine;

public class ImageControl : MonoBehaviour
{
    public static ImageControl Instance { get; private set; }

    public RenderTexture renderTexture; // Assign in Inspector
    public Material drawingMaterial; // Assign in Inspector
    private Texture2D sharedTexture;

    public float fadeSpeed = 1f;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            sharedTexture = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGBA32, false);
            ClearTexture();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
            ClearTexture();
        if (Input.GetKeyDown(KeyCode.F))
            FadeLines();
    }

    private void FadeLines()
    {
        Color[] pixels = sharedTexture.GetPixels();
        Color backgroundColor = Color.black; // Change to match your background

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.Lerp(pixels[i], backgroundColor, fadeSpeed);
        }

        sharedTexture.SetPixels(pixels);
        sharedTexture.Apply();
    }

    public Texture2D GetSharedTexture() => sharedTexture;
    public void ApplyTexture() => sharedTexture.Apply();

    public void SetPixel(int x, int y, Color color)
    {
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