using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//아이템->장비 드래그드랍 안되
public class InvenSlot : MonoBehaviour, IDragHandler, IPointerEnterHandler,
    IPointerExitHandler, IEndDragHandler, IPointerDownHandler, Slot
{

    //드래그 드롭이 이상하게됨 정반대 형식으로 적용된다
    //빈공간에서 이미지로 드래그해야 제대로먹힘?
    //그리고 이미지가 잇어야함
    //레이캐스트푸니깐 해결 됨ㅡ

    public int count = 0;//카운트 뭔가 받으면 최소 1, 1이어야 보임
    //장비슬롯을 응용해야할거같음

    //더블클릭하면 라스트유즈타임이 정의되고, 장비면 장비슬롯 갔으면 장비슬롯에서 딜레이를 줌딜레이타임은(아이템 추가할때 기본으로 있음)
    //드래그드랍하면 라스트유즈타임과 딜레이타임이 같이감 왜냐면 그 아이템을 썼다면 그만한 쿨이 돌아야 할테니
    //근데이러면 다른 아이템은 쓸수가 있는건데 하게된다면 쿨하게 소모품만 적용할듯
    public float coolTime = 0;

    public ITEM item;//아이템db는 아이템 데이터베이스고 실제아이템은 ITEM
    //이스크립을 재정의해야할거같아서

    public void OnDrag(PointerEventData data)
    {//이함수는 IDragHandler를 쓰면 반드시 써야하는 함수임

        //자신의 자식의 수를 먼저확인해봄 1개라도 있어야함
        if (transform.childCount > 0 && this.count != 0)//만약 자신의 인벤에 childeCount가 있다면은
        {//뒤에 count 체크는 빈공간에서부터 옮기기 방지
            transform.GetChild(0).SetParent(GameManager.inst.GetComponent<Inventory>().MovingObject);
            //대입안되고 이동?복사? 그런느낌
        }
        //0번째자리의 부모를 인벤인스턴스의 움직이는 오브젝트로함
        if (GameManager.inst.GetComponent<Inventory>() != null)
            GameManager.inst.GetComponent<Inventory>().MovingObject.GetChild(0).position = data.position;
        //todo :: 유니티예외처리발생함 < 아무것도 안들고있으니깐 
        if (InfoBox.instance.Info.activeSelf == true)
        {
            InfoBox.instance.Reset();
            //            InfoBox.instance.Info.SetActive(false);
        }

    }
    public void OnPointerEnter(PointerEventData data)
    {//IPointerEnterHadnler인터페이스 에 있던함수
     //이슬롯에 들어가면ㅇ  
     //해당 슬롯에 들어가면 발동되는식임
     //     if(GameManager.inst.GetComponent<Inventory>() != null)
        GameManager.inst.GetComponent<Inventory>().selectSlot = this;//포인터가 들어가면

        if (InfoBox.instance != null)
        {
            InfoBox.instance.slot = this;
            if ( this.item.ID != 0 && count > 0)//이동하다가 인벤문제?
            {
                InfoBox.instance.Info.SetActive(true);
                InfoBox.instance.ShowInfo(item);
            }
        }

    }
    public void OnPointerExit(PointerEventData data)
    {//IPointerExitHandler인터페이스 에 있던 함수
        if (GameManager.inst.GetComponent<Inventory>() != null)
            GameManager.inst.GetComponent<Inventory>().selectSlot = null;//포인터가 나갈땐 다시 null로돌림
        if (InfoBox.instance != null)
        {
            InfoBox.instance.slot = null;
            if (InfoBox.instance.Info.activeSelf == true)
            {
                InfoBox.instance.Reset();
                InfoBox.instance.Info.SetActive(false);
            }
        }


    }
    public virtual void OnEndDrag(PointerEventData data)
    {//IEndDragHandler 에 함수
        GameManager.inst.GetComponent<Inventory>().MovingObject.GetChild(0).SetParent(transform);
        //EndDrag, 드래그가 끝났을때 인벤토리 인스턴스의 1번째 자식의 부모를 자신으로
        transform.GetChild(0).localPosition = Vector3.zero;//그리고 정중앙에 위치
                                                           //내현재의 부모가 인벤토리가 일경우임
        
        if (gameObject.name == GameManager.inst.GetComponent<Inventory>().selectSlot.name)
            return;
        if (InfoBox.instance.Info.activeSelf == false)
        {//드래그 끝나면 다시 보여주는식
         // InfoBox.instance.Reset();
            InfoBox.instance.Info.SetActive(true);

            InfoBox.instance.ShowInfo(item);
        }

        //이조건에서 갔더니 equip소속인경우도 포함시켜야함
        if (data.pointerEnter.GetComponent<EquipSlot>() != null)
        {
            //여기서 맞는창인지 체크하도록함 <장비에서 장비로 드래그드롭할때 틀리면 그냥 돌아감
            //아이템ID가 0이아닌, 즉 장비중일경우에는
            SlotUse();
            //아이템장비적용은 여기서
            //ㄴ슬롯유즈를 한이유는 장비를 더블클릭하면 (사용하면) 끼게되니깐
            //드래그드랍으로 하는식이였는데 결과는 똑같으니 이렇게햇음
        }

        else if (GameManager.inst.GetComponent<Inventory>().selectSlot != null)//선택한슬롯이 있으면
        {
            //TODO : 퀵슬롯에 아이템이 등록되있으면 현재는 인벤토리 밖에 안보니깐
            //중첩을 안시켜주게됨
            if (item == GameManager.inst.GetComponent<Inventory>().selectSlot.item && item.GetItemType() == ITEMTYPE.CONSUMPTION)
            {
                //만약 아이템 이 서로같은데 오브젝트명이다를경우로ㅇㅇ
                item = new ITEM();
                data.pointerEnter.GetComponent<InvenSlot>().count += count;
                count = 0;
                //추가한다음 드로우
                GameManager.inst.GetComponent<Inventory>().ItemDraw(this.transform);
                GameManager.inst.GetComponent<Inventory>().ItemDraw(GameManager.inst.GetComponent<Inventory>().selectSlot.transform);
                return;
            }

            ITEM temp = item;//현재아이템을 temp에저장함
            item = GameManager.inst.GetComponent<Inventory>().selectSlot.item;//그리고 현재아이템을
                                                      //인스턴스에서 선택된 아이템슬롯으로 바꿈
            GameManager.inst.GetComponent<Inventory>().selectSlot.item = temp;
            //ㄴ 결국 스왑

            int ctemp = count;//안바뀌던데? 카운트도 옮겨봄
            count = GameManager.inst.GetComponent<Inventory>().selectSlot.count;
            GameManager.inst.GetComponent<Inventory>().selectSlot.count = ctemp;
            //스왑이니깐 해당인스턴스의 선택된 슬롯의 아이템을 템프로바꿈


            GameManager.inst.GetComponent<Inventory>().ItemDraw(this.transform);//자신그리기
            //여기서 목적지가 장비창이 아닐경우로 해야할듯


            GameManager.inst.GetComponent<Inventory>().ItemDraw(GameManager.inst.GetComponent<Inventory>().selectSlot.transform);//현재가리키고있는얘그리기
                                                                                 //ㄴ이게문제겠음 인벤토리가 없는데 인벤토리를 불러서인거같음
                                                                                 // Transform child out of bounds <오류
                                                                                 //그리고 추가로 부모가 인벤토리가 아니라면으로 가정도 넣어서 이동해서적용되게?
        }


    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (Shop.inst.sell == true && item.ID != 0)
        {
            GameManager.inst.player.gold += (uint)(item.price / 2f);
            count -= 1;
            if (count == 0)
                item = new ITEM();
            GameManager.inst.GetComponent<Inventory>().ItemDraw(this.transform);
            eventData.clickCount = 0;
        }
        else if (eventData.clickCount == 2 && count > 0)
        {
            SlotUse();
            eventData.clickCount = 0;
        }
        if (count == 0)
            InfoBox.instance.Reset();


    }

    public void SlotUse()
    {
        //기존에 장비됐던게 또다시 장비할려하면 공격력증가를 안하게됨

        if (this.item.GetItemType() == ITEMTYPE.CONSUMPTION && count > 0)
        {
            if (coolTime + item.delay > Time.time)
            {
                GameManager.inst.CreateText(GameManager.inst.player.gameObject, "준비되지 않았습니다", Color.black);
                return;//만약 쿨타임+ 아이템딜레이가 타임.타임보다 높다면 < 걍리턴
            }

            //이걸 메소드로만들어서 외부에서사용하게
            //ㄴ그렇게하면 퀵슬롯에서 호출하면 쉬움
            coolTime = Time.time;
            Player player = GameManager.inst.player;
            count--;
            player.virtualStatus.hp += this.item.hp;
            player.virtualStatus.mp += this.item.mp;
            //아이템의 증가는 가상을 통해 채워줌
            player.CheckOverHpMp();
            if (count == 0)
            {
                item = new ITEM();
            }
            GameManager.inst.GetComponent<Inventory>().ItemDraw(this.transform);//해당위치를 다시그려서 갱신
        }
        else
        {//만약 장비템일경우에는? 해당장비코드와 같은 위치로가서 아이템 스왑을 행함
         //실질적으로 해당이미지를 그리도록?

            EquipItem Etemp = this.item as EquipItem;

            //ㄴ 구아이템 담기용
            //좀줄이자 스위치케이스라 슬롯이 너무길어짐, type의 슬롯값-1위치에 담는걸로하면 1/5로 줄어들거임
            //ㄴranged는? 그럼 디폴트로하던가하면됨


            switch (item.GetItemType())
            {
                //더블클릭하면합체됨 그게중요한가 하드코딩이중요하지 
                case ITEMTYPE.WEAPON:
//                case ITEMTYPE.RANGEDWEAPON:
                    //                      GameManager.inst.GetComponent<Inventory>().equipWindow.equips[0] = item as EquipItem;
                    
                    Etemp = GameManager.inst.GetComponent<Inventory>().equipWindow.equipSlot[0].item as EquipItem;//이건 장비로써 넘기는건데 큰문제는 없다고봄
                    GameManager.inst.GetComponent<Inventory>().equipWindow.equipSlot[0].item = item as EquipItem;//ㄴ 무기클래스 자체가 노쓸모일지도
                    GameManager.inst.GetComponent<Inventory>().equipWindow.equipSlot[0].count = 1;
                    GameManager.inst.GetComponent<Inventory>().equipWindow.equipSlot[0].EquipCheck();

                    GameManager.inst.GetComponent<Inventory>().ItemDraw(GameManager.inst.GetComponent<Inventory>().equipWindow.equipSlot[0].transform);
                    break;
                default://왜 -2? 칸의차이임 별거(?)없음)
                    Etemp = GameManager.inst.GetComponent<Inventory>().equipWindow.equipSlot[(int)item.GetItemType() - 2].item as EquipItem;
                    //                     GameManager.inst.GetComponent<Inventory>().equipWindow.equips[1] = item as EquipItem;
                    GameManager.inst.GetComponent<Inventory>().equipWindow.equipSlot[(int)item.GetItemType() - 2].item = item as EquipItem;
                    GameManager.inst.GetComponent<Inventory>().equipWindow.equipSlot[(int)item.GetItemType() - 2].count = 1;
                    GameManager.inst.GetComponent<Inventory>().equipWindow.equipSlot[(int)item.GetItemType() - 2].EquipCheck();
                    GameManager.inst.GetComponent<Inventory>().ItemDraw(GameManager.inst.GetComponent<Inventory>().equipWindow.equipSlot[(int)item.GetItemType() - 2].transform);
                    break;

                    //TODO:여기서 쉴드의경우 RANGEDWEAPON이 껴질경우 벗겨져야함
            }
            Equipviewer.inst.ActiveEquip(item.GetItemType(true));//아이템사용하면서 여기서 아이템이 장착됐다는 실제 표현을하게됨
            //ㄴ장비슬롯으로 넘기는게 좋지않을까
            //ㄴ 장비슬롯이 이전아이템을 가지고있진 않음 있어도 ID뿐이고 그걸통해 생성한다면 좀복잡할듯
            //ㄴ그래서 여기서 장착하면 슬롯그리듯이 할것
            count = 0;
            //                this.item = Etemp as ITEM;
//            (Etemp as Weapon).TwoHand = false;
            //ㄴ Etemp가 본래 EquipItem임 근데 웨폰에 몇몇기능이 추가되었고 이걸 안전히 꺼내기위해서는 맨아래인 Weapon으로해야함?
            //이렇게 하니 문제가 가독성임 난 장비인데 Weapon으로 함수장난을 쳐야함 그럼 장비가 들어갔을때 얘의 형태를 알아보는건?
            //Weapon형을 하나 더잡아야함 
            //ㄴ Etemp에 괄호치니 잘된다


            item = new ITEM();//아이템값을 우선비움
            if (Etemp != null && Etemp.ID != 0)//양손무기의 경우를 생각하면 1개더 있어야할듯
            {//만약복제했던 Etemp가 안비어있을경우
                Equipviewer.inst.NonActive(Etemp.GetItemType(true));
                Etemp.UnEquip(GameManager.inst.player);
                //                GameManager.inst.GetComponent<Inventory>().AddItem(Etemp.ID);//AddItem이아니라 뭔가로 단순아이템 교체로바꿔볼까
                //바꿔서 얻은 이점 템복사우려안됨 2추가가아니라 복사인게 명확(?) //걍 etemp아이디의 아이디값을 추가했음 좋지않을까< 위치변경이아닐듯
                item = Etemp;
                count = 1;
                //만약 받은 아이템이 Weapon이고 TwoHanded가 참일경우 방팰르 뱉도록해야한다
            }



            GameManager.inst.GetComponent<Inventory>().ItemDraw(this.transform);
//            GameManager.inst.GetComponent<Inventory>().ItemDraw(this.transform);//해당위치를 다시그림

        }

    }
    //아이템은 쿨타임이 슬롯에서 담당함 따라서 카운트 복제시 슬롯도 같이 복제될것
    public float ShowCoolTime()
    {
        if (item.delay + coolTime > Time.time)//만약 스킬을 마지막쏜시간과 + 쿨타임이 더크면
        {
            return (item.delay + coolTime - Time.time) / item.delay;
        }
        return 0;
    }
}
