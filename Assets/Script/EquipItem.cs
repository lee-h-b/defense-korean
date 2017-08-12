using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItem : ITEM,IEquipment {
    //상속관계인건 둘째쳐도, 복제하는게 필요한가?
    //몇몇 기능은 아예 여기서 생성하는게 맞다고 생각

    public int atk;
    public int def;
    public float speed;//스피드? 안넣어도될꺼같음
    public float range;//공격범위
    //그러면 이 공격시간으로 애니메이션을 행해야하는데 < Lerp로 해결?
    public float criPer;
    public float dodgePer;

    public EquipItem(int _ID = 0, string _name = "none", ITEMTYPE _type = ITEMTYPE.WEAPON,
    int _price = 0, int _hp = 0, int _mp = 0, int _atk = 0, int _def = 0,
    float _speed = 0.0f, float _range = 0.0f, float _delay = 0.0f,
    float _cri = 0.0f, float _dodge = 0.0f)
    : base(_ID, _name, _type, _price, _hp, _mp, _delay,Resources.Load<Sprite>("Item/" + _ID))
    {
        atk = _atk;
        def = _def;
        criPer = _cri;
        dodgePer = _dodge;
        speed = _speed;
        range = _range;
        delay = _delay;
    }

    public void Equip(Player p)
    {
        equipBonus bonus = new equipBonus(atk,speed,def,hp,mp,criPer,dodgePer,delay,range);
        p.ApplyItem(bonus);
    }
    public void UnEquip(Player p)
    {
        equipBonus bonus = new equipBonus(atk, speed, def, hp, mp, criPer, dodgePer, delay,range);
        bonus.reverse();

        p.ApplyItem(bonus);
    }
}
