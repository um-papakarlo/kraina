using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ScoreManager {
    public static string location = "start";
    public static string next_loc = "";
    public static int score = 0;

    public void SetScore(int newscore) { score = newscore; }
    public int GetScore() { return score; }

    public void SetLocation(string currentLocation) { location = currentLocation; }
    public string GetLocation() { return location; }

    public void SetNextLoc(string nl) { next_loc = nl; }
    public string GetNextLoc() { return next_loc; }

    public void GameMode(string name) { SceneManager.LoadScene(name); }

    public void NewScore()  {
        PlayerPrefs.DeleteAll();
        this.LoadScore();
        this.SaveScore();
    }

    public void LoadScore() {
        score = PlayerPrefs.GetInt("score", 0);
        location = PlayerPrefs.GetString("location", "start");
    }

    public void SaveScore() {
        PlayerPrefs.SetInt("saved", 1);
        PlayerPrefs.SetInt("score", score);
        PlayerPrefs.SetString("location", location);
    }

    public void ApplySettings() {
        PlayerPrefs.DeleteAll();
        Debug.Log("applysettings, all savings are removed");
    }

    public ScoreManager() {
    }
}

public class MenuBuilder : MonoBehaviour {
    private ScoreManager sm;

    public void NewGame()  {
        sm.NewScore();
        sm.GameMode("quest");
    }

    public void LoadGame() {
        sm.LoadScore();
        sm.GameMode("quest");
    }

    public void ApplySettings() {
        sm.ApplySettings();
    }

    public void ExitApp() {
        Application.Quit();
    }

    void Start() {
        sm = new ScoreManager();
        // Перевіряємо, чи можливо завантажити гру
        if (PlayerPrefs.GetInt("saved", 0) == 0) {
            Button loadButton = GameObject.Find("loadButton").GetComponent<Button>();
            if (loadButton != null)
                loadButton.interactable = false;
        }
    }
}
