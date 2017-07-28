using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipviewer : MonoBehaviour
{
    public Transform projectile;
    public Player player;
    public static Equipviewer inst;

    public void ActiveEquip(ITEMTYPE type = ITEMTYPE.WEAPON)
    {
        if (type == ITEMTYPE.RING)
            return;
        //기존방식 (아이템형에따라 복제)는 빼고 비활성화상태로 있다가 장착시 부활시킴
        //그리는건 링을뺀 모든게 될거임 하지만링은 그릴 이유가없으니 6개정도면됨
        
        this.transform.GetChild((int)type-1).gameObject.SetActive(true);

        if (type == ITEMTYPE.RANGEDWEAPON)//원거리무기와 근거리무기는 동시에 그려져선 안되니깐 논액티브 시킴
        {
            GameManager.inst.player.ranged = true;
            //무기가 원거리가 됨을 표현
            NonActive(ITEMTYPE.WEAPON);
        }
        

    }
    public void NonActive(ITEMTYPE type = ITEMTYPE.WEAPON)
    {
        //만약이게 무기면서 원거리무기가 미활성 혹은 타입이 반지
        if ((type == ITEMTYPE.WEAPON && this.transform.Find("RangedWeapon").gameObject.activeSelf == false)//이구분 노쓸모?
            || type == ITEMTYPE.RING)
        {
            return;
        }
        this.transform.GetChild((int)type - 1).gameObject.SetActive(false);

        if(this.transform.Find("Weapon").gameObject.activeSelf == false && this.transform.Find("RangedWeapon").gameObject.activeSelf == false)
        {
            GameManager.inst.player.ranged = false;
            //무기가 근거리가 됨을 표현

            ActiveEquip(ITEMTYPE.WEAPON);
        }
        //만약 무기가 2개다 장비해제 됐을경우 막대기 하나 보여야함 < 우선은 무기끼리 교환시 비활성화부터
    }
    void Start()
    {
            if (inst == null)
                inst = this;
            //         this.gameObject.transform;
            player = this.transform.parent.GetComponent<Player>();
        ActiveEquip();
            //기본장비를 인벤에 두고하는식이 된거같은데(아님말고)
            //그렇다면 무기생성을다르게해야할듯
            //시작장비가 생긴다면 넣게될것		 //애니메이션은 시작장비가 해야될까?
        }
    
}
