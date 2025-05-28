using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    [System.Serializable]
    public class ScoreEntry
    {
        public string playerName;
        public int score;
    }

    public List<ScoreEntry> scoreEntries = new List<ScoreEntry>();
    public GameObject leaderboardUI;
    public TMP_Text[] nameTexts;
    public TMP_Text[] scoreTexts;

    private const string PlayerPrefsKey = "LeaderboardData";

    private void Awake()
    {
        LoadScores();
    }

    public void AddScore(string name, int score)
    {
        scoreEntries.Add(new ScoreEntry { playerName = name, score = score });

        // Ordena do maior para o menor score
        scoreEntries.Sort((a, b) => b.score.CompareTo(a.score));

        // Mantém apenas os top 5 scores
        if (scoreEntries.Count > 5)
        {
            scoreEntries.RemoveAt(scoreEntries.Count - 1);
        }

        SaveScores();
        UpdateUI();
    }

    private void UpdateUI()
    {
        for (int i = 0; i < Mathf.Min(scoreEntries.Count, nameTexts.Length); i++)
        {
            nameTexts[i].text = scoreEntries[i].playerName;
            scoreTexts[i].text = scoreEntries[i].score.ToString();
        }
    }

    private void SaveScores()
    {
        string json = JsonUtility.ToJson(new Wrapper { scores = scoreEntries });
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
    }

    private void LoadScores()
    {
        if (PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            string json = PlayerPrefs.GetString(PlayerPrefsKey);
            Wrapper wrapper = JsonUtility.FromJson<Wrapper>(json);
            scoreEntries = wrapper.scores;
            UpdateUI();
        }
    }

    [System.Serializable]
    private class Wrapper
    {
        public List<ScoreEntry> scores;
    }
}