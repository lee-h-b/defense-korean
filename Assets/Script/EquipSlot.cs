using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
//이제 인벤토리->장비에서 아이템능력치만큼 감소하는게필요함
//ㄴ뭔듯임 내가더블클릭으로 장비바꿀시에 능력치감소가 안이루워짐
//sub : EquipmentWindow정리 <instance추가가 주가될듯
//장비에서 더블클릭으로 해제하고, 재착용 버그있음 카운트가 1남는다
public class EquipSlot : InvenSlot {

    // Use this for initialization
    public ITEMTYPE placeType;
    public int beforeID = 0;//기존 ID쯤

    //드래그드롭같은걸 햇는데 이 플레이스 타입과 다르다면 바로 버리게
    //private를 통해 어찌하면 될거같긴 하지만 간편하게 public
    //언제검사하도록? update를 통해? 아니면 끼자마자?
    //윈도우에서 하지말고 여기 EndDrag에서하자
    //temp로 복제도했겠다 알고싶은거 80%는있다고봄
    public override void OnEndDrag(PointerEventData data)
    {
        {
            if (GameManager.inst.GetComponent<Inventory>() == null)//에러보기싫어서
                return;

                GameManager.inst.GetComponent<Inventory>().MovingObject.GetChild(0).SetParent(transform);
            //EndDrag, 드래그가 끝났을때 인벤토리 인스턴스의 1번째 자식의 부모를 자신으로
            transform.GetChild(0).localPosition = Vector3.zero;//그리고 정중앙에 위치
                                                               //내현재의 부모가 인벤토리가 일경우임
                                                               //여기서 만약 다른장비창에 뒀을경우를 둬야할듯함
//if (data.pointerPress.GetComponent<EquipmentWindow>() != null)
            {
                if (data.pointerEnter.GetComponent<EquipSlot>() != null)
                {
                    //여기서 맞는창인지 체크하도록함 <장비에서 장비로 드래그드롭할때 틀리면 그냥 돌아감
                    if (data.pointerEnter.GetComponent<EquipSlot>().placeType != this.item.GetItemType() )
                        return;

                }
                //장비->아이템창임

                EquipItem temp = (EquipItem)item;
                //            count = 1;//카운트
                if (this.item.ID != 0)//이아이디가 0이아니라면 해제
                {
                    temp.UnEquip(GameManager.inst.player);
                }//아이템해제

                //장비비활성화 드래그로 해제
                Equipviewer.inst.NonActive(item.GetItemType(true));


                int ctemp = count;

                count = GameManager.inst.GetComponent<Inventory>().selectSlot.count;
                item = GameManager.inst.GetComponent<Inventory>().selectSlot.item ;
                GameManager.inst.GetComponent<Inventory>().selectSlot.item = temp;
                GameManager.inst.GetComponent<Inventory>().selectSlot.count = ctemp;
  
                GameManager.inst.GetComponent<Inventory>().ItemDraw(this.transform);
                GameManager.inst.GetComponent<Inventory>().ItemDraw(GameManager.inst.GetComponent<Inventory>().selectSlot.transform);
                this.transform.GetChild(0).gameObject.SetActive(true);
                if (this.transform.parent.transform.Find("Weapon").GetComponent<EquipSlot>().item.ID == 0)//만약 아이템의 아이디가 0임
                    Equipviewer.inst.ActiveEquip(ITEMTYPE.WEAPON);
                //다하고 다시등록
            }
        }
    }
    //이제 장비창에서 더블클릭했을경우를 만들어야함
    public override void OnPointerDown(PointerEventData eventData)
    {
        if(eventData.clickCount == 2 && count > 0)
        {
            if (this.item.ID != 0)
            {
                var temp = this.item as EquipItem;
                temp.UnEquip(GameManager.inst.player);
            }//아이템해제

            //빈자리 만들어내기 + 아이템추가 + 인포박스 안보이게
            InfoBox.instance.Reset();
            //장비창에서의 해제
            Equipviewer.inst.NonActive(item.GetItemType(true));//원거리까지 해체할수 있도록 
            GameManager.inst.GetComponent<Inventory>().AddItem(item.ID);
             this.item = new EquipItem();                 
            count = 0;
            GameManager.inst.GetComponent<Inventory>().ItemDraw(this.transform);
            this.transform.GetChild(0).gameObject.SetActive(true);



        }
    }
    private void Start()
    {
        //안쓰일거같음
//        placeType;
    }
    // Update is called once per frame
    void Update () {
//        EquipCheck();
    }
    public void EquipCheck()
    {
            EquipItem temp = item as EquipItem;

            temp.Equip(GameManager.inst.player);

        if ((item as Weapon) != null)//이장비가 무기일경우
        {
            
            Weapon wTemp = (item as Weapon);
            //이무기가 원거리인지 아닌지를 먼저봄
            if (wTemp.GetItemType(true) == ITEMTYPE.RANGEDWEAPON)
                GameManager.inst.player.ranged = true;
            else GameManager.inst.player.ranged = false;

            if (wTemp.TwoHand == true)//이 장비가 양손일 경우
            {
                EquipItem shield = transform.parent.Find("Shield").GetComponent<EquipSlot>().item as EquipItem;

                //내가 끼게된 무기가 양손이면서 내쉴드 부분에 아이템 아이디(어짜피 해당종류 아이템이 쉴드여야 낄테니)가 0이 아닐경우
                if ( shield != null && shield.ID != 0)
                {
                    transform.parent.Find("Shield").GetComponent<EquipSlot>().item = new ITEM();//복제는 됐으니 기존의 아이템을 없앰

                    shield.UnEquip(GameManager.inst.player);//쉴드의 능력치감소
                    Equipviewer.inst.NonActive(shield.GetItemType());//쉴드의 타입의 이미지를 비활성
                    GameManager.inst.GetComponent<Inventory>().AddItem(shield.ID);//쉴드아이템추가
                    GameManager.inst.GetComponent<Inventory>().ItemDraw(transform.parent.Find("Shield"));
                }
            }
        }
        else if (item.GetItemType() == ITEMTYPE.SHIELD)//이장비가 방패인경우에
        {
            Weapon weapon = transform.parent.Find("Weapon").GetComponent<EquipSlot>().item as Weapon;
            if(weapon != null && weapon.TwoHand == true)
            {
                transform.parent.Find("Weapon").GetComponent<EquipSlot>().item = new ITEM();
                weapon.UnEquip(GameManager.inst.player);
                Equipviewer.inst.NonActive(weapon.GetItemType(true));
                GameManager.inst.GetComponent<Inventory>().AddItem(weapon.ID);
                GameManager.inst.GetComponent<Inventory>().ItemDraw(transform.parent.Find("Weapon"));
            }
        }
            
    }
}
