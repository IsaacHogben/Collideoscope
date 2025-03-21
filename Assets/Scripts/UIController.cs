using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public GameManager gameManager;
    // Menu
    public VisualElement mainMenuScene;
    public Button playButton;
    public Button freedrawButton;
    // Game UI
    public VisualElement playScene;
    public Label scoreLabel;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>().GetComponent<GameManager>();

        // Get root UI componant
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Initialize elements in menu
        mainMenuScene = root.Q<VisualElement>("mainMenuScene");
        playButton = root.Q<Button>("playButton");
        playButton.clicked += PlayButtonPressed;
        freedrawButton = root.Q<Button>("freedrawButton");
        freedrawButton.clicked += FreedrawButtonPressed;

        // Initialize elements in game
        playScene = root.Q<VisualElement>("playScene");
        playScene.style.display = DisplayStyle.None; // Hide on start while not using
        scoreLabel = root.Q<Label>("scoreLabel");
    }

    void PlayButtonPressed()
    {
        mainMenuScene.style.display = DisplayStyle.None; // Hide Menu
        playScene.style.display = DisplayStyle.Flex; // Show Game UI
        gameManager.StartGame();
        UpdateScore(0);
    }

    public void ReturnToMenu()
    {
        mainMenuScene.style.display = DisplayStyle.Flex; // Hide Menu
    }

    void FreedrawButtonPressed()
    {
        //Hide Menu
        //Enable freedraw controls
        //Display Freedraw Controls
    }

    public void UpdateScore(int newValue)
    {
        scoreLabel.text = newValue.ToString();
    }
}