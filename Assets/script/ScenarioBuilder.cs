using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;


public class ScenarioBuilder : MonoBehaviour {
    public ScoreManager sm;
    private ScenarioGraph sg;
    private CanvasGroup menu_cg;
    private RectTransform map_pointer;
    private RawImage map_overlay;
    private Text status_label;
    private float map_height;
    public bool error_code = false;

    public void Start() {
        sm = new ScoreManager();
        sg = new ScenarioGraph(this);
        menu_cg = GameObject.Find("rightMenuPanel").GetComponent<CanvasGroup>();
        map_pointer = GameObject.Find("mapPointer").GetComponent<RectTransform>();
        map_overlay = GameObject.Find("mapOverlay").GetComponent<RawImage>();
        status_label = GameObject.Find("status").GetComponent<Text>();
        this.HideMenu();
        if (!error_code)
            sg.Start();
    }

    public void Update() {
        status_label.text = "Очки гравця: " + sm.GetScore()
                            + "\r\nЛокація: " + sm.GetLocation(); // Debug
    }

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
        sm.SaveScore();
        sm.GameMode("main");
    }

    public void ExitApp() {
        sm.SaveScore();
        Application.Quit();
    }

    public void ButtonTrigger(int b_id) {
        HideMenu();
        if (sg.link_href[b_id] != null)
            sg.QuestClick(sg.link_href[b_id], sm.GetLocation());
    }

    public void ButtonTriggerStory() {
        HideMenu();
        sg.QuestClick(sg.story_href, sm.GetLocation());
    }

    public void UpdateMap(string filename) {
        Texture2D imgUpd;
        imgUpd = (Texture2D)Resources.Load(filename, typeof(Texture2D));
        map_overlay.texture = imgUpd;

        float tm_height = 0;
        RectTransform topMenu = GameObject.Find("topMenu").GetComponent<RectTransform>();
        if (topMenu != null)
            tm_height = topMenu.rect.height;
        map_height = Screen.height - tm_height;
    }

    public void UpdateCoord(string coord) {
        float x=0, y=0;
        string[] coords;
        coords = coord.Split(':');
        if(coords.Length != 2) return;
        x = float.Parse(coords[0]) * Screen.width / 100;
        y = (map_pointer.sizeDelta.y / 2) - (float.Parse(coords[1]) * map_height / 100);
        map_pointer.anchoredPosition = new Vector2(x, y);
    }
}
