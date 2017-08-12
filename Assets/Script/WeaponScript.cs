using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
//스피어콜라이더 필수


public class WeaponScript : MonoBehaviour {


    //angle도 넣을까?
    //아래의 public은 임시
    public Status enemy, me;
    float lastAttackTime;
/*    public void Shoot()
    {
        if (projectile == null)
            return;
        else
        {
            //투사체가 잇을경우 하는 무언가 << 통상 투사체가 있는건 무기가 있다는가정
        }
    }
    */
    // Use this for initialization
    void Start () {
                  me=   gameObject.transform.parent.transform.parent.GetComponent<Player>();//걍파인드로?
//        me = GameManager.inst.player.GetComponent<Status>();//원래 안바꾸려다가 발사체와의 형편성 때문에 바꿈      
//ㄴ못바꿈 시작순서를 바꾸던가 게임매니저 awake에서 플레이어를 할당해야함
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //내타입이 weapon일 경우에만 작동하도록 함
    private void OnTriggerEnter(Collider other)
        //웨폰에 무기를 함부로 쓰지 않도록 브레이크를 만들어야겠음
    {//칼 돌아가는건 플레이어가 하지만 실질 공격은 무기 자체가함
        //문제있음 적의 시야각이 트리거 엔터하므 그렇다면 충돌체끼리의 접촉이니 OnCollision?
        //no 레이캐스트를 쓰거나 오버랩했다면 발동하는걸로함
        if(other.tag == "Enemy" && lastAttackTime + me.realStatus.atkDelay < Time.time)
        {

            enemy = other.GetComponent<Enemy>();
            //한번 공격을 했으면 me의 공격딜레이의 1/2정도시간까진 공격못하게

            //여기서 충돌했으면 caster의 패시브를 찾아가서 걔한테중 현재 온인애들을 전부부르는게 좋겠음
            //딕셔너리는 뭐랄까 순서가 1000부터 시작할꺼같음 << List <Dicti로?
            //ㄴ기우임 쓸모x

            //패시브가 되면 공격대신 패시브가 발동되는구조임
            if (PassiveList.inst.LoopPassiveChance(me, enemy) == false)
            {
                me.Attack(enemy);
            }
            lastAttackTime += me.realStatus.atkDelay;

        }
    }//적이 내사정거리안에 들어오면 회전하는것은 player가 함
}
