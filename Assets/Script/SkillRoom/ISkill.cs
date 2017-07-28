using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//기본적인 스킬의 공통분모들 모든 스킬오브젝트의 자식들은 이 3가지가 필수임
//스킬오브젝트에는 필요가 없는것 같지만 패시브스킬은 오브젝트가 없으므로 이것들을 직관적으로 상속받게될텐데
//그래서 이렇게 만들었음, 겸사겸사 상속도 시켜주고
public interface ISkill {

    void Init();
    void bonusUp(double size, double etc);
    void ResetObject(SKILLTYPE t);

}
