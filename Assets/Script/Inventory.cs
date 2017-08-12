using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

 /// <summary>
 /// ///////////기존에 업섣ㄴ오류가 생긴다면 인벤토리 포메이션때문에 문제가 생기는거일수 있음
 /// </summary>

public class Inventory : MonoBehaviour {


    //    public List<ItemInventory> items = new List<ItemInventory>();
    //items 에서 슬롯리스트로 바뀜 아이템's 는 이 메소드에 모든걸 넣은방식이였음
    //이번에 바뀌는방식으로하면 생성된 얘로 내려가서 관리가됨 아마 좀더편할거같음?
    public GameObject quickList;
    public GameObject itemSlotPannel;
//    public GameObject InventoryMainObject;
    public Transform InventoryFormation; //인벤토리를 가르킴
    
    private bool activeScreen = false;
    public int Maxcount = 20;//이름만 봐서는 인벤토리 한계값 따라서 내인벤한계인 20

    public Transform MovingObject;//드래그할때 사용할 좌표같은아이

    //여기까지
    public GameObject Invenscreen; //find는 비활성화된 얘를 못찾음
    //캐릭매니저가? 아님 여기서? 여기서로
    public List<InvenSlot> slotList = new List<InvenSlot>();//슬롯으로 새리스트 만듬 아이템슬롯 하나하나 관여
    //ㄴ이것도 딕셔로?
    //상시접근이니 할필요성 크게못느낌

    public InvenSlot selectSlot;//현재 선택된 슬롯쯤

    //장비템은 장비니깐 여기서 참조안할줄알았는데 해야될거같음
    public EquipmentWindow equipWindow;
//    public static Inventory instance;//인스턴스
    public void ItemDraw(Transform slotpos)//_slot으로 될까
    {
        if( slotpos.GetComponent<InvenSlot>().count == 0 )//카운트가 0인소모품은 안그림 <없으니깐
        {
            slotpos.GetChild(0).gameObject.SetActive(false);//수정필요할듯
            slotpos.transform.GetChild(0).GetComponent<Image>().sprite = null;
        }
        else
        {
            slotpos.GetChild(0).gameObject.SetActive(true);//이미지 활성화
            slotpos.transform.GetChild(0).GetComponent<Image>().sprite =
                slotpos.GetComponent<InvenSlot>().item.image;
            if (slotpos.transform.GetChild(0).childCount > 0)
            {
                Text countOrstatus = slotpos.transform.GetChild(0).GetChild(0).GetComponent<Text>();//이름을쉽게 카운트로
                                                                                                                //transform으로 했더니 엄청길어짐
                if (slotpos.GetComponent<InvenSlot>().item.GetItemType() == ITEMTYPE.CONSUMPTION)
                {
                    countOrstatus.text = slotpos.GetComponent<InvenSlot>().count.ToString();
                    slotpos.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
                    //소모품의 갯수를 표시시킴 << fillimage던가 그건 안해도될꺼같은데
                }
                else
                    slotpos.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
            }
            
        }
    }

    public void OnOffScreen()
    {

        if(activeScreen == false )
        {
            Invenscreen.SetActive(true);
            activeScreen = true;
        }
        else if(activeScreen == true)
        {
            if (InfoBox.instance != null)
            {
                InfoBox.instance.slot = null;
                if (InfoBox.instance.Info.activeSelf == true)
                {
                    InfoBox.instance.Reset();
                    InfoBox.instance.Info.SetActive(false);
                }
            }

            Invenscreen.SetActive(false);
            activeScreen = false;
//            UpdateInventory();
        }
        //원래 업뎃에 있었는데 난이렇게 만들었으니 여기에 넣어봄
    }

    public void AddItem(int ID,int count =1, bool swap = true)//카운트와 해당아이템을 찾을 ID만 챙긴다
    {
            int index = 21;
        //퀵슬롯에 아이템 있는걸 체크
        if (swap == false)//스왑이 거짓일경우에만 퀵슬롯체크
        {
            for (int i = 0; i < quickList.transform.childCount /2; i++)
            {
                //편의를 위해 템프라는걸만들어서 이게 null(없거나 스킬)일경우 혹은 아이템아이디가 다르면 다음으로 넘어갈수있도록
                var temp = quickList.transform.GetChild(i).GetComponent<QuickSlot>().slot as InvenSlot;
                if (temp == null || temp.item.ID != ID) continue;
                else//아이디가 같을경우
                {
                    (quickList.transform.GetChild(i).GetComponent<QuickSlot>().slot as InvenSlot).count += count;
                    ItemDraw(quickList.transform.GetChild(i));
                    return;//아이템 추가하고 끝내기
                }
            }
        }
        for (int i = 0; i < Maxcount; i++)
        {
            //조건은 총3가지 1.아이템아이디값이 같을경우 2. 소모품일경우 3.총갯수가98개일경우 이럴경우 중첩을행한다
            if (slotList[i].item.ID == ID && slotList[i].item.GetItemType() == ITEMTYPE.CONSUMPTION && slotList[i].count < 99)
            {
                slotList[i].count += count;
                ItemDraw(slotList[i].transform);
                //중첩하면 탈출함
                return;
            }
            //아니면 index할자리 찾으러감
            if(slotList[i].item.ID == 0  && index >= Maxcount)
            {
                index = i;
            }
        }
        if (slotList[index].item.ID == 0 && slotList[index].count == 0)//만약 슬롯리스트의 아이디가 없으면서 카운트가 0일경우
            {
            //ID만 보면될거같지만 이렇게 이중으로 검사시켜봤음 <<카운트는 보면안됨 중보고디면 중첩해야지
            //                int index = ItemDB.instance.items.Binary,ID);
            //에러가능
            if (ItemDB.instance.items.ContainsKey(ID))
                slotList[index].item = ItemDB.instance.items[ID];
                slotList[index].count += count;
                 //오류가능
                ItemDraw(slotList[index].transform);
            }

    }


    private void Start()
    {
        for (int i = 0; i < Maxcount; i++)
        {
            GameObject temp = Instantiate(itemSlotPannel);
            temp.transform.SetParent(InventoryFormation);
            temp.name = "Slot" + " " + (i);
            //이거땜에 인벤토리로 사용자를 옮김 인벤토리가 인벤토리 관리하는거니 큰문제는 없어야하는데
            //어쨌던 temp수만큼 넣었고

            slotList.Add(temp.GetComponent<InvenSlot>());

        }
    }


    private void Update()
    {
    }
}
[System.Serializable]
public class ItemInventory
{
    public int ID;
    
    public GameObject ItemGameObj;
    public int count;


}
