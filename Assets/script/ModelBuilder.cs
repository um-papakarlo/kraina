using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelBuilder : MonoBehaviour {
    public ScoreManager sm;
    private CanvasGroup menu_cg;

    public void Start() {
        sm = new ScoreManager();
        menu_cg = GameObject.Find("rightMenu").GetComponent<CanvasGroup>();

        HideMenu();
    }

    public void MsgProcess() {
        sm.SetLocation(sm.GetNextLoc());
        sm.SetScore(sm.GetScore() + 100);
        sm.SaveScore();
        sm.GameMode("quest");
    }

    public void MsgDisplay(string msg) {
        Text text = GameObject.Find("msgText").GetComponent<Text>();
        CanvasGroup cv = GameObject.Find("msgOverlay").GetComponent<CanvasGroup>();
        RectTransform rt = GameObject.Find("msgOverlay").GetComponent<RectTransform>();

        text.text = text.text + msg;
        rt.SetSiblingIndex(100);
        cv.blocksRaycasts = true;
        cv.alpha = 1;
    }

    public void Update() {}

    public void ToggleMenu() {
        bool status = true;
        if (menu_cg != null) {
            status = menu_cg.alpha != 0;
            menu_cg.alpha = status ? 0 : 1;
            menu_cg.blocksRaycasts = !status;
        }
    }

    public void HideMenu() {
        if (menu_cg != null) {
            menu_cg.alpha = 0;
            menu_cg.blocksRaycasts = false;
        }
    }

    public void MainMenu() {
        sm.GameMode("main");
    }

    public void ExitApp() {
        Application.Quit();
    }
}
