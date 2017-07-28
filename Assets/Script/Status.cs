using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct StatusValue
{//스테이터스 밸류들의 값들만 필요한경우가 있음
    public int max_Hp;//프로퍼티로 초기화
    public int hp;
    public int max_Mp;//동문
    public int mp;

    public int minAtk;
    public int maxAtk;
    public int def;
    public float speed;
    public float atkDelay;//쓰긴함
    public float criPer;
    public float criDmg;
    public float dodgePer;

    public static StatusValue operator+ (StatusValue status1, StatusValue status2)
    {
        StatusValue status = new StatusValue();
        status.max_Hp = status1.max_Hp + status2.max_Hp;
        status.hp = status1.hp + status2.hp;
        status.max_Mp = status1.max_Mp + status2.max_Mp;
        status.mp = status1.mp + status2.mp;

        status.minAtk = status1.minAtk + status2.minAtk;
        status.maxAtk = status1.maxAtk + status2.maxAtk;
        status.def = status1.def + status2.def;
        status.speed = status1.speed * (status2.speed + 1);
        status.atkDelay = status1.atkDelay + status2.atkDelay;
        status.criPer = status1.criPer + status2.criPer;
        status.criDmg = status1.criDmg + status2.criDmg;
        status.dodgePer = status1.dodgePer + status2.dodgePer;

        return status;
    }
}
public class Status : MonoBehaviour
{

    public StatusValue realStatus;//실제 상태값이될거임
    //능력치 밸류값이 3개나되는데
    //캐릭터의 실제능력치를가르킬 realStatus와 캐릭터의 기본능력치가될 charaStatus
    //또한 거이기본(?)으로 돌아갈 가상 스테이터스 ,
    //캐릭터에 준영구, 즉 아이템,스킬을 통한 변동이 없을것을넣고
    //가상스테이터스는 디버프,아이템등등을 전부받은값이될거임 realStatus는 이2개를합친값으로
    //캐릭터간의 배틀에서 실제로 사용이될거임 //2개로 줄이고싶었는데 줄이는게 불가능
    //문제 업데이트로 한다면 realStatus의 능력치가 항상갱신이되서 데미지를 받으면 어떻게되겠음?
    //데미지를받으면 그 결과를 virtual이 가지고 있도록함
    [SerializeField]
    protected StatusValue charaStatus;//가상만외부에서 조리돌림당하라고 이렇게 함해봤음
    //real은 상시로 최대치를 갱신해야하니 컨스트를못함

    public StatusValue virtualStatus = new StatusValue();//가상 스텟 모든 스탯은 얘를 통해 증감을받고
                                                         //실제계산까지 하게되게 할것임 확률이던 뭐던 전부 더해지게될것


    public float nextattackTime = 0.0f;

    protected uint lv;
    public event System.Action Death;//죽음을 판단할 함수 포인터

    public StatusValue GetVirtualStatus() {
        StatusValue temp = new StatusValue();//모노디벨롭은 복제가 안된다라고 워링뜨는데
        //그럼 어떤복제를해야할지 모르겠음~_
        //ㄴ 걍 밸류만 따로두고 그걸 카피하기로했음 잘될것?
        //실제값을 카피해감, 그래야 제대로된 능력치수정을가할수가 있겠음
        //ㄴ ㄴㄴ 디버프,버프를 위해서인거같은데 버프를 가할땐 그냥 VIRTUAL값을 수정시킴됨
        //따라서 이거의 중요성이 낮아진거같음 그냥 해당값을 virtual에 서 감소시키면 되는걸테니
        //%의 경우는? 그럼일당 per,speed 등만 챙겨봄
        /*
        temp.minAtk = this.realStatus.minAtk;
        temp.maxAtk = this.realStatus.maxAtk;
        temp.def = this.charaStatus.def;
        */
        temp.speed = this.charaStatus.speed;
        temp.atkDelay = this.charaStatus.atkDelay;
        temp.criPer = this.charaStatus.criPer;
        temp.criDmg = this.charaStatus.criDmg;
        temp.dodgePer = this.charaStatus.dodgePer;
        return temp;
    }//이건 상태이상걸기위해, 백업용으로 보내는것

