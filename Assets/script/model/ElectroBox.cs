using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ElectroBox : MonoBehaviour {
    public string selector;
    private ModelBuilder model;
    private IDictionary<string, string> chstate;

    public void Start() {
        chstate = new Dictionary<string, string>();
        model = GameObject.Find("ModelBuilder").GetComponent<ModelBuilder>();
        selector = "spacer"; chstate["ph0"] = selector; chstate["ph1"] = selector; chstate["ph2"] = selector;

        string[] palBoxList = { "pal0Box", "pal1Box", "pal2Box", "pal3Box" };
        foreach(string pb in palBoxList) {
            Outline palBox;
            palBox = GameObject.Find(pb).GetComponent<Outline>();
            palBox.enabled = false;
        }
    }

    public void Update() {}

    private IEnumerator ChartChecker() {
        if(chstate["ph0"] == "electrol" && chstate["ph1"] == "res-47k" && chstate["ph2"] == "diode") {
            yield return new WaitForSecondsRealtime(0.5f);
            model.MsgDisplay("Схема в електричному щитку запрацювала.");
        }
    }

    public void SelectPal(string pal) {
        model.HideMenu();
        selector = pal;
    }

    public void ChartUpdate(string referer) {
        model.HideMenu();
        chstate[referer] = selector;
        string filename = "default/models/electrobox/" + chstate[referer];
        Texture2D imgUpd;
        RawImage ph = GameObject.Find(referer).GetComponent<RawImage>();
        imgUpd = (Texture2D)Resources.Load(filename, typeof(Texture2D));
        ph.texture = imgUpd;
        StartCoroutine(ChartChecker());
    }
}
