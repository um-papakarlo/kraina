using System.IO;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScenarioGraph {
    private Text text;
    private RawImage image;
    private ScenarioBuilder parent;
    private XmlDocument scenario;
    private XmlNodeList scenarioTag;
    private XmlNodeList dataList;
    public TextAsset xmlfile;
    private string startLocation;
    private string xmlname = "scenario";
    public string[] link_descr;
    public string[] link_href;
    public string story_href;

    public void Start() {
        string loc = parent.sm.GetLocation();
        parent.UpdateMap(scenarioTag.Item(0).Attributes.GetNamedItem("map").InnerText);
        if(loc == "start") loc = startLocation;
        QuestLoad(loc);
    }

    public ScenarioGraph(ScenarioBuilder sb) {
        parent = sb;
        link_href = new string[8];
        link_descr = new string[8];
        text = GameObject.Find("questTask").GetComponent<Text>();
        image = GameObject.Find("rightImage").GetComponent<RawImage>();

        try {
            xmlfile = (TextAsset)Resources.Load(xmlname, typeof(TextAsset));
            scenario = new XmlDocument();
            scenario.LoadXml(xmlfile.text);
        }
        catch {
            CatchNotify("Failed to load scenario file"); return;
        }

        try {
            float engineVersion;
            scenarioTag = scenario.GetElementsByTagName("scenario");
            engineVersion = float.Parse(scenarioTag.Item(0).Attributes.GetNamedItem("kraina_version").InnerText, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            if (engineVersion > 1) {
                CatchNotify("Vrong scenario engine version (kraina_version: " + engineVersion + ")"); return;
            }
            startLocation = scenarioTag.Item(0).Attributes.GetNamedItem("start_location").InnerText;
            dataList = scenario.GetElementsByTagName("item");
        }
        catch {
            CatchNotify("Failed to parsre scenario file"); return;
        }
    }

    private void QuestLoad(string href) {
        int link_id = 0;
        XmlNode quest_step = null;
        
        foreach (XmlNode item in dataList) { // знайти тег item з потрібним name
            if(item.Attributes.GetNamedItem("name").InnerText == href)
                quest_step = item;
        }
        
        if (quest_step == null) { // спроба повернутись на початок у випадки помилки сценарію
            Debug.LogError("Mistake in the ridding scennario. Reqwested item: " + href + ". Going to start location...");
            foreach (XmlNode item in dataList) {
                if (item.Attributes.GetNamedItem("name").InnerText == startLocation)
                    quest_step = item;
            }
        }

        if (quest_step.Attributes.GetNamedItem("type") != null) {
            switch (quest_step.Attributes.GetNamedItem("type").InnerText) {
                case "story":
                    UpdateStoryImage(quest_step);
                    return;
                case "model":
                    foreach (XmlNode subtag in quest_step.ChildNodes) {
                        switch (subtag.Name) {
                            case "model":
                                if (subtag.Attributes.GetNamedItem("href") != null)
                                    parent.sm.SetNextLoc(subtag.Attributes.GetNamedItem("href").InnerText);
                                if (subtag.Attributes.GetNamedItem("name") != null)
                                    parent.sm.GameMode(subtag.Attributes.GetNamedItem("name").InnerText);
                                break;
                        }
                    }
                    return;
            }
        }

        parent.sm.SetLocation(href);
        foreach (XmlNode subtag in quest_step.ChildNodes) {
            switch (subtag.Name) {
                case "text":
                    text.text = subtag.InnerText;
                    break;
                case "coord":
                    parent.UpdateCoord(subtag.InnerText);
                    break;
                case "link":
                    string x_href = "";
                    if (subtag.Attributes.GetNamedItem("href") != null)
                        x_href = subtag.Attributes.GetNamedItem("href").InnerText;
                    link_href[link_id] = x_href;
                    link_descr[link_id] = subtag.InnerText;
                    link_id++;
                    break;
                case "image":
                    Texture2D imgUpd;
                    try {
                        imgUpd = (Texture2D)Resources.Load(subtag.InnerText, typeof(Texture2D));
                        image.texture = imgUpd;
                    }
                    catch {
                        Debug.Log("Failed to load image");
                    }
                    break;
            }
        }

        for(int i = link_id; i < 8; i++) {
            link_href[i] = "";
            link_descr[i] = null;
        }
        UpdateQuestButtons();
    }

    public void QuestClick(string href, string sender) {
        //Debug.Log("Користувач переходить з вершини " + sender + " у вершину " + href);
        if (href == "reset_fail") {
            parent.sm.LoadScore();
            parent.sm.GameMode("main");
            return;
        }
        QuestLoad(href);
    }

    private void CatchNotify(string errMsg = "NULL") {
        Debug.LogError(errMsg);
        parent.error_code = true;
        text.text = "В ході інтерпретації файлу сценарію виникла помилка:\r\n" + errMsg + "\r\n\r\nСпробуйте запустити гру без підтримки зовнішніх XML-сценаріїв";
        link_href[0] = "exit"; link_descr[0] = "Повернутись в головне меню";
        UpdateQuestButtons();
    }

    private void UpdateStoryImage(XmlNode xn) {
        int t_size = 14;
        Color t_color = Color.black;
        Text text = GameObject.Find("bannerText").GetComponent<Text>();
        RawImage image = GameObject.Find("questImage").GetComponent<RawImage>();
        CanvasGroup story_cg = GameObject.Find("questImage").GetComponent<CanvasGroup>();
        RectTransform story_rt = GameObject.Find("questImage").GetComponent<RectTransform>();
        if (xn.Attributes.GetNamedItem("href") != null)
            story_href = xn.Attributes.GetNamedItem("href").InnerText;
        foreach (XmlNode subtag in xn.ChildNodes) {
            switch (subtag.Name) {
                case "image":
                    Texture2D imgUpd;
                    try {
                        imgUpd = (Texture2D)Resources.Load(subtag.InnerText, typeof(Texture2D));
                        image.texture = imgUpd;
                    }
                    catch {
                        Debug.Log("Failed to load image");
                    }
                    break;
                case "text":
                    text.text = subtag.InnerText;
                    if (subtag.Attributes.GetNamedItem("size") != null)
                        t_size = int.Parse(subtag.Attributes.GetNamedItem("size").InnerText);
                    if (subtag.Attributes.GetNamedItem("color") != null)
                        ColorUtility.TryParseHtmlString(subtag.Attributes.GetNamedItem("color").InnerText, out t_color);
                    text.text = subtag.InnerText;
                    text.fontSize = t_size;
                    text.color = t_color;
                    story_rt.SetSiblingIndex(100);
                    story_cg.alpha = 1;
                    story_cg.blocksRaycasts = true;
                    break;
            }
        }
    }

    public void UpdateQuestButtons() {
        float hi_top = 40;
        for (int i = 0; i < 8; i++) {
            Text text = GameObject.Find("var" + i).GetComponent<Text>();
            CanvasGroup cg = GameObject.Find("var" + i).GetComponent<CanvasGroup>();
            RectTransform rt = GameObject.Find("var" + i).GetComponent<RectTransform>();
            if (link_descr[i] != null){
                text.text = link_descr[i];
                cg.alpha = 1;
                cg.blocksRaycasts = true;
                Canvas.ForceUpdateCanvases();
                float height = rt.sizeDelta.y + 20;
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, hi_top);
                hi_top = hi_top + height;
            }
            else
            {
                text.text = "";
                cg.alpha = 0;
                cg.blocksRaycasts = false;
                rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            }
        }
    }
}
