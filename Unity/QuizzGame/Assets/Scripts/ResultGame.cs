using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResultGame : MonoBehaviour
{
    private QuestionAdministration QuestionAdministration;
    private MainScript MainScript;
    [SerializeField] TextMeshProUGUI finalScoreText;
    [SerializeField] TextMeshProUGUI historicScoreText;
    private int score;
    private int historicScoreMax;
    public string quizzScene = "QuizzScene";
    public string mainScene = "MainScene";
    private string prefScoreMax = "MaxScore";
    private string prefRestart = "Restart";
    private string prefMenu = "BackToMenu";
    private void Start()
    {
        FinalScore();
        HistoricScore();

    }
    public void FinalScore()
    {
        score = PlayerPrefs.GetInt("Score");
        Debug.Log(score);
        finalScoreText.text = score.ToString();
    }

    public void HistoricScore()
    {
        score = PlayerPrefs.GetInt("Score");
        historicScoreMax = PlayerPrefs.GetInt(prefScoreMax);
        if (score > historicScoreMax)
        {
            PlayerPrefs.SetInt(prefScoreMax, score);
            historicScoreText.text = historicScoreMax.ToString();
        }
        else
        {
            historicScoreText.text = historicScoreMax.ToString();
        }
    }

    public void RestartButton(bool button)
    {
        if (button)
        {
            PlayerPrefs.GetInt(prefRestart, 1);
            SceneManager.LoadScene(quizzScene);
        }
        
    }

    public void BackToMenubutton()
    {
        PlayerPrefs.GetInt(prefMenu, 1);
        SceneManager.LoadScene(mainScene);
    }
}
