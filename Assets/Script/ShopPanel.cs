using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//개명가능
//커서들이대면 infoBOX가 활성화되는식 <<인벤슬롯처럼
public class ShopPanel : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public ITEM item;
    //    ItemDB data;
    //플레이어는 Shop이 가지고있도록함
    Image itemImage;
    public Text itemName;
    Text price;//설명 db에서 따오고 ID와 같은 설명을 얻어오는구조임 
    Text type;
    //3개전부다 아이템에서 따오도록함

    public uint count = 1;//<번거롭겠지만 uint를 함으로써 -악용을 미리 방지해봄
    //무기의경우는 1임

    public InputField InputText;//만약 소모품일 경우 보이게되고 소모품이 아니면 안보임

    private int pay;//실제값
    //ㄴ아이템이 세일 될수도 있으니깐

    public void ChangeCount(Text text)
    {
        uint init = 0;
        //        print(text.text);
        if (uint.TryParse(text.text, out init) == false)
        {
            count = 0;
            return;
        }
        //z        text.text = InputText.text;
        count = uint.Parse(text.text);
    }
    public void Buy()
    {
        if (count == 0)
        {
            GameManager.inst.CreateText(Shop.inst.guest.gameObject, "수량을 입력해주샤요!", Color.black);
            return;
        }
        if (count * pay > Shop.inst.guest.gold)
        {
            GameManager.inst.CreateText(Shop.inst.guest.gameObject, "자금이 부족합니다!", Color.black);
            return;
        }
        Shop.inst.guest.gold -= (uint)(count * pay);
        //수량자체는 외부에서 건들여지므로 여기(구입버튼)에선 그냥함
        GameManager.inst.GetComponent<Inventory>().AddItem(item.ID, (int)count);//추가
    }
    public void Write()
    {
        if (item != null)
        {
            itemName.text += item.itemName;
            type.text += ItemDB.instance.TYPENAME[(int)item.GetItemType()];
            price.text += pay + "골드";

        }
        //여기서 아이템이름, 종류, 가격,간단설명 등을 하게될거임 << 그냥 이미지에 커서대면 옵션나오게?
    }
    public void ItemDraw()
    {
        if (item.image != null)
            itemImage.sprite = item.image;
        else
            itemImage = null;
    }
    public void SetItem(int ID)//아이템을 셋하는데 실패하면 이오브젝트를 파괴
    {
        if (ItemDB.instance.items.ContainsKey(ID))
        {
            item = ItemDB.instance.items[ID];
            
            float temp = item.price * (1f - Shop.inst.guest.GetComponent<Player>().goldRate);//이걸 pay로 바로넘기면 계산에 오차가 있는지 -1시킴
            //그래서 이렇게 결과를 인트로 변환해서 받도록함
            pay = (int)temp;

            Write();
            ItemDraw();
            if (item.GetItemType() != ITEMTYPE.CONSUMPTION)
            {
                count = 1;
                //소모품일경우만 인풋박스던가 활성화
                InputText.gameObject.SetActive(false);
            }
            else
                count = 0;
            //            return true;

        }
        else
            Destroy(gameObject);

        //아이템 세트

    }
    public void OnPointerExit(PointerEventData data)
    {
        if (InfoBox.instance.Info.activeSelf == true)
        {
            InfoBox.instance.Reset();
            InfoBox.instance.Info.SetActive(false);
        }
    }
    public void OnPointerEnter(PointerEventData data)
    {
        //이미지에 닿을경우 액션
        if (data.pointerEnter.gameObject.name == itemImage.name)
        {
            InfoBox.instance.Info.SetActive(true);
            InfoBox.instance.ShowInfo(item);

        }
    }
    void Awake()
    {
        //시작하면 이미지,이름,설명,타입등을 전부 대입해줌<< 맨날할꺼니 꼭해야함
        //성능을 위해서면 public으로 할꺼같음
        itemImage = this.transform.Find("ItemImage").GetComponent<Image>();
        itemName = this.transform.Find("Name").GetComponent<Text>();
        price = this.transform.Find("Text").GetComponent<Text>();
        type = this.transform.Find("Type").GetComponent<Text>();
        InputText = this.transform.Find("InputField").GetComponent<InputField>();
        //        SetItem(101);
        //대입은빠르게, 왜냐면 start시점에서 하면 Shop에서 패널만들때 늦음
    }


    // Update is called once per frame
    void Update()
    {

    }
}
