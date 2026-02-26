using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    public TMP_Text scoreText;
    private int score;

    const string SCORE_KEY = "PLAYER_SCORE";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("⚠️ Đã tồn tại ScoreManager khác. Hủy bản này.");
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void Start()
    {
        LoadScore();
        UpdateScoreText();
        Debug.Log("✅ ScoreManager đã khởi động - Score hiện tại: " + score);
    }

    public void AddScore(int amount)
    {
        score += amount;
        Debug.Log($"💰 Điểm tăng +{amount}! Tổng điểm: {score}");
        UpdateScoreText();
        SaveScore(); // Tự động lưu mỗi lần nhặt
    }

    // Static convenience method for other scripts
    public static void AddToScore(int amount)
    {
        if (Instance != null)
            Instance.AddScore(amount);
        else
            Debug.LogError("❌ Không có ScoreManager (Instance = null).");
    }

    public void SaveScore()
    {
        PlayerPrefs.SetInt(SCORE_KEY, score);
        PlayerPrefs.Save();
        Debug.Log("💾 Đã lưu điểm: " + score);
    }

    void LoadScore()
    {
        score = PlayerPrefs.GetInt(SCORE_KEY, 0);
        Debug.Log("📂 Đã load điểm: " + score);
    }

    void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
        else
        {
            Debug.LogError("❌ ScoreText chưa được gán trong Inspector!");
        }
    }

    // Reset điểm (gọi từ button hoặc menu)
    public void ResetScore()
    {
        score = 0;
        UpdateScoreText();
        SaveScore();
        Debug.Log("🔄 Đã reset điểm về 0");
    }
}