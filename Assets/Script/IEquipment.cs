using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//슛,어택같은건 액션이란이름으로 개명이될거고
//그 액션은 자식일 EquipItem이 관여한다 << 걍 겟컴포넌트로 무기를 꺼내면 되지않을까?
//ㄴ그렇게되면 슬롯도 좀 손봐야할테니깐 차라리 가상화로 일 안하는애 만드는게 낫다고봄

public interface IEquipment {
//   void Shoot();//가상 클래스처럼 두고싶은데 그러면 안이쁠듯
                                 //    { }//공격을 따로 한다면 이클래스 자체가 추상이될수있음
                                 //    public Attack() <<여기서 활동을함, 만약 ranged웨폰값이면 그냥슛호출하는식으로
  void Equip(Player p);
  void UnEquip(Player p);
  //void Attack();//플레이어가 눌렀을때, 그게 근거리면 어택을, 아니라면 슛을 함

    /// /////////
}
