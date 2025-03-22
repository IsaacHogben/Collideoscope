using UnityEngine;

public class PlayerDrawing : MonoBehaviour
{
    private Texture2D texture;
    private Color drawColor;
    private Vector2 previousPosition;
    private bool firstFrame = true;
    [SerializeField] private float maxInterpolationLength = 0f;

    private void Start()
    {
        texture = ImageControl.Instance.GetSharedTexture();
    }

    private void Update()
    {
        drawColor = ColorManager.Instance.CurrentColor;
        Vector2 currentPosition = transform.position;

        if (!firstFrame)
        {
            DrawInterpolated(previousPosition, currentPosition);
        }
        else
        {
            DrawAtPosition(currentPosition);
            firstFrame = false;
        }

        previousPosition = currentPosition;
    }

    void DrawInterpolated(Vector2 start, Vector2 end)
    {
        Vector2 startTex = WorldToTextureCoords(start);
        Vector2 endTex = WorldToTextureCoords(end);

        float distance = Vector2.Distance(startTex, endTex); // prevent lines appear when player mirros snap to new position
        if (distance > maxInterpolationLength)
            return;

        int steps = Mathf.CeilToInt(Vector2.Distance(startTex, endTex));
        for (int i = 0; i <= steps; i++)
        {
            Vector2 interpolated = Vector2.Lerp(startTex, endTex, (float)i / steps);
            ImageControl.Instance.SetPixel((int)interpolated.x, (int)interpolated.y, drawColor);
        }

        ImageControl.Instance.ApplyTexture();
        ImageControl.Instance.UpdateRenderTexture();
    }

    void DrawAtPosition(Vector2 worldPos)
    {
        Vector2 texturePos = WorldToTextureCoords(worldPos);
        ImageControl.Instance.SetPixel((int)texturePos.x, (int)texturePos.y, drawColor);
        ImageControl.Instance.ApplyTexture();
        ImageControl.Instance.UpdateRenderTexture();
    }

    Vector2 WorldToTextureCoords(Vector2 worldPos)
    {
        float x = Mathf.InverseLerp(-5f, 5f, worldPos.x) * ImageControl.Instance.renderTexture.width;
        float y = Mathf.InverseLerp(-5f, 5f, worldPos.y) * ImageControl.Instance.renderTexture.height;
        return new Vector2(x, y);
    }
}
