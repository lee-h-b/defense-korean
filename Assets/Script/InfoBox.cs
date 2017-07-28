using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
/* 인포박스구상 <이건 창에둘거임 이유는
 * 아이템슬롯,스킬슬롯에 닿을때마다 보여주게 할꺼라서
 * 1.마우스따라다니기 <<통상시는 비활성화?
 * 소속은 게임매니저
 * 2.마우스가 슬롯에 접촉하면 itemslot인지, 스킬슬롯인지확인하기
 * ㄴ구성요소 panel<image,text>
 * 3.이미지 띄우기(희망사항)
 * 4.간단한 수치들만 텍스트로 표시되기
 * 이미지는 아이콘에 닿으면 해당이미지를 빼가는역활
 * 이벤트시스템의 온엔터 뭐시기를 활용해서 해당 컴포넌트가 슬롯인지,스킬슬롯인지를 확인
 * 슬롯을 재정의할 필요가 있을거같음
 */
 //설명을 추가하면서 for의 길이를 -2로 줄이고 디아3처럼 설명 다음에 가격나오게바꿈
public class InfoBox : MonoBehaviour
{

    [SerializeField]
    private GameObject info;
    private RectTransform infoRect;
    public Slot slot;
    public static InfoBox instance;
    // Use this for initialization
    public GameObject Info
    {//프로퍼티 사용
        get { return info; }
    }
    void Start () {
        infoRect = info.GetComponent<RectTransform>();
        info.SetActive(false);//통상시 비활성화
        instance = this;
	}
    public void ShowSkill(Skill skill, GameObject OnlyPassive = null)//이 온니 패시브는 상황에따라 달라질수있으니 게임오브젝트인채로둠
    {
            //내가쓸스킬이 뭔지
            SkillObject sup = skill.prefab.GetComponent<SkillObject>();
            //버그 가능

            Dictionary<int, string> dict = new Dictionary<int, string>();
            dict.Add(0, skill.skillName);
            dict.Add(1, ((int)skill.type).ToString());
            //여기까진 패시브,액티브 똑같음
            //여기아래부턴 패시브의 경우 설명을 넣어야겠음
            if (skill.type == SKILLTYPE.PASSIVE)
            {
            //임시로 
            float[] value1 = OnlyPassive.GetComponent<PassiveSkill>().GetAllValues();

            dict.Add(2, string.Format(skill.description,value1[0],value1[1],value1[2]));
            ChangeImage(skill.image);
            WritePassiveSkill(dict);            
            }
            else
            {
                dict.Add(2, skill.power.ToString("F2"));//공격력의 ~배 이렇게 써질것
                dict.Add(3, skill.cost.ToString());//소모 마나 : 이렇게 써질것

                dict.Add(4, ((float)sup.etc + sup.addEtc).ToString());//스킬타입에따라 속도,지속시간이될거임



                if (skill.type == SKILLTYPE.PLACE)
                {
                    dict.Add(5, (sup.GetComponent<SphereCollider>().radius + sup.addSize).ToString());//이건 크기 범위기술한정 
                    dict.Add(6, skill.description);
                }
                else dict.Add(5, skill.description);


                ChangeImage(skill.image);
                WriteActiveSkill(dict);
            }
    }
	public void ShowInfo(ITEM item)
    {
//        box.SetActive(true);

        EquipItem temp = item as EquipItem;
        Dictionary<int, string> dict = new Dictionary<int, string>();//하드코딩의 시작!
        //원래는 값 하나하나를 넣어서 인포박스를 통해넘기고 체크하려했음
        if (item.GetItemType() == ITEMTYPE.CONSUMPTION)
        {
            dict.Add(0, item.itemName);
            dict.Add(1, ((int)item.GetItemType()).ToString());//이름옆에 쓰고싶음
            dict.Add(2, item.hp.ToString());//여기부터는 스트링으로 포장해서 보내고 인포박스에서 이걸 열어봤는데 값이 0이면 안씀
            dict.Add(3, item.mp.ToString());
            dict.Add(4, item.description.ToString());
            dict.Add(5, item.price.ToString());
            //TODO차라리 여기서부터 조건을걸어서 더하던 말던하는게 낫지않을까?
            ChangeImage(item.image);
            WriteItem(dict);
        }
        else
        {//소모품때랑 엇비슷
            dict.Add(0, temp.itemName);
            dict.Add(1, ((int)temp.GetItemType()).ToString());//타입을 받고 거기서 소모품인지,장비품인지 또확인함 왜냐면 표시방식이 좀달라지기위함
            dict.Add(2, temp.atk.ToString());
            dict.Add(3, temp.def.ToString());
            dict.Add(4, temp.hp.ToString());
            dict.Add(5, temp.mp.ToString());
            dict.Add(6, temp.criPer.ToString());
            dict.Add(7, temp.dodgePer.ToString());
            dict.Add(8, temp.speed.ToString());
            dict.Add(9, temp.delay.ToString());//바뀔듯?
            dict.Add(10, temp.range.ToString());//공격옆으로 옮길듯?
                                                //ㄴ근데 6~10은 미구현에가까움ㅎ
            dict.Add(11, temp.description.ToString());
            dict.Add(12, temp.price.ToString());

           ChangeImage(temp.image);
           WriteItem(dict);
        }
 }
    public void ChangeImage(Sprite img)
    {
        info.transform.Find("InfoImage").GetComponent<Image>().sprite = img;        
    }
    //패시브를 따로한이유 < 그냥 보기 편하게할려고, 큰이유는 없음
    public void WritePassiveSkill(Dictionary<int, string> text)
    {
        Text paper = info.transform.Find("InfoText").GetComponent<Text>();
        Text sideText = paper.transform.GetChild(0).GetComponent<Text>();
        RectTransform size = paper.GetComponent<RectTransform>();
        for (int i = 0; i < text.Keys.Count; i++)
        {
            switch (i)
            {
                case 0: sideText.text += "이름 : " + text[i] + "\n"; break;
                case 1: sideText.text += "타입 : " + SkillDB.instance.TYPENAME[int.Parse(text[i])] + "\n"; break;
                default: break;

            }
        }
        paper.text += "\n설명 : " + text[text.Keys.Count - 1] + "\n";

        if (sideText.preferredWidth > 67)
        {
            info.GetComponent<RectTransform>().sizeDelta += new Vector2(sideText.preferredWidth - 67, 0);
        }
        info.GetComponent<RectTransform>().sizeDelta += new Vector2(0, paper.preferredHeight);


    }

