using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainScript : MonoBehaviour
{
    public int historicMaxScore;
    public int actualMaxScore;
    private ResultGame ResultGame;
    [SerializeField] TextMeshProUGUI historicScoreText;
    public bool endGame;
    private int backMenu = 0;
    // Start is called before the first frame update
    void Start()
    {
        backMenu = PlayerPrefs.GetInt("BackToMenu");
        Debug.Log(backMenu);
        RefreshScore();
        //SceneManager.LoadScene("MainScene");
    }


    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void RefreshScore()
    {
        historicMaxScore = PlayerPrefs.GetInt("MaxScore");
        Debug.Log(historicMaxScore);
        historicScoreText.text = historicMaxScore.ToString();
    }
    public void Back(bool flag)
    {
        endGame = flag;
    }

    public void QuitApplication()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
    
