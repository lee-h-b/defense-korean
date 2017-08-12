using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Iagro
{
    //본래 타겟시스템의 기본은 따로 분리되있어음
    //그런데 시야각을 하나로 통합하려하니 이것이 은근히문제가됨
    //따라서 이걸하나로 합침으로써 편리성을꽤함
    void ChangeTarget(GameObject target);
	// Use this for initialization

}