    public void WriteActiveSkill(Dictionary<int,string> text)
    {
        Text paper = info.transform.Find("InfoText").GetComponent<Text>();
        Text sideText = paper.transform.GetChild(0).GetComponent<Text>();
        RectTransform size = paper.GetComponent<RectTransform>();
        for(int i = 0; i< text.Keys.Count ; i++)
        {
            //스킬은 왠만해선 다있을거라가정함 하지만 설명은 맨마지막
//            print(i);//5에서 에러가 발생함 유도기능은 참,거짓이니깐그래
            //트라이펄스하면되겠는데 하드코딩함
//            if (i > 2 && i != 5 && float.Parse(text[i]) == 0) continue;
            switch(i)
            {                
                case 0: sideText.text += "이름 : " + text[i] +"\n"; break;
                case 1: sideText.text += "타입 : " + SkillDB.instance.TYPENAME[int.Parse(text[i])] + "\n"; break;
                case 2: paper.text += "공격력의" + text[i] + "배\n"; break;
                case 3: paper.text += "소모 마나 : " + text[i] + "\n"; break;
                case 4:
                    {
                        if(int.Parse(text[1]) == (int)SKILLTYPE.PLACE ||
                            int.Parse(text[1]) == (int)SKILLTYPE.PEACE && float.Parse(text[i]) != 0 )
                            //TODO :: 여기에 투두하기엔 좀그런데 버프스킬을 따로하나만듬
                            //될수있다면 안만드는게 어떤가싶음
                            paper.text += "지속시간 : " + text[i] + " 초\n";

                        else if(int.Parse(text[1]) == (int)SKILLTYPE.NONTARGET ||
                            int.Parse(text[1]) == (int)SKILLTYPE.GUIDED)  paper.text += "스피드 : " + text[i] ;
                        break;
                    }
                case 5:
                    if (int.Parse(text[1]) == (int)SKILLTYPE.PLACE)
                    {

                        paper.text += "크기 : " + text[i] + "\n";
                    }
                break;
            }
        }
        paper.text += "\n설명 : " + text[text.Keys.Count - 1] + "\n";

        if (sideText.preferredWidth > 67)
        {
            info.GetComponent<RectTransform>().sizeDelta += new Vector2(sideText.preferredWidth - 67, 0);
        }
        info.GetComponent<RectTransform>().sizeDelta += new Vector2(0, paper.preferredHeight);

    }
    public void WriteItem(Dictionary<int, string> text)
    {
        Text paper = info.transform.Find("InfoText").GetComponent<Text>();
        Text sideText = paper.transform.GetChild(0).GetComponent<Text>();
        RectTransform size = paper.GetComponent<RectTransform>();
        // var temp = paper.preferredHeight;

        //        print(size.rect.width);
        //아래는 소모품형식의 경우에 출력하는 공식같은것
        if (int.Parse(text[1]) == (int)ITEMTYPE.CONSUMPTION) 
        {
            for (int i = 0; i < text.Keys.Count - 2; i++)
            {

                if(i>= 2)//더이상없다면 끝내기
                    if (int.Parse(text[i]) == 0)
                        continue;
                
                    switch (i)
                    {//개행할때마다 rect의 height가 늘어남
                        case 0: sideText.text += "이름 : " + text[i] + "\n"; break;
                            //타입은 정비 필요
                        case 1: sideText.text += "타입 : " + ItemDB.instance.TYPENAME[int.Parse(text[i])] + "\n"; break;
                        case 2: paper.text += "체력 " + text[i] + " 회복\n"; break;
                        case 3: paper.text += "마력 " + text[i] + " 회복\n"; break;
                    }
            }
            paper.text += "\n설명 : " + text[text.Keys.Count - 2] + "\n";
            paper.text += "가격 : " + text[text.Keys.Count-1] + "원\n"; 
        }
        else
        {
            CaseEquip(text, paper,sideText);
        }
        if (sideText.preferredWidth > 67)
        {
            info.GetComponent<RectTransform>().sizeDelta += new Vector2(sideText.preferredWidth - 67, 0);
        }
        info.GetComponent<RectTransform>().sizeDelta += new Vector2(0, paper.preferredHeight);
//        size.sizeDelta  = new Vector2(paper.preferredWidth,paper.preferredHeight);
    }

