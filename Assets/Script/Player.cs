using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//외부로 꺼내야지 다른 cs에서 사용

    //TODO::아이템의 버프를 통한값따로,버프를 통한값ㅎ따로 해서 표현해야할거같다

[System.Serializable]
//그냥 아이템에 있는것주 일부만 적용하도록 바꾸면 간결해질걸?
public struct equipBonus
{
    public int atk;
    public float speed;
    public int def;
    public int hp;
    public int mp;
//    public float range;
    public float atkDelay;
    public float criPer;

    public float dodgePer;
    public float range;//공격범위 무기 에서 따옴

    public equipBonus(int _atk = 0, float _speed = 0, int _def = 0, int _hp = 0, int _mp = 0,float _criPer = 0,float _dodgePer = 0, float _atkDelay = 0, float _range = 0)
    {
        atk = _atk;
        
        speed = _speed;
        def = _def;
        hp = _hp;
        mp = _mp;
//        range = _range;
        atkDelay = _atkDelay;//사실상폐기
        criPer = _criPer;
        dodgePer = _dodgePer;
        range = _range;
    }
    public void reverse()
    {
        equipBonus temp = this;
        atk = -temp.atk;
        speed = -temp.speed;
        def = -temp.def;
        hp = -temp.hp;
        mp = -temp.mp;
        atkDelay = -temp.atkDelay;
        criPer = -temp.criPer;
        dodgePer = -temp.dodgePer;
        range = -range;
//        return temp;
    }
}

[RequireComponent(typeof(PlayerController))]
//또한 여기에 무기 컨트롤러도 만들생각<<자식클래스에서 찾아감
public class Player : Status
{
    public uint gold;//임시 로 퍼블릭, 프라이베잇해야할거
    public Text goldText;//현재골드를 써줄거
    //내자식인 웨폰핸들을 가지고 있도록하고,
    //거기 무기의 tag로 칼인지 뭔지 알도록 함
    //알필요가 있을까 그냥 돌리면 되지않나? //애니메이터의 컨트롤러는 누가? 애니메이터가 하면되지
    // Use this for initialization
    public Slider HpSlider, MpSlider;

    public Transform range ;//공격범위 무기 에서 따옴<< 무기의 실제 길이쯤
    //ㄴ 실제 공격할범위가 되야하고, 자연스러움?을 위해 무기의 트랜스폼으로 했음
    //ㄴ하지만 원거리의 경우는 반지름만 늘어나면 되는데?
    //ㄴ c++처럼 pair가 있음 그걸쓰고싶은데 없으니 하나더만들어야..
    public SphereCollider archerRange;

    public float goldRate = 0;//골드비율쯤 소숫점단위로 움직이겠고 높을수록 소비할때싸지고 골드드랍도잘됨

    public bool ranged;//공격타입이 원거리인지 아닌지를 나타낼꺼


    public void ApplyItem(equipBonus item)
    {
        virtualStatus.minAtk += (int)item.atk;
        virtualStatus.maxAtk += (int)item.atk;
        virtualStatus.def += (int)item.def;
        virtualStatus.max_Hp += (int)item.hp;       
        virtualStatus.hp += item.hp;
        virtualStatus.max_Mp += (int)item.mp;
        virtualStatus.mp += item.mp;
        virtualStatus.speed += item.speed/100f;
        virtualStatus.atkDelay += (float)item.atkDelay;
        virtualStatus.criPer += item.criPer;
        virtualStatus.dodgePer += item.dodgePer;
        range.localScale +=new Vector3( item.range/2f, 0);//사정거리는 무조건 최소한 0.5라는 의지임
        archerRange.radius += item.range / 2f;

        //이 range가 오르면 무기의 range에 영향을 끼쳐야한다

        //어택딜레이,크리,회피확률
        CheckOverHpMp();//나중에 Apply가상스탯 이런거 있을때나쓰게될거임

        if(virtualStatus.minAtk > virtualStatus.maxAtk)
        {
            int temp = virtualStatus.minAtk;
            virtualStatus.minAtk = virtualStatus.maxAtk;
            virtualStatus.maxAtk = virtualStatus.minAtk;
        }
    }
    /// <summary>
    /// //////////////////////////////////////
    /// </summary>
    void Start()
    {
        archerRange = transform.Find("EquipHandle").Find("RangedWeapon").GetComponent<SphereCollider>();
        //ㄴ깊게생각하지말고 잔머리굴림, range가 증가할때 애도 따라서증가함, 둘은 다른타입으로 활동하니 두개가 곂쳐서 버그가날일은 적을꺼라고봄
        //ㄴ 그럼 range를 상황에따라 바뀌는 변수로서 썼으면 좋지않을까?
        range = transform.Find("EquipHandle").Find("Weapon").transform;
        //.GetComponent<SphereCollider>();
        gold = 2500;
        goldText = GameObject.Find("GoldText").GetComponent<Text>();
        ranged = false;//맨처음 공격은 근거리임
        
        base.Start();

    }
    //업데이트를 픽스드로?
    // Update is called once per frame
    void Update()
    {
        base.Update();

        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = realStatus.speed;
        HpSlider.value = (float)realStatus.hp / realStatus.max_Hp;
        MpSlider.value = (float)realStatus.mp / realStatus.max_Mp;

        goldText.text = "소지금 : " + gold;
    }
}
