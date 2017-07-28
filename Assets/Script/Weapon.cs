using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/* Weapon에 양손전용인지 아닌지를 넣기위해 이렇게 확장함
 * 원딜이문제
 */
public class Weapon : EquipItem {

    // Use this for initialization
    public bool TwoHand = false;
    bool Ranged = false;
	void Start () {
		
	}
    //왜 무기생성시 무조건 WEAPON이냐면 아이템 장비 만들때 원거리무기를 같이두게 하기위함임
    //따라서 RANGED는 얘원거리입니다,아닙니다를 판단하는 장치
    public Weapon(int _ID = 0, string _name = "none", ITEMTYPE _type = ITEMTYPE.WEAPON,bool _TwoHand = false,
int _price = 0, int _hp = 0, int _mp = 0, int _atk = 0, int _def = 0,
float _speed = 0.0f, float _range = 0.0f, float _delay = 0.0f,
float _cri = 0.0f, float _dodge = 0.0f)
    : base(_ID, _name, ITEMTYPE.WEAPON, _price, _hp, _mp, _atk, _def, _speed, _range,_delay, _cri, _dodge)
    {
        if (_type == ITEMTYPE.RANGEDWEAPON)
        {
            Ranged = true;
        }
        TwoHand = _TwoHand;
        //        delay = _delay;
    }
    public override ITEMTYPE GetItemType(bool detail)
    {
        if (Ranged == true && detail == true)
            return ITEMTYPE.RANGEDWEAPON;
        else
        return this.type;//무기니깐 곱게 리턴이 될거임
    }


}