    public void CaseEquip(Dictionary<int, string> text,Text paper,Text side)
    {
        //장비만 따로보냈음, 보기 좋게할려고
        for(int i = 0; i<text.Keys.Count - 2; i++)
        {
            if (i >= 2)//위랑 너무같음ㅎ..<< 값이 없을경우임
                if (float.Parse(text[i]) == 0)
                    continue;
            
            switch(i)
                {
                  case 0: side.text += "이름 : " + text[i] + "\n"; break;
                  //타입은 정비 필요
                  case 1: side.text += "타입 : " + ItemDB.instance.TYPENAME[int.Parse(text[i])] + "\n"; break;
                  case 2: paper.text += "공격력 : " + text[i] + "\n"; break;
                  case 3: paper.text += "방어력 : " + text[i] + "\n"; break;
                  case 4: paper.text += "체력 : " + text[i] + "\n"; break;
                  case 5: paper.text += "마력 : " + text[i] + "\n"; break;
                  case 6: paper.text += "크리확률 : " + text[i] + "%\n"; break;
                  case 8: paper.text += "속도 : " + text[i] + "%\n"; break;
                  case 9: paper.text += "공격딜레이 : " + text[i] + "초\n"; break;
                  case 10: paper.text += "사정거리증가 : " + text[i] + "%\n"; break;
//                  case 11: paper.text += "설명 : " + text[i] + "\n"; break;
//                  case 12: paper.text += "가격 : " + text[i] + " 원\n"; break;

                    }
        }
        if(text[text.Keys.Count-2].Length > 0)
        paper.text += "\n설명 : " + text[text.Keys.Count - 2] + "\n";
        paper.text += "가격 : " + text[text.Keys.Count - 1] + "원\n";

    }
    public void Reset()
    {
        Text sideText = info.transform.Find("InfoText").GetComponent<Text>().transform.GetChild(0).GetComponent<Text>();

        info.GetComponent<RectTransform>().sizeDelta -= new Vector2(sideText.preferredWidth > 67 ? sideText.preferredWidth - 67 : 0, 
            info.transform.Find("InfoText").GetComponent<Text>().preferredHeight);
       // info.GetComponent<RectTransform>().sizeDelta = Vector2.zero;
        sideText.text = null;
        info.transform.Find("InfoText").GetComponent<Text>().text = null;
        info.SetActive(false);
    }

    void Update () {
        var mouse = Input.mousePosition;
        float x;
        //대충중간지점쯤? 상점에 인포박스를 오른쪽으로 보이게함
        if (mouse.x < 300)
            x = mouse.x + 70f;
        else
            x = mouse.x - 70f;
        if(mouse.y < 200)
            infoRect.position = new Vector3(x, mouse.y + 15, 0);
        else
            infoRect.position = new Vector3(x, mouse.y - 15, 0);
        
    }
}
