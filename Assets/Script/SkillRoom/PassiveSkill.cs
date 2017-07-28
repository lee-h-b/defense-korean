using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//처음값은 prefab에 전적으로 의지하게될것
public class PassiveSkill : MonoBehaviour, ISkill
{
    public Player caster;
    public virtual void Init()
    {//스킬의초기화
    }

    public virtual void PassiveActive(Status caster, Status target = null)
    {//패시브전용 액티브, 본래 필요는 없을지 모르지만 발사체의경우 맞추더라도 이게 어디서왔는지, 즉 플레이어를 쉽게 못불러냄
        //ㄴ그냥 게임매니저 인스턴스 쓰면되지 않음? < 불러낸다 한들 플레이어의 타겟이 바뀌면 도루묵임 그리고 이게더 전문가같음 인스턴스남발은 좀그렇기도함


    }
    public virtual void bonusUp(double val1 = 0, double val2 = 0)
    {//스킬 업그레이드

    }
    public virtual void ResetObject(SKILLTYPE t)
    {
    }
    public virtual float[] GetAllValues()
    {
        return null;
    }
    public virtual bool CheckPassivePer(Status resquester, Status target = null)//타겟이없을경우도 있지
    {
        return false;
    }

}
