using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct ItemDrop
{
    public int ID;
    public float Per;

}
public class Enemy : Status, Iagro
{
    UnityEngine.AI.NavMeshAgent nav; 
    private GameObject defaultTarget;
    private GameObject currentTarget;
    private GameObject hpText;


    public GameObject GetTarget//프로퍼티
    {
        get
        {
            return currentTarget;
        }
    }

    //    private Vector3 target_Point;
    //    Transform Target;
    public bool aggression; //공격당하면 변함 <<eye에 넣을까 싶지만 여기가 좀더 다루기편함?
    public uint gold = 10;//임시

    public ItemDrop[] itemIDArr;//ㅁㄹ <<아이템 길이가될예정이였던거같음
    // Use this for initialization
    
    float sqrDstToTarget;
    float TargetCollision;
   public float myCollisionRadius;
//    public float attackTime = 1.0f;
    public DropItem item;//안될지도?
    public Projectile projectile;
    //적이죽었다는건 즉 아이템이 나온단소리
    //골드는 드랍이되고, 그드랍된걸 직접 주워먹도록함 << 하지만 아이템을 일일히 가서 먹긴 힘드니깐 그아이템이 자석처럼 끌려가도록
    protected override void Die()
    {
        float seed = Random.Range(0f, 100f);
        //드랍 아이템 표시안되고 테일즈위버마냥 바로 먹는방식
        for (int i = 0; i < itemIDArr.Length; i++)
        {
            if (seed < itemIDArr[i].Per )
            {
                GameObject.Find("ItemGetPanelList").GetComponent<ShowItemGet>().AddBars(ItemDB.instance.items[itemIDArr[i].ID].itemName);
                GameManager.inst.GetComponent<Inventory>().AddItem(itemIDArr[i].ID);
            }
        }
                    
        item.value = gold;//여기서 값을바꾸고 복제한다고해도 이미 복제된녀석들은 안바뀜
         //돈만떨구지말고 아이템도 떨구도록하자 아이템을 떨굴덴 구조체를 떨구게 될거같음 ID와 갯수를 가지라고

        Instantiate(item, transform.position, item.transform.rotation);
        base.Die();
    }


    public void Shoot()
    {
        if (Time.time > nextattackTime)
        {
            projectile.Shooter = this;
            projectile.SetAtk(realStatus.minAtk, realStatus.maxAtk);
            transform.Find("RangedWeapon").GetComponent<RangedWeapon>().Shoot(projectile);
            nextattackTime = Time.time + realStatus.atkDelay;
        }
    }

    public void ChangeTarget(GameObject target = null)//타겟을 바꿈  < 나->하우스인경우도처리담당
    {
        if (target == null)
            target = defaultTarget;
        currentTarget = target;
        if (target.tag == "House" || projectile != null)//타겟의 태그가 하우스거나 원거리공격이 가능
        {
            myCollisionRadius = transform.Find("Eye").GetComponent<SphereCollider>().radius;
        }
        else
            myCollisionRadius = GetComponent<CapsuleCollider>().radius;

        if (target.GetComponent<BoxCollider>() != null)
        {
            TargetCollision = target.GetComponent<BoxCollider>().size.z;
        }
        else
            TargetCollision = GetComponent<CapsuleCollider>().radius;
    }
    private void ShowHp()
    {
        //그저 체력을 그리는용도 << 프라이베이트씩이나..
        hpText.GetComponent<TextMesh>().text = realStatus.hp + " / " + realStatus.max_Hp;
    }
    //애니메이션 대응으로 돌아가기에,그리고 공격이 데미지를 항상 주지않도록 하기위해 
    IEnumerator Attack()
    {
        float per = 0f;//보간값
        Status enemy;
        bool attacking = false;
        var defaultRotation = transform.rotation;//var좀쓰고싶었음
        Quaternion attackRotation = transform.rotation * Quaternion.Euler(30, 0, 0);
        //안될려나
        transform.Rotate(Vector3.forward * realStatus.speed * 10, Space.World);
        while (per <= 1)
        {
            while (per >= 0.5f && attacking == false)
            {
                attacking = true;
                    enemy = currentTarget.GetComponent<Status>();
                Attack(enemy);
            }
//            attacking = false; << 2번하면 끝남
            per += Time.deltaTime * realStatus.speed;
            float interpolation = (-Mathf.Pow(per, 2) + per) * 4;
            transform.rotation = Quaternion.Lerp(defaultRotation, attackRotation, interpolation);
            yield return null;//이게 밖에있음 안됨 이뮬레이터의 신비

        }
    }


    /// <summary>
    /// 
    /// </summary>
    void Start()
    {
        base.Start();
        nav = GetComponent<UnityEngine.AI.NavMeshAgent>();
        defaultTarget = GameObject.FindGameObjectWithTag("House");
        currentTarget = defaultTarget;
//                target = target;

        ChangeTarget();
        //타겟 충돌체크기 판별 외부로 꺼내는게 좋을까?

        hpText = this.transform.Find("HpText").gameObject;
        //체력텍스트를 쓸거임 << 걍 텍스트만 가져오면 되지않을까?
    }
    
    // Update is called once per frame
    //캡슐콜라이더와 충돌하면 멈추는걸로
    void Update()
    {

        nav.speed = realStatus.speed;
        //fixedUpdate로?
        realStatus = charaStatus + virtualStatus;
        //hptext작성
        ShowHp();
                nav.SetDestination(currentTarget.transform.position);

        //항시해야하고,매번 달라지니 함수로 옮기지않음
        //두벡터간의 거리를 탐지
        sqrDstToTarget = (currentTarget.transform.position - transform.position).sqrMagnitude;
        //만약 탐지한거리가 내충돌체크기+상대 충돌체크기^2 보다 작을경우 즉 충돌체끼리 접촉이 가능한경우
        if (sqrDstToTarget < Mathf.Pow(myCollisionRadius + TargetCollision, 2))
        {//플레이어일경우 차이가 심하게남 ? << SphereCollider라인임
            nav.isStopped = true;
            transform.LookAt(currentTarget.transform.position);//타겟을 조금이라도 바라보도록
                                                               //그리고 여기서공격 공격은 몹따로 나따라 정의?

            //여기서 30도회전시킴
            if (nextattackTime < Time.time && realStatus.speed > 0)
            {//속도가 0이면 애초에 작동도안하도록함

                if (projectile != null)
                {
                    Shoot();
                }
                else StartCoroutine(Attack());
                nextattackTime = Time.time + realStatus.atkDelay;

            }

        }
        else nav.isStopped = false;
        //

        //만약 공격범위 안에 들어온다면 공격

    }

}
