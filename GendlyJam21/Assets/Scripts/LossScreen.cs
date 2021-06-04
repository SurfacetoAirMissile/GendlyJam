using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class LossScreen : MonoBehaviour
{
    [SerializeField]
    TMP_Text scoreText = default;

    // Start is called before the first frame update
    void Start()
    {
        UpdateHighscoreString();
    }

    void UpdateHighscoreString()
    {
        string scores = HandleTextFile.ReadString();
        scoreText.text = "Highscores:\n\n" + scores;
    }

    public void GoToGame()
    {
        SceneManager.LoadScene("demo");
    }
}
