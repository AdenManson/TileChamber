using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public Slider slider;
    public Text levelText;
    public BombButton multiDestroyButton;
    private int score = 0;
    private float startMaxScore = 50;
    private float maxScore;
    private int level = 1;
    private float difficultyMult = 1.6f; // determines how much maxScore will increase by

    private BoardManager boardManager;

    private void Start()
    {
       // LoadData();

        boardManager = GameObject.Find("Board").GetComponent<BoardManager>();

        levelText.text = "Level: " + level;
        maxScore = startMaxScore + (Mathf.Pow(level, difficultyMult));
    }

    private void Update()
    {
        slider.value = score / maxScore;
        if (score >= maxScore)
        {
            multiDestroyButton.setEnabled(true);
            resetScore();
        }
    }

    public void resetScore()
    {
        score = 0;
        level++;
        maxScore = startMaxScore + (Mathf.Pow(level, difficultyMult));
        levelText.text = "Level: " + level;
    }

    public int getScore() { return score; }
    public void setScore(int score) { this.score = score; }
    public void addScore(int amount) { score += amount; }

    public void disableMultiDestroyButton()
    {
        multiDestroyButton.setEnabled(false);
    }
    public int getLevel() { return level; }

    private void OnApplicationQuit()
    {
       // SaveData();
    }

    private void SaveData() {
        // Using PlayerPrefs as security is not a major issue for this project
        PlayerPrefs.SetInt("CurrentLevel", level);
        PlayerPrefs.SetInt("Score", score);
        PlayerPrefs.SetInt("HasBomb", multiDestroyButton.getEnabled() ? 1 : 0);
        PlayerPrefs.Save();
    }
    private void LoadData() {
        level = PlayerPrefs.GetInt("CurrentLevel");
        score = PlayerPrefs.GetInt("Score");
        int hasBomb = PlayerPrefs.GetInt("HasBomb");
        if (hasBomb == 1)
            multiDestroyButton.setEnabled(true);
    }
}
