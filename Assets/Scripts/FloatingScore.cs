using TMPro;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class FloatingScore : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float fadeDuration = 1f;
    private TextMeshProUGUI textMesh;
    public TMP_Text text;
    private Color textColor;
    RectTransform rectTransform;
    int score = 0;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform not found on " + gameObject.name);
            return;
        }
        UpdateTextColor(score);
        Destroy(gameObject, fadeDuration); // Remove after duration
    }

    void Update()
    {
        // Move the text upward in UI space
        rectTransform.anchoredPosition += Vector2.up * moveSpeed * Time.deltaTime;

        // Fade out the text
        textColor.a -= Time.deltaTime / fadeDuration;
        textMesh.color = textColor;
    }

    void UpdateTextColor(int score)
    {
        if (score < 0)
        {
            textColor = Color.red; // Negative values are red
        }
        else
        {
            float t = Mathf.Clamp01(score / 100f); // Normalize value between 0 and 1
            textColor = Color.Lerp(Color.green, Color.yellow, t); // Interpolate from green to yellow
        }
        textMesh.color = textColor;
    }

    public void SetScore(int score)
    {
        if (score > 0)
            text.SetText("+" + score);
        else 
            text.SetText(""+score);

        this.score = score;
    }
}
