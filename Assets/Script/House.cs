using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class House : Status {
    private float repair_Per;
    public Transform hpBlock;
    //이hp블록들의 포지션을 가지고 장난칠생각
    //ㄴ물록 내체력백분율로
    float hpPerz;
    private int repair_Val;
    public int Repair_Val
    {
        get
        { return repair_Val;
        }
        set
        {
            repair_Val = value;
        }
     }
    public float Repair_Per//더블은 f안붙이던걸로암 붙일려고 이거함
    {
        get
        {
            return repair_Per;
        }
        set
        {
            repair_Per = value;
        }
    }

    private void Start()
    {
        base.Start();
        hpPerz = hpBlock.transform.localScale.z;
    }
    protected override void Update()
    {
        base.Update();
        //z스케일만 건드려봄
        hpBlock.transform.localScale = new Vector3(hpBlock.transform.localScale.x, hpBlock.transform.localScale.y,  hpPerz*( (float)realStatus.hp / realStatus.max_Hp));

        hpBlock.transform.localPosition = new Vector3(hpBlock.transform.localPosition.x, hpBlock.transform.localPosition.y, 0 +( 1- (float)realStatus.hp / realStatus.max_Hp)*0.4f );
    }
}
