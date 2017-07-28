using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sight : MonoBehaviour {
  //  PlayerController me;//me는 함수내에서 정의함 내부모가 플레이어일경우
    //아니면 적일경우, 것도아니면스킬일경우

    //에너미뷰의 인사이트는 필요없음 360도 전체를 볼꺼기에
    //반경은 넉넉히 2로했으나 나중에 무기사이즈에 따라 바뀌게할수도있겠음

    private bool inSight;
    public float ViewAngle = 360f;//다른곳에 넣을지도 적마다 다르게설정
   //    http://www.3dtutorials.org/video/207/unity3d-tutorial-stealth-game-tutorial-403-enemy-sight-and-hearing-unity-official-tutorials/
   private SphereCollider col;//시야 는 구형충돌체형태
    void Start () {
        //        me = transform.parent.GetComponent<PlayerController>();
        col = GetComponent<SphereCollider>();
	}

    // Update is called once per frame
    //other는 임시자료형, 스위치케이스가 길어지면 보기안좋을테니 여기로옮길거
    public void TargetCheck(Iagro me,GameObject other, string target)
    {
        //other가 들어온것, 그들어온것이 target인지 확인
        //충돌에서 스위치문은 제외하고, 여기서 이게 플레이어인지확인?
  
        if (other.tag == target && me != null)// me!=null 안될지도
        {
            inSight = false;//있어도조건에 충족해야 참이됨
            //이를위해 쓰는게 angle인데 이건 2점을 파악해서 그각도를반환함
            //적에서 플레이어에 이르는 벡터와 enemy의 전방벡터를 분석할것
            //이각도가절반보다 작을경우 플레이어가 적의 시야에 들어온것
            Vector3 direction = other.transform.position - transform.position;
            //거리는 플레이어위치 - 내위치 
            float angle = Vector3.Angle(direction, transform.forward);
            if (angle < ViewAngle * 0.5f) //위에 썼듯이 절반보다 작을경우
            {
                //시야안에있다해도 사이에 방해물이 있으면 안되겠지
                //이걸위해 적-플레이어를 이루는 Raycast를 사용한다 뭔가맞아도 그게플레이어인질 알아야함
                RaycastHit hit;
                //발에서 레이캐스트하면 바닥이맞게됨 그대신 레이캐스트를위한 출발위치를
                //따로(position + formup)을 하면대충 이컴포넌트 사용자키의 절반쯤의 크기를 갖게될것
                //키가2라면 Y축으로 1만큼 늘어난 위치가 되겠음 이방향은 앞서 각도측정을
                //햇던 방향벡터
                //이캐스트의 방향 벡터는 항상 정규화가됨(normalized)
                //                Debug.DrawLine(transform.position + new Vector3(0, 0.35f, 0), direction.normalized * 10, Color.blue);

                if (Physics.Raycast(transform.position + new Vector3(0, 0.35f, 0),
                    direction.normalized, out hit, col.radius))
                {
                    //뭔가맞았을때
                    if (hit.collider.tag == target)
                    {//여기서 충돌체가 보인다는 사실을 알게됨
                        inSight = true;
                        me.ChangeTarget(hit.transform.gameObject);
                    }
                }
            }
        }

    }
    //스킬유도개념 첫번째 인자는 필요없겠지만 안하면 한없이 길어질꺼같고해서
    public void SkillGuided(NonTargetSkill parent,GameObject other)
    {
        switch(parent.caster.tag)
        {
            case "Player":
                if (other.tag == "Enemy")
                    parent.target = other;
                break;
            case "Enemy":
                if (other.tag == "House" || other.tag == "Player")
                    parent.target = other;
                    break;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        //넉넉히 Enter로 해봄 Stay랑 별반차이 없다고봄 <<내가 치고있던 적이 죽었으면 바로 바꿔야지
        //에너미뷰와 동일한일을 하는데 합치는게 좋지않을까
        //얘의부모가 플레이어일경우는 이걸하고 적이면 저걸하고 이렇게
        if (this.enabled == true)
        {
            switch (transform.parent.tag)//부모태그별로 나눠서함 스킬,나,에너미 3개를 이걸로 통합할거임
            {//코드 최대한 짧게칠려고 자신의 부모의 컴포넌트와 other의 게임오브젝트를보냄
                case "Player": TargetCheck(transform.parent.GetComponent<PlayerController>(), other.gameObject,"Enemy"); break;
                case "Enemy":  TargetCheck(transform.parent.GetComponent<Enemy>(),other.gameObject, "Player"); break;//뷰의 부모태그가 Enemy?
                    
                case "Skill": SkillGuided(transform.parent.GetComponent<NonTargetSkill>(), other.gameObject); break;
            }
        }
    }

}
//적이 시야에 들어오면 쫓아왔었음
//이번에 여기서는 그걸이용해서 아군의 반경에 적이들어오면
//어택모드로변하고 동시에 적타겟을 따오도록할거임