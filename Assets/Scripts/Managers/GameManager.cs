using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Start, Playing, Paused, Victory, GameOver }


public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState State { get; private set; }
    public int Score { get; private set; }


    public event Action<GameState> OnStateChanged;
    public event Action<int> OnScoreChanged;

    private PlayerHealth playerHealth;
    private GUIStyle hudStyle;
    private GUIStyle bigStyle;
    private GUIStyle smallStyle;

    private void Awake()
    {
       
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Time.timeScale = 1f;
    }

    private void Start()
    {
        playerHealth = FindFirstObjectByType<PlayerHealth>();
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += HandleHealthChanged;
            playerHealth.OnDied += HandlePlayerDied;
        }
        else
        {
            Debug.LogWarning("GameManager: no se encontró PlayerHealth en la escena.");
        }

        Score = 0;
        SetState(GameState.Start);
    }

    private void OnDestroy()
    {
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= HandleHealthChanged;
            playerHealth.OnDied -= HandlePlayerDied;
        }
        if (Instance == this) Instance = null;
    }

    private void Update()
    {
        switch (State)
        {
            case GameState.Start:
                if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                    SetState(GameState.Playing);
                break;

            case GameState.Playing:
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
                    SetState(GameState.Paused);
                break;

            case GameState.Paused:
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.P))
                    SetState(GameState.Playing);
                break;

            case GameState.Victory:
            case GameState.GameOver:
                if (Input.GetKeyDown(KeyCode.R))
                    Restart();
                break;
        }
    }

    public void AddScore(int points)
    {
        if (State != GameState.Playing) return;

        Score += points;
        Debug.Log("Puntaje: " + Score);
        OnScoreChanged?.Invoke(Score);
    }

    public void Win()
    {
        if (State != GameState.Playing) return;
        SetState(GameState.Victory);
    }

    public void Restart()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void SetState(GameState newState)
    {
        State = newState;


        Time.timeScale = (newState == GameState.Playing) ? 1f : 0f;

        Debug.Log("Estado del juego: " + newState
            + " | Vida: " + (playerHealth != null ? playerHealth.CurrentHealth.ToString() : "-")
            + " | Puntaje: " + Score);

        OnStateChanged?.Invoke(newState);
    }


    private void HandleHealthChanged(int currentHealth)
    {
        Debug.Log("Vida del jugador: " + currentHealth);
    }

    private void HandlePlayerDied()
    {
        if (State == GameState.Playing)
            SetState(GameState.GameOver);
    }

    private void OnGUI()
    {
        if (hudStyle == null)
        {
            hudStyle = new GUIStyle(GUI.skin.label);
            hudStyle.fontSize = 22;
            hudStyle.fontStyle = FontStyle.Bold;
            hudStyle.normal.textColor = Color.white;

            bigStyle = new GUIStyle(GUI.skin.label);
            bigStyle.fontSize = 48;
            bigStyle.fontStyle = FontStyle.Bold;
            bigStyle.alignment = TextAnchor.MiddleCenter;
            bigStyle.normal.textColor = Color.white;

            smallStyle = new GUIStyle(GUI.skin.label);
            smallStyle.fontSize = 24;
            smallStyle.alignment = TextAnchor.MiddleCenter;
            smallStyle.normal.textColor = Color.white;
        }

        int hp = playerHealth != null ? playerHealth.CurrentHealth : 0;
        int maxHp = playerHealth != null ? playerHealth.MaxHealth : 0;
        GUI.Label(new Rect(20, 15, 400, 30), "Vida: " + hp + "/" + maxHp, hudStyle);
        GUI.Label(new Rect(220, 15, 400, 30), "Puntaje: " + Score, hudStyle);

        float w = Screen.width;
        float h = Screen.height;

        switch (State)
        {
            case GameState.Start:
                bigStyle.normal.textColor = Color.white;
                GUI.Label(new Rect(0, h * 0.35f, w, 80), "REALITY", bigStyle);
                GUI.Label(new Rect(0, h * 0.35f + 80, w, 40), "Este no es tu mundo. Juntá los fragmentos y encontrá el portal.", smallStyle);
                GUI.Label(new Rect(0, h * 0.35f + 120, w, 40), "Enter para comenzar", smallStyle);
                break;

            case GameState.Paused:
                bigStyle.normal.textColor = Color.white;
                GUI.Label(new Rect(0, h * 0.4f, w, 80), "PAUSA", bigStyle);
                GUI.Label(new Rect(0, h * 0.4f + 80, w, 40), "Esc o P para continuar", smallStyle);
                break;

            case GameState.Victory:
                bigStyle.normal.textColor = Color.green;
                GUI.Label(new Rect(0, h * 0.4f, w, 80), "VOLVISTE A LA REALIDAD", bigStyle);
                GUI.Label(new Rect(0, h * 0.4f + 80, w, 40), "R para reiniciar", smallStyle);
                break;

            case GameState.GameOver:
                bigStyle.normal.textColor = Color.red;
                GUI.Label(new Rect(0, h * 0.4f, w, 80), "TE ATRAPARON", bigStyle);
                GUI.Label(new Rect(0, h * 0.4f + 80, w, 40), "R para reiniciar", smallStyle);
                break;
        }
    }
}