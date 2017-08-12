using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SkillPanel : MonoBehaviour {
    public Button[] tabs = new Button[3];
    private Button lastSelect = null;
    public static SkillPanel instance;
    public Transform dragHelper;//인벤토리를 응용해서 드래그로 스킬아이콘 보이도록
    public GameObject screen;
    public void OnOffScreen()
    {//todo onoff때 슬롯설명이 있으면 그슬롯설명사라지도록하기
            if(InfoBox.instance != null)
            {
                InfoBox.instance.slot = null;
                if(InfoBox.instance.Info.activeSelf == true)
                {
                    InfoBox.instance.Reset();
                    InfoBox.instance.Info.SetActive(false);
                }
            }
        screen.SetActive(!screen.activeSelf);
    }
    public void SelectTab(Button button)
    {
        if (lastSelect != null)
        {
                lastSelect.interactable = true;
            lastSelect.transform.Find("Inline").gameObject.SetActive(false);
        }
        button.interactable = false;
        button.transform.Find("Inline").gameObject.SetActive(true);
        lastSelect = button;
    }
    // Use this for initialization
    void Start () {
        instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
