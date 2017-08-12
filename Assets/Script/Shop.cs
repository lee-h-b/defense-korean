using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/* 상점과 인벤토리의 차이?
 * 상점에선 더블클릭하면 구매가되던가 해야, 즉 장착하면안됨
 * 드래그금지 슬롯을 따로만들게 될거같음
 * 구입버튼누르고 사게할까 싶은데 그냥 더블클릭으로 사는게 낫겠음
 */

public class Shop : MonoBehaviour {

    //패널에서 구입을 담당하니 얘는 그냥 판매만 어떻게 구현하면됨
    //자기자식으로 prefab에 있는 샵패널을 부르고, 그샵패널에 아이템을추가시키는식정도가 되겠음
    //ㄴ전부 start에서함?
    public Dropdown filter;
    public Player guest;//손님
    public static Shop inst;
    public GameObject defaultPanel;
    public GameObject showCase;
    public GameObject shopScreen;
    public bool sell =false;
    float limitScroll;
    
    public void turnSell()
    {
        if(sell == false)
        {
            GameManager.inst.GetComponent<CursorChange>().ChangeCursor(2);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        sell = !sell;
        //커서바꿈

    }
    public void limitCheck()
    {
        if(showCase.GetComponent<RectTransform>().localPosition.y > limitScroll)
        {
            showCase.GetComponent<RectTransform>().localPosition = new Vector3(0,limitScroll);
        }
        else if(showCase.GetComponent<RectTransform>().localPosition.y  < 0 )
            showCase.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);

    }
    public void OnOff()
    {
        if(shopScreen.activeSelf == true)
        {
            if (sell == true)
                turnSell();
            shopScreen.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            shopScreen.SetActive(true);
        }
    }

    void MakeItem(int ID)
    {
        var panel = Instantiate(defaultPanel);
        panel.transform.SetParent(showCase.transform,false);
        panel.GetComponent<ShopPanel>().SetItem(ID);

    }

    void Start()
    {  
        if(inst == null)      
        inst = this;
        filter = shopScreen.transform.Find("OutlinePanel").transform.Find("Dropdown").GetComponent<Dropdown>();
    }
    public int Filter()
    {
        int ignoreCount = 0;
        for (int i = 0; i < showCase.transform.childCount; i++)
        {//0번부터 전부 훓어보기시작
            var cur = showCase.transform.GetChild(i).gameObject;
            switch (filter.value)
            {
                case 0:
                    {
                        cur.gameObject.SetActive(true);
                        continue;//브레이크도 아까움 바로 넘어가기
                    }

                case 1:
                    if (cur.GetComponent<ShopPanel>().item.GetItemType() == ITEMTYPE.CONSUMPTION)
                    {
                        cur.gameObject.SetActive(false);//거짓으로 돌림
                        ignoreCount++;//이그노어 카운트라는 애를 올림
                    }
                    else
                        cur.gameObject.SetActive(true);
                    break;
                case 2:
                    if (cur.GetComponent<ShopPanel>().item.GetItemType() != ITEMTYPE.CONSUMPTION)
                    {
                        cur.gameObject.SetActive(false);
                        ignoreCount++;
                    }
                    else
                        cur.gameObject.SetActive(true);//그이외는 전부 참으로
                    break;
            }
        }
        if (ignoreCount + 4 >= showCase.transform.childCount)
        {
            limitScroll = 0;
            limitCheck();
        }
        return ignoreCount;

    }
    // Update is called once per frame
    void Update () {

        if (showCase.transform.childCount - Filter() >= 5)
        {
            limitScroll = (showCase.transform.childCount - 4) * defaultPanel.GetComponent<RectTransform>().rect.height;
            limitCheck();
        }

    }
    //
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && GameManager.inst.InBattle == false)
        {
            other.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            Time.timeScale = 0;
            //하드코딩의 잔재
            MakeItem(101);
            MakeItem(102);
            MakeItem(103);
            MakeItem(151);
            MakeItem(152);
            MakeItem(153);
            MakeItem(176);
            MakeItem(177);
            MakeItem(178);
            MakeItem(201);
            MakeItem(301);
            MakeItem(401);
            MakeItem(501);
            MakeItem(701);
            MakeItem(801);
            MakeItem(901);
            MakeItem(1001);
            MakeItem(1002);
            MakeItem(1003);
            if (showCase.transform.childCount >= 5)
            {
                limitScroll = (showCase.transform.childCount - 4) * defaultPanel.GetComponent<RectTransform>().rect.height;
            }
            else limitScroll = 0;
        


        OnOff();
            other.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //진열장에 있는 자식중 아이템패널을 전부 지우기
        int i = 0;
            while( i< showCase.transform.childCount)
        {
            if (showCase.transform.GetChild(i).name == "ItemPanel(Clone)")
            {
                Destroy(showCase.transform.GetChild(i).gameObject);
            }
            i++;

        }
    }

}
