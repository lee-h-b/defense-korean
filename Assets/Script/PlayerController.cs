using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;//단축키때문에 하는거, 안하고싶음
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class PlayerController : MonoBehaviour,Iagro
{

    [SerializeField]//어택모드 볼려고
    private bool attackmode;//공격모드? 아마 어택땅상태가 되면 하게될거임
    private float myCollisionRadius;
    private float TargetCollisionRadius;
    //    public float attackRange = 1.0f;//공격범위 무기 에서 따옴 플레이어가 쓸꺼임 이걸 씀으로써 활의 경우 
    //무기의 사정거리를 조작가능하고 결과적으로 원거리의 시야를 형성할것
    public LayerMask mask;
    public Skill callSkill;
    //게임오브젝트가아닌 스킬을넘긴다 오브젝트로 하니 귀찮복잡

    //TODO :: 버튼들 매니저로 넘기기
    public GameObject equipButton;
    public GameObject inventoryButton;
    public GameObject skillButton;

    bool checkAttack;
    
    private float angle = 360.0f;
    private Sight view;
    


    UnityEngine.AI.NavMeshAgent move_point;//이동할지점쯤

//    float lastAttackTime;
 //   float attackDelay; 스테이터스에 역활함

    Player player;
    public GameObject target;//없어도 될지도? 재정의 게임오브젝트가 아니라 포지션으로잡고, 그위치를 대입받는식으로 해야겠으ㅡ
                             //    Vector3 target;
                             //    private Vector3 dirToTarget;
public GameObject GetTarget()
    {
        return target;
    }
    public CursorChange cursor;

    //    public GameObject gameManager;


    //스킬슬롯에서 코루틴을불러들여서 하고싶었음 하지만 안됨
    //왜냐면 슬롯을 닫으면 ㅂ활성화가 되기때문,스킬슬롯에 있는 얘라는걸 감안할때
    //스킬오브젝트를 player로 전달해주고 <물론 데미지등등 설정다하고 보내야함
    //커서를바꾼상태에서 내가 왼쪽클릭을하면 발싸가됨 코루틴으로 할필요가 없어질거같음
    //그래도 그게 멋질터이니 함
    //멋진건 모르겠고 비효율적이라고 생각함 굳이 메소드로 되는데 왜?


    void Shoot()
    {
        if (Time.time > player.nextattackTime)
        {
            player.transform.Find("EquipHandle").Find("RangedWeapon").GetComponent<RangedWeapon>().Shoot(player.realStatus.minAtk, player.realStatus.maxAtk);
            player.nextattackTime = Time.time + player.realStatus.atkDelay;
        }
    }

    //업데이트에서 만약 스킬이 존재한다면 그스킬을 쓰라고 이 코루틴을 부르겠음
//    IEnumerator CallFromMouse()//마우스의 위치를 잡고, 그위치에 현재스킬타입에 따른 행동을 일으킴
    //ㄴ 타겟과 범위기 두개를 해결하기위해 만든거 따라서 소환도 여기서함
    void CallFromMouse()
    {
        //여기서 커서체인지
        GameManager.inst.GetComponent<CursorChange>().ChangeCursor(1);//커서바꾸고
//        while (true)
  //      {
            if (Input.GetMouseButton(0) && callSkill.ID != 0)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {

                    //여기에 피스스킬넣음 왜냐면 얘는 적에게 할 스킬이 아니기때문
                    if(callSkill.type == SKILLTYPE.PEACE)
                    {
                        var Comtemp = callSkill.prefab.GetComponent<PeaceSkill>();//안될지도

                        //첫째조건. 내스킬이 하우스전용인가 && 플레이어전용이고 바르게 찍었나를봄
                        if ((Comtemp.houseOnly == true && hit.collider.gameObject.tag == "House")
                            || (Comtemp.houseOnly == false && hit.collider.gameObject.tag == "Player"))
                        {
                            var skillobject = Instantiate(callSkill.prefab, hit.point, transform.rotation);
                            if (Comtemp.costMp == true)
                                this.GetComponent<Player>().virtualStatus.mp -= (int)callSkill.cost;
                            else
                                this.GetComponent<Player>().virtualStatus.hp -= (int)callSkill.cost;

                            callSkill.lastTime = Time.time;
                            callSkill = new Skill();

                            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
//                            yield return null;
                        }
                        else return;
                        //안맞다면 브레이크를할것임
                    }
                    
                    else if (callSkill.type == SKILLTYPE.PLACE)
                    {
                        //마나깎고 복제
                        this.GetComponent<Player>().virtualStatus.mp -= (int)callSkill.cost;
                        var skillobject = Instantiate(callSkill.prefab, hit.point,transform.rotation);//이정도면됐다고봄 땅속에있어도 오브젝트충돌만 관리할꺼라 일없다
//                            skillobject.AddComponent<PlaceSkill>();
                        callSkill.lastTime = Time.time;
                        callSkill = new Skill();
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
  //                      yield return null;
                    }
                   else if (hit.collider.gameObject.tag == "Enemy")//우선적인지부터보고 맞으면
                   //뭐가됐던 조건성립이니 스킬발동
                   //적이아니라면 현재스킬타입이 범위형인지를또보고 맞으면 복제수행
                   //것도아니면 불발이니 리턴시킴
                    {//마나깎기
                        this.GetComponent<Player>().virtualStatus.mp -= (int)callSkill.cost;
                        

                        //이걸 적이쓴다면? hit을 플레이어로 바꾸면됨
                        var skillobject = Instantiate(callSkill.prefab, hit.transform.position, transform.rotation);
                        //로테이션은 필요없을지도
                        callSkill.lastTime = Time.time;
                        callSkill = new Skill();//초기화안하면 양산됨
                        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

    //                    yield return null;
                    }
                    else if(callSkill.type == SKILLTYPE.TARGET && Input.GetMouseButton(0))
                {
                    callSkill = new Skill();

                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    return;
                }
                //                    else return; //스킬불발

            }
            }
            else if (Input.GetMouseButton(1))
        {
            callSkill = new Skill();

            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }

        //      yield return null;
        //    }
        //나갈대 커서돌려줌
    }

    //내가 무브를하다가 일정시간(아마 공격시간)이되면 아래가 발동되는식으로 구상
    /* 우선 타겟이있는가?(내가 공격범위안에있다면 항상 있을꺼임)
     * 1. 그 타겟을바라봄
     * 2. 복제
     * 3. 끝
     * 4. 일정시간(아마 어택딜레이)후 1로 돌아감 < 이건 반복문을 통하겠음? no yield를 통해볼거임한번
     */
    //우선 코루틴형태로 함해보고, 안되면 Time걸고 해보지

    public void MoveMent()
    {
        view.enabled = false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 disRotation = new Vector3(hit.point.x, this.transform.position.y, hit.point.z);
            transform.LookAt(disRotation);
            move_point.updateRotation = false;
            move_point.SetDestination(hit.point);
            if (hit.collider.gameObject.tag == "Enemy")
            {
                ChangeTarget(hit.collider.gameObject);
            }
        }
    }

    //적의 체인지타겟과 같은기능이면 스테이터스로 옮김
    public void ChangeTarget(GameObject _target)
    {//나의타겟
        if(target == null)
        this.target =_target.transform.gameObject;
        TargetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
        attackmode = true;//이어택모드를 아래에서 ?

    }
    //적과 접촉했을때 이메소드가 활동홤
    //그대로가져온거라 원거리에겐 잘 안되는 면이 있을지도?
    public void TargetContact()
    {
        //접촉을하면 SetDestination을 멈춤 << 하지만 몬스터는 안멈춤 <<공격은 위-아래로한다고침
        move_point.isStopped = true;
        var enemy = target.GetComponent<Enemy>();

        //사실상 이조건에서 시간 관련 조건은 쓸모없다고봄 << 저공격시간이 브레이크역활쯤을하긴함
        if (enemy != null)
        {
            view.enabled = true;

        }
    }

        // //////////////아래는 start,update,온콜리젼 엔터등등?
        // Use this for initialization
    void Start()
    {
        callSkill = new Skill();
          attackmode = false;
        
        move_point = GetComponent<UnityEngine.AI.NavMeshAgent>();
        //        GameManager
        cursor = GameObject.FindWithTag("GameManager").GetComponent<CursorChange>();
        player = this.GetComponent <Player> ();
        myCollisionRadius = GetComponent<SphereCollider>().radius;
//        print(transform.GetChild(0).GetChild(0).tag);
        //해당 GetChild는 값이 바뀔 가능성이 있음
        // 
//        player.Range = this.transform.GetChild(0).GetChild(0).GetComponent<SphereCollider>().radius ;//반을 안쪽에 집어넣었기에 이렇게
        //ㄴ근접전용임 <<활도가능함 활이면 행동대신 발사를하도록
        player.nextattackTime = Time.time;
        //넥스트 어택타임이 프로텍티드보안이 안되는걸로보아 코드가 잘못된기분이듬ㅡㅡ

        view = transform.Find("Eye").GetComponent<Sight>();
    }


    // Update is called once per frame
    private void Update()
    {
        /////////////이동
        if (!move_point.pathPending)
        {
            if (move_point.remainingDistance <= move_point.stoppingDistance)
            {
                if (!move_point.hasPath || move_point.velocity.sqrMagnitude == 0f)
                    view.enabled = true;
            }
        }

        //만약 내가 공격당한다면 view가 트루로되도록함 <<추가적으로 일정시간의 텀이필요할듯
        if (Input.GetKeyDown(KeyCode.A))
        {
            move_point.isStopped = false;
            cursor.ChangeCursor(0);
            attackmode = true;
            //이제 여기서 공격버튼누른상태로 왼클하면 움직이게함
            //가다가 만나면 공격하러 다가가고

        }

        else if (Input.GetMouseButtonDown(1))//오른쪽버튼을 누르면 통상이동 하지만 대상이 몬스터일경우는 공격으로
        {
            move_point.isStopped = false;
            target = null;//타겟지우기
            attackmode = false;
            if (target != null)
                target = null;
            cursor.ChangeCursor(-1);
            MoveMent();

        }
        //내가 중간버튼을 누르고, 원거리모드일경우
        else if (Input.GetMouseButtonDown(2) && player.ranged == true)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Vector3 disRotation = new Vector3(hit.point.x, this.transform.position.y, hit.point.z);
                transform.LookAt(disRotation);//쏜위치바라봄 < 무브먼트에 있던거라서 이걸 따로 함수화 가능할듯

                Shoot();

            }


        }

        //내가 공격모드< 이공격모드가 공격키임) 고 왼쪽클릭했을경우 
        //근데 이공격모드가 2가지일을 하는거같음 잘하면 지울수도
        else if (attackmode == true && Input.GetMouseButtonDown(0))
        {
            //여기서부터 공격시키자 필요한건 공격범위 <<해당공격범위 안에 들어가면 칼휘두르고, 그칼에충돌했다면, 데미지를적용 
            //우선 칼휘두르는것부터ㅇ

            cursor.ChangeCursor(-1);

            if (target == null)
                //어택모드는 가다가 시야에 적이 있다면임 즉 적이 날보던 시야질과 흡사
                MoveMent();
            //어택 한게 적이라면 그적을 잡으러가고, << 무브먼트에서 처리? << Vector3쯤

            //이동중에 적을 만났으면 거기로 이동 << enemy처럼 시야 넣어서 처리할듯
            //적인거 알게되면 애니메이션으로 ! 뜨게 하면?

            view.enabled = true;//어택땅은 시야가 있어야함
        }
        //bug 바라보기만하고 잡진않음 간혹
        //이론상 감ㅈ지는 했는데 이동을안하게됨


        //////////////기능
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryButton.GetComponent<Button>().onClick.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            equipButton.GetComponent<Button>().onClick.Invoke();
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            skillButton.GetComponent<Button>().onClick.Invoke();
        }

        if (callSkill.ID != 0)
        {
            //            print(callSkill);
            //            StartCoroutine(CallFromMouse());
            CallFromMouse();
        }

    }
    private void FixedUpdate()
    {
        if (target != null )
        {

            //이건 접근해서 충돌하면 공격애니메이션을 실행함
            //해당거리가 일정거리이하(무기 사정거리포함거리)면 뭔가하도록

            //타겟과의 거리임 타겟위치 - 현재 내위치의 거리를 구한다
            float sqrDstToTarget = (target.transform.position - transform.position).sqrMagnitude;
            if(player.ranged == true)//공격타입이 원거리임
            {//원거리면 우선 어택땅을 했거나 해서 적을 찾았으면 이게될텐데 여기서 조건은 rangedweapon의 거리임
                //거기서 멈추고, 적을바라본상태(어짜피 보고있을듯)에서 활을쏘는식
                //아마 ㅇ플레이어 아처의 반지름 + 내반지름 + 타겟의 반지름의 제곱
                if (sqrDstToTarget < Mathf.Pow(player.archerRange.radius + myCollisionRadius + TargetCollisionRadius, 2))
                {
                    TargetContact();//타겟접촉했음을 뜻함
                                    //그렇다면 이제 플레이어의 아처에게서 프로젝타일이 나오게? < EquipHandle에서 물체를 발사시킴

                    //이부분이 일정간격으로 행하는 식이면 좋겠음
                    //또 IEum뭐시기?
                    //예를들면 내가 무브를하다가 일정시간(아마 공격시간)이되면 아래가 발동되는식이지
                    //우선 코루틴형태로 함해보고, 안되면 Time걸고 해보지
                    transform.LookAt(target.transform);
                    Shoot();
                    //StartCoroutine(Shoot());
                }
                else
                {
                    move_point.isStopped = false;
                    transform.LookAt(target.transform.position);

                    move_point.SetDestination(target.transform.position);
                }
            }

            //만약 타겟과의거리가 공격범위+내충돌체크기+상대충돌체크기제곱보다 작다면
            else if (sqrDstToTarget < Mathf.Pow((player.range.localScale.x/2) + myCollisionRadius + TargetCollisionRadius, 2) )
            {
                
                TargetContact();

                    this.transform.Rotate(Vector3.up * (player.realStatus.speed), Space.World);
                //이회전을 에너미의 인사공격을 응용해서 하는걸로바꿔봄
                    //코루틴하면다잡고멈춰서 이렇게 해둠 << 코루틴사실상 패기상태 그럼공격은 누가하나? < status의 attack함수가함
                    //                    StartCoroutine(Attack());

            }
            //타겟이 멀리있으면 그타겟을 보고 이동을 재개함
            else
            {               
                move_point.isStopped = false;
                transform.LookAt(target.transform.position);
            }


            if (move_point.enabled)
            move_point.SetDestination(target.transform.position);
//            transform.LookAt(target.transform.position);            
        }
            //공격범위에 닿으면 이걸 행동
            //이오브젝트 충돌체의 반지름+ 무기타입이 weapon일경우엔 그 충돌체의 반지름까지포함
/*       Vector3 rot = transform.rotation.eulerAngles;
        rot.y += 1f;
        transform.rotation = Quaternion.Euler(rot);
   */  
   
    }


}
