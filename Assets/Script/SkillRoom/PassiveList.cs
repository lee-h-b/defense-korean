using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 스킬습득시 자동으로 프리팹을 만들어서 패시브 아래에붙이는식이 아니라 패시브가 직접 아이디, 스크립트로 구분,관리하게 할꺼같음 << 문제 스크립트의 수치는 어떻게 가져옴?
/// 
/// 현재문제. 공격형의 경우 확률에따라 그 Enemy를 매개로해서 보내서 공격을 시킬꺼임
/// 그럼 메소드로 2개가 필요하고 내부밸류도 1개는 필요할꺼임
/// 가장유력한건 패시브형 스크립트를 하나만들고 아래의 스킬오브젝트들이 패시브스킬 스크립트가되는거
/// 비전투형 스킬은 버프니깐 배우자마자 적용ㅇㅇ
/// </summary>
public class PassiveList : MonoBehaviour
{
    public static PassiveList inst;
//    public List<Dictionary<int,PassiveSkill>> test = new List<Dictionary<int,PassiveSkill>>();
    public Dictionary<int, PassiveSkill> list = new Dictionary<int, PassiveSkill>();//<< 초기화 꼭꼭해줘야함 안해주면 오류터짐
    
    private void Start()
    {
        if (inst == null)
            inst = this;//스킬 인스턴스를 통해 GetComponene~~이런거 안하고 조금이라도 간편하게
        //호출하려고 씀 < 즉 익스플로젼때문에 만들었다고 봐도 무관
    }
    public void AddList(int ID, PassiveSkill obj)
    {
        list.Add(ID, obj);
//        list[ID].GetComponent<PassiveSkill>().Init();//밸류가 패시브스킬인데 겟컴포넌트 써야하나
        

    }

    //val1,2는 있어야겠는데 3부턴 모르겠음
    public void PassiveUpgrade(int ID, double val1 = 0, double val2 = 0, double val3 =0)
    {
        //아이디를찾아서 해당 obj에게 bonusUp이던 Activity던 부름
        PassiveSkill temp;
        if (list.TryGetValue(ID, out temp))
        {
            temp.bonusUp(val1, val2);
        }

    }
    public PassiveSkill GetSkill(int ID)
    {
        PassiveSkill temp;
        if (list.TryGetValue(ID, out temp))
        {
            return temp;            
        }
        return null;
    }

    //해당 ID패시브를 활성화시킴정도
    public void InitSwitch(int ID)
    {

        if (list[ID] != null)
        {
            list[ID].gameObject.SetActive(true);
            list[ID].Init();
        }
    }
    //실험결과 id번째가아님 0번재부터더라
    public bool LoopPassiveChance(Status requester,Status target = null)//타겟을상대 or 기준으로 패시브를함
    {
        foreach(KeyValuePair<int, PassiveSkill> item in list)
        {

            //만약 해당 아이템이 거짓(안배움)일경우 그냥넘어감
            if (list[item.Key].gameObject.activeSelf == false) continue;
            //패시브는 1개만 터지도록? 걍다터지는건? 1개만터지면될듯
            if (item.Value.CheckPassivePer(requester,target) == true)
                return true;
        }
        return false;
    }
    //패시브스킬은 크게 2가지 공격형(일정확률 데미지정도), 버프형(능력치증감)있겠음
    //각각1개씩 만들생각
    //버프형의경우 버프디버프 오브젝트를 활용해서 만드는게 가장유력해보임
    //버프형의 문제는 지속시간의 존재임 따라서 그냥 패시브는 직접적으로 플레이어의 능력치에 감산을함

    //공격형의경우는 내가 공격을 했을때마다 얘가 발동을 해서 확률놀음을하고 그확률에 따라 데미지를 가하는 형태
    //ㄴ 그럼 이 공격을가할때를 언제함 ? 내가 공격할때라면 이 공격할때 반복문을 돌리던지 할텐데 그걸로 판단함? <또한 이펙티브를 상대에게 줘서 상대가 패시브 터진걸 알려야할텐데 그것도 문제라면문제
    //아니면 내주변에 적들을 일정확률로 데미지를줘버리는건? < 보통이라면 공격을 가할때 데미지를 주겠음 적과 ㅁ접촉을했을때 내패시브 아래애들을봐서 공격형이 있다면 걔한테 주사위(?)를 굴리게하고
    //일정확률데미지
    //
    //데미지의 경우면 증감값으로함
    //Etc가 해당데미지를 가할 확률이 될거임 << 1 = 100%로
/*    public float percent;

    public override void Activity()
    {//패시브의 발동문 < 주로 자기자신형태 << 버프식

    }
    protected virtual void Damage(Status target)//상대에게 패시브발동 즉공격형
    {

    }
    void Start () {
		
	}
	
	void Update () {
		
	}
    */
}
