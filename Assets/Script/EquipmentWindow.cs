using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//인벤토리와 대동소이해서 그냥 상속관계화 시킬까 싶지만 귀찮음 귀찮음보다는 어려울듯

public class EquipmentWindow : MonoBehaviour {

  
//    public Player player;//중요도 낮음? << 걍 게임오브젝트에서 하도록함 의존성을 높이는기분
//    public EquipItem[] equips = new EquipItem[6];
    //ㄴ 잘하면 안써도된다 써야할듯?
    //ㄴ장비낄때 id비교나 그럴때? <장비슬롯에넣으면안해도됨
    public EquipSlot[] equipSlot = new EquipSlot[6];
    //이런 간단한것들은 샘플 클래스 비슷한거 만들고 거기에 상속박는게 좋을꺼같은데
    private bool activeScreen = false;
    public GameObject equipscreen;

    public void OnOffScreen()
    {
        if (activeScreen == false)
        {
            equipscreen.SetActive(true);
            activeScreen = true;
        }
        else if (activeScreen == true)
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
                equipscreen.SetActive(false);
            activeScreen = false;
        }
        //원래 업뎃에 있었는데 난이렇게 만들었으니 여기에 넣어봄
    }

    //불필요한걸 정리하니 결국 장비슬롯 모음집이 됐음
}
