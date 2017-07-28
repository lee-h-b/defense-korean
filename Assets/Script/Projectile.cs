using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//논타겟스킬이랑 매우 유사하게 굴러갈것
//하지만 화살은 정면에서 맞아야 데미지가 나오게되니 아주작은 데미지를 주는 오브젝트를 붙이거나
//앞에 ray를 쏴서맞추는게 좋을거 fps에서도 응용하기 쉽도록 후자
public class Projectile : MonoBehaviour {
    public LayerMask collisionMask;//충돌결정
    public int shooterMinAtk, shooterMaxAtk;
    int damage = 1;//임시
    float speed = 10f;
    float lifeTime = 5f;//수명
    public Status Shooter;//불필요하다고 생각함 하지만 데미지계산을 공격자가 디펜더에게 가하는식으로해서 어쩔수가없음
    //디펜더가 데미지를 받고 그데미지를 계산해서 처리하는식이였으면 좋았음

    float skinWIdth = .1f;//곂칠경우 데미지 처리
    public void SetAtk(int _min, int _max)
    {
        shooterMinAtk = _min;
        shooterMaxAtk = _max;
    }

    private void Start()
    {
        if (shooterMaxAtk != 0)//공격을 설정 따로안했으면
            damage = Random.Range(shooterMinAtk, shooterMaxAtk);
        else//플레이어껄 따름 < 어짜피 공격설정은 적쪽만 할것
        damage = Random.Range(GameManager.inst.player.realStatus.minAtk, GameManager.inst.player.realStatus.maxAtk);
        //데미지를 게임매니저 인스턴스의 플레이어에게서 따옴

        Destroy(gameObject, lifeTime);//lifeTIme만큼의 시간을뒤에 스스로삭제

        Collider[] initialCollisions = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);

        if(initialCollisions.Length > 0)//발사했을때 무언가가 들어와 있음
        {
            HitObject(initialCollisions[0]);//0번째 적에게 데미지를 가함
        }
    }

    private void Update()
    {
        float distance = speed * Time.deltaTime;
       transform.Translate(-Vector3.forward *distance);
        CheckCollisions(distance);
    }

    void CheckCollisions(float distance)
    {
        Ray ray = new Ray(transform.position, transform.forward);//전방에 레이저를쏨
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, distance + skinWIdth, collisionMask, QueryTriggerInteraction.Collide))
            //쿼리트리거인터랙션 = 어떤 타입과 충돌할지 고름 이게 Collide니깐 트리거 콜라이더들과 충돌가져옴
        {
            HitObject(hit);
        }
    }
    //이제 여기서 얘가 맞출때 패시브를 돌려야겠다
    void HitObject(RaycastHit hit)
    {
        Status hitObject = hit.collider.GetComponent<Status>();
        if (hitObject != null)
        {
            Attack(hitObject);
        }
        GameObject.Destroy(gameObject);
    }

    void HitObject(Collider c)
    {
        Status hitObject = c.GetComponent<Status>();

        if (hitObject != null)
        {
            Attack(hitObject);
        }
        GameObject.Destroy(gameObject);
    }

    void Attack(Status target)
    {
        if (Shooter.tag == "Enemy")
        {
            Shooter.Attack(target);
        }
        else if (PassiveList.inst.LoopPassiveChance(GameManager.inst.player, target) == false)
            GameManager.inst.player.Attack(target);

    }
}
