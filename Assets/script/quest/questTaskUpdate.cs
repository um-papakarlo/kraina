using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class questTaskUpdate : MonoBehaviour {
	private float tm_height = 0;
	private RectTransform questTask;

	void Start() {
		RectTransform topMenu = GameObject.Find("topMenu").GetComponent<RectTransform>();
		if (topMenu != null)
			tm_height = topMenu.rect.height;
		questTask = GameObject.Find("questTask").GetComponent<RectTransform>();
	}

	void Update() {
		if(questTask != null)
			questTask.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, tm_height + (questTask.rect.height - 12));
	}
}
