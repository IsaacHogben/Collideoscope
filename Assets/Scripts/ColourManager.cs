using UnityEngine;

public class ColorManager : MonoBehaviour
{
    public static ColorManager Instance { get; private set; }
    public Color CurrentColor { get; private set; } = Color.green;
    public float colorChangeSpeed = 1f;

    private float t = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        CurrentColor = Color.HSVToRGB(Mathf.PingPong(t, 1f), 1f, 1f);
        t += Time.deltaTime * colorChangeSpeed;
    }
}
