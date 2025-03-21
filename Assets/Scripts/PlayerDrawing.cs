using UnityEngine;

public class PlayerDrawing : MonoBehaviour
{
    private Texture2D texture;
    private Color drawColor;

    private void Start()
    {
        texture = ImageControl.Instance.GetSharedTexture();
    }

    private void Update()
    {
        drawColor = ColorManager.Instance.CurrentColor;
        DrawAtPosition(transform.position);
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