    //회피는 공격전에 판별하고 크리는 데미지 계산때 판별
    public int DamageCalc(int def, GameObject defender)
    {
        int damage = Random.Range(realStatus.minAtk, realStatus.maxAtk);
        damage -= def;
        float per = Random.Range(0, 100);
        if (per < realStatus.criPer)//크리떴으면
        {
            damage = Mathf.RoundToInt(damage * realStatus.criDmg);
            if (Mathf.Max(damage, 0) != 0)
                GameManager.inst.CreateText(defender, "크리티컬! : " + damage.ToString(), new Color(0, 255, 255));
        }
        else//크리안떴으면
        {
            if (Mathf.Max(damage, 0) != 0)//데미지가 1이상
                GameManager.inst.CreateText(defender, damage.ToString() + "!", new Color(0, 255, 255));
            else//데미지가 0
                GameManager.inst.CreateText(defender, "막아냄!", Color.blue);
        }
        return Mathf.Max(damage, 0);
    }
    public void TakeDamage(int damage)
    {
        virtualStatus.hp -= damage;
        realStatus.hp -= damage;//짜피 데미지 받을껀데 미리 받음 <이걸 다시호출해야 0이라고 죽게되는데 그걸방지하기위함이 주목적
        if (realStatus.hp <= 0 )
            Die();
    }
    public void CheckOverHpMp()
    {
        realStatus = charaStatus + virtualStatus;//능력치 재조정정도
        if (realStatus.hp > realStatus.max_Hp)
        {
            int temp = realStatus.hp - realStatus.max_Hp;
            virtualStatus.hp -= temp;
        }
        if (realStatus.mp > realStatus.max_Mp)
        {
            int temp = realStatus.mp - realStatus.max_Mp;
            virtualStatus.mp -= temp;
        }
    }
    protected virtual void Die()
    {
        if (Death != null)
        {
            Death();
        }
        Destroy(gameObject);
    }
    //statuc에 뭔가 했어야 한거같은데
    public void Attack(Status defender)
    {
        float per = Random.Range(1f, 100f);
        if (defender.realStatus.dodgePer > per)
        {
            //회피띄우는 무언가
            GameManager.inst.CreateText(this.gameObject, "빗나감!", Color.green);
            return;
        }
        if(defender.GetComponent<Iagro>() != null)//어그로가 있는대상의 어그로를 가져옴
        defender.GetComponent<Iagro>().ChangeTarget(this.gameObject);
        defender.TakeDamage(DamageCalc(defender.realStatus.def, defender.gameObject));

    }
    //우선 데미지 공식을 산출해서 가져오도록

    //마법공격 전용 공식
    //마법은 크리를 안받고, 회피도안받음굳ㅎ
    public void MagicAttack(Status defender, int TakeDamage)
    {
        int quickCalc = Mathf.Max(TakeDamage - defender.realStatus.def, 1);
        GameManager.inst.CreateText(defender.gameObject, quickCalc.ToString() + "!", Color.yellow);
        if (defender.GetComponent<Iagro>() != null)
            defender.GetComponent<Iagro>().ChangeTarget(this.gameObject);
        defender.TakeDamage(quickCalc);
    }
    // Update is called once per frame
    // Use this for initialization
    protected void Start()
    {
        //외부에서 쓰게? <안해도될거같은데
        charaStatus.hp = charaStatus.max_Hp;
        charaStatus.mp = charaStatus.max_Mp;
    }

    //버그가 넘칠거로 예상됨
    //구상해보니 능력치관련해서 에로사항이 꽃필거라예상되서 status를받고 거기에 값을 변동시키는걸로함
    public void StatusBuff(int b_atk = 0, int b_def = 0,float b_speed = 0f, float b_criper = 0f, float b_cridmg = 0f, float b_dodge = 0f)
        //여기서 값을params로 받을지 걍 배열식으로 하드코딩하게할지 생각해봤는데
    //배열식으로 하드코딩하게 하는게 나을듯 params는무한한값인건 둘째ㅑ쳐도 하면 스위칭하고 그래야할거같은데 보기 복잡함
    {
        //지금 새로한식(가상 + 캐릭) = 실제 이렇게할경우 이거의 %증감은 사실상 무효가될것
        this.virtualStatus.minAtk += b_atk;
        this.virtualStatus.maxAtk += b_atk;
        this.virtualStatus.def += b_def;
        //여기까진 잘될듯 문제는 아래
        //계산답은 곱셈식인데 이 곱셈식으로 어떻게 가해야할까? 계산하고 나온결과값을 뱉어주면 좋겠지만

//        this.virtualStatus.speed = realStatus.speed * (100 + b_speed)/100;//해당계산결과와 리얼스피드 자체의 차이만큼을 계산하도록?
        //현재 100 + 0 / 100 = 1 거기에 총합 스피드를 *하게되니깐 오류임
        //finalValue = currentValue / maxValue * 100;
        //그럼 이거 어짜피 전부 퍼센트니깐 단순 계산식으로변경
        //스피드는 가상스피드는 기본1 이고 거기에 *를 행하는식 << 리얼 = 캐릭스피드 * 1+ 가상스탯.speed 
        this.virtualStatus.speed += b_speed;
        this.virtualStatus.criPer += b_criper;
        this.virtualStatus.criDmg += b_cridmg;

//        GetComponent<UnityEngine.AI.NavMeshAgent>().speed = charaStatus.speed+ virtualStatus.speed;

    }


    protected virtual void Update () {//왜프로텍티드냐면 혹시 안될지도 몰라서임
        realStatus = charaStatus + virtualStatus;
	}
}
