using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;

public class UIController : MonoBehaviour
{
    public GameManager gameManager;
    // Menu
    public VisualElement introScene;
    public VisualElement mainMenuScene;
    public VisualElement freeDrawScene;

    public Button playButton;
    public Button freedrawButton;
    public Button hardModeButton;
    public Button backButton;
    // Game UI
    public VisualElement playScene;
    public Label scoreLabel;

    private bool onIntroScene = true;
    public Camera introCamera;
    public AudioSource introSound;

    public bool hardModeUnlocked = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>().GetComponent<GameManager>();

        // Get root UI componant
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Initialize elements in menu
        mainMenuScene = root.Q<VisualElement>("mainMenuScene");
        mainMenuScene.style.display = DisplayStyle.None; // Hide on start while not using

        introScene = root.Q<VisualElement>("introScene");
        freeDrawScene = root.Q<VisualElement>("freeDrawScene");
        freeDrawScene.style.display = DisplayStyle.None;

        playButton = root.Q<Button>("playButton");
        playButton.clicked += PlayButtonPressed;
        freedrawButton = root.Q<Button>("freedrawButton");
        freedrawButton.clicked += FreedrawButtonPressed;
        backButton = root.Q<Button>("backButton");
        backButton.clicked += BackButtonPressed;

        hardModeButton = root.Q<Button>("hardModeButton");
        hardModeButton.clicked += HardModeButtonPressed;
        hardModeButton.style.display = DisplayStyle.None;
        // Initialize elements in game
        playScene = root.Q<VisualElement>("playScene");
        playScene.style.display = DisplayStyle.None; // Hide on start while not using
        scoreLabel = root.Q<Label>("scoreLabel");

        // Intro Easter Egg
        Label introLabel = root.Q<Label>("Label");
        string[] messages = { "Press Every Key To Continue"};
        introLabel.text = messages[Random.Range(0, messages.Length)];
    }

    private void Update()
    {
        if (onIntroScene && Input.anyKeyDown)
        {
            mainMenuScene.style.display = DisplayStyle.Flex;
            introCamera.enabled = false;
            onIntroScene = false;
            introScene.style.display = DisplayStyle.None;
            gameManager.UnlockDrawing();
            gameManager.MenuClickSound();
        }
    }
    void PlayButtonPressed()
    {
        gameManager.MenuClickSound();
        MenuToGame();
        gameManager.StartGame(1);
    }

    void MenuToGame()
    {
        introSound.Pause();
        mainMenuScene.style.display = DisplayStyle.None; // Hide Menu
        introScene.style.display = DisplayStyle.None;
        playScene.style.display = DisplayStyle.Flex; // Show Game UI
        UpdateScore(0);
    }

    public void ReturnToMenu() // Happens at the end of a level
    {
        mainMenuScene.style.display = DisplayStyle.Flex;
        gameManager.inFreeDrawMode = false;
        if (hardModeUnlocked)
            hardModeButton.style.display = DisplayStyle.Flex; // show button for new mode upon returning
        introSound.Play();
    }

    void FreedrawButtonPressed()
    {
        gameManager.MenuClickSound();
        //Change Scene
        freeDrawScene.style.display = DisplayStyle.Flex;
        mainMenuScene.style.display = DisplayStyle.None;
        playScene.style.display = DisplayStyle.None;
        //Enable freedraw controls
        gameManager.inFreeDrawMode = true;
        gameManager.UnlockDrawing();
    }

    void BackButtonPressed()
    {
        gameManager.MenuClickSound();
        //Change Scene
        freeDrawScene.style.display = DisplayStyle.None;
        mainMenuScene.style.display = DisplayStyle.Flex;
        playScene.style.display = DisplayStyle.None;
        //Enable freedraw controls
        gameManager.inFreeDrawMode = false;
    }
    void HardModeButtonPressed()
    {
        gameManager.MenuClickSound();
        MenuToGame();
        gameManager.StartGame(2);
    }

    public void UpdateScore(int newValue)
    {
        scoreLabel.text = newValue.ToString();
    }
}