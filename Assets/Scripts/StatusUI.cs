using TMPro;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
    public TMP_Text livesText;
    public TMP_Text scoreText;
    public TMP_Text levelText;
    public TMP_Text difficultyText;

    public void RefreshStatus(int lives, int score, int currentLevel, int difficulty)
    {
        if (livesText != null)
        {
            livesText.text = lives.ToString();
        }

        if (scoreText != null)
        {
            scoreText.text = score.ToString();
        }

        if (levelText != null)
        {
            levelText.text = "Niv. " + currentLevel.ToString();
        }

        if (difficultyText != null)
        {
            difficultyText.text = "Diff. " + difficulty.ToString();
        }
    }
}