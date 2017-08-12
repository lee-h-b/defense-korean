using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
//디버프를 따로 빈오브젝트 만들어서 거기에 넣어두는식으로 해야겠음
//옵션 중첩되면 갱신만해서


    //스킬쿨타임의 실질적 사용은 얘가 할것임
    //스킬이 소환되면 스킬의 라스트타임을 쿨타임 + 타임으로하게될거

public class SkillSlot : MonoBehaviour, Slot, IDragHandler, IEndDragHandler,IPointerEnterHandler, IPointerExitHandler
{

    public int skill_ID = 0;//스킬슬롯아이디를 외부에서입력받고, 스킬db에 이게 있는지 확인,적용

    public Skill skill;
    public SkillSlot[] needSkill;//타입을 스킬형으로 하면 어떤스킬이 필요한지 찾기난해함
    public Increase bonus;
    //db따와야하는데 그db에서 알아와야하고 난감
    //클릭할때 NeedSkill을 배웠는지 확인해야함
    //  public Increase bonus;//스킬의 프리팹의 컴포넌트에 있게됨
    public bool lean = false;//배워야 use행가능
    public Text lvText;//내 레벨텍스트
    public Image img;//skillImage는 작대기가 3개라 보기힘듬
    public Button upButton;
    public GameObject needArrow;
    public Player caster;


    //    private float lastTime;//이 라스트타임에서 스킬쿨타임 + 타임만큼의 값이 추가되어 실제로 쿨타임역활을함 
    //ㄴ 실제로는 그렇게 안될거같음, 스킬을 타겟팅형태로 쏘는 애들은 얘가 어디에있는지그런걸 모름
    //설상 찾는다 하더라도 시간이 너무오래걸려서 비효율적임
    private float current;

    //니드스킬빼고는 전부 private해도될듯
    //캐스터도빼고, lvtext,img,arrow는 전부 일단 찾아주면 해결


    public void FindSkill()
    {
        if(SkillDB.instance.skills.ContainsKey(skill_ID) == true)
        skill = SkillDB.instance.skills[skill_ID];

        //항상 스킬의 프리팹에는 스킬오브젝트가 있단 가정하에움직임 << 오류위험
        if (SkillUpDB.instance.increase.ContainsKey(skill_ID) == true)
            bonus = SkillUpDB.instance.increase[skill_ID];
    }
    public void SlotUse()
    {

        if ( ( (skill.prefab.GetComponent<SkillObject>().costMp == true && caster.realStatus.mp >= skill.cost) ||
            (skill.prefab.GetComponent<SkillObject>().costMp == false && caster.realStatus.hp >= skill.cost)  )&&
            (skill.lastTime + skill.coolTime < Time.time ))
        {//조건이 좀길어서보기 안좋을지도 모르지만 소모가 체력인지 마력인지 확인하는것 + 쿨타임체크
            switch (skill.type)
            {
                case SKILLTYPE.NONTARGET:
                case SKILLTYPE.GUIDED:
                    {
                        caster.virtualStatus.mp -= (int)skill.cost;
            GameObject temp = Instantiate(skill.prefab, caster.transform.position + new Vector3(0f, 0.5f, 0f), caster.transform.rotation);
                            //걍스킬오브젝트로해도되지만 이렇게하는게 좀더 직관?적인거같음
                        temp.GetComponent<NonTargetSkill>().caster = this.caster.gameObject;

                        if (skill.type == SKILLTYPE.GUIDED)
                        {
                            temp.GetComponent<NonTargetSkill>().guided = true;
                        }
                        skill.lastTime = Time.time;
                        break;
                    }

                case SKILLTYPE.PEACE:
                    //버프가 있으면서 하우스를 전용이아닐경우에만
                    if (skill.prefab.GetComponent<PeaceSkill>().buffType != BuffType.NONE && skill.prefab.GetComponent<PeaceSkill>().houseOnly == false)
                    {
                        caster.virtualStatus.mp -= (int)skill.cost;
                        GameObject temp = Instantiate(skill.prefab, caster.transform.position + new Vector3(0f, 0.5f, 0f), caster.transform.rotation);
                        //걍스킬오브젝트로해도되지만 이렇게하는게 좀더 직관?적인거같음
                        temp.GetComponent<SkillObject>().caster = this.caster.gameObject;
                        skill.lastTime = Time.time;
                        
                        break;
                    }
                    else
                    {
                        skill.prefab.GetComponent<SkillObject>().caster = this.caster.gameObject;
                        caster.GetComponent<PlayerController>().callSkill = skill;
                        break;
                    }

                case SKILLTYPE.TARGET: //스킬이 타겟이면 우선 마우스로 클릭하는걸잡아서 그게 적일경우에만 발싸가 가능함
                case SKILLTYPE.PLACE:
                    skill.prefab.GetComponent<SkillObject>().caster = this.caster.gameObject;
                    caster.GetComponent<PlayerController>().callSkill = skill;
                    break;
                    //ㄴ 타겟과 범위의 공통점은 내가 마우스로 타겟을 찝어준다에 있음
                    //ㄴ콜스킬을 경유해서 컴포넌트를 추가함
            }
            //데미지공식 최소~최대 +1의랜덤 * 스킬위력
//            skill.prefab.GetComponent<SkillObject>().damage = (float)skill.power * (Random.Range(caster.minAtk, caster.maxAtk + 1));
            for(int i = 0; i < skill.prefab.GetComponents<SkillObject>().Length; i++)
                skill.prefab.GetComponents<SkillObject>()[i].damage = (float)skill.power * (Random.Range(caster.realStatus.minAtk, caster.realStatus.maxAtk ) /(i+1));
            //스킬컴포넌트가 여러개일경우를 가정 //ex 스파크처럼 범위지속딜까지 있는경우ㅡ 그럴경우 많을수록 오피가될테니 /i값 으로 데미지조정을 꾀함


            

        }
        else if(skill.lastTime + skill.coolTime >= Time.time)
            GameManager.inst.CreateText(caster.gameObject, "스킬이 준비되지 않았습니다", Color.black);
        else if (skill.prefab.GetComponent<SkillObject>().costMp == false )//하드코딩 스럽다
            GameManager.inst.CreateText(caster.gameObject, "체력이 부족합니다!", Color.black);
        //아마 마력 회복스킬 같은 체력소모 스킬을 쓰면 나옴
        else
            GameManager.inst.CreateText(caster.gameObject, "마나가 부족합니다!", Color.black);
        
    }
	// Use this for initialization
    //마우스클릭하면 발동하도록함 쓰고싶으면 드래그해서 퀵슬롯으로 가거나 더블클릭으로 할것
    //스킬업그레이드가 문제 db로짤려나?
    //ㄴ 결국 db로 짜게됨
    public void SkillUp()
    {
        skill.power += bonus.powerUp;
        skill.cost -= bonus.costDown;

        skill.prefab.GetComponent<SkillObject>().bonusUp(bonus.size, bonus.etc);
        InfoBox.instance.ShowSkill(skill);

    }
    public void leanSkill()
    {
        //패시브는 배우자마자 바로 적용이 되는게 좋겠음
        //아니면 디버프처럼 하고 영구히 거기에 존속되던지
        //지금생각한 패시브방식이 여기서 스위치문으로 해당 ID면 그냥 그걸 늘리는거임
        /* 하드코딩같음 버프,디버프처럼 증가시키되,지속시간이 무한인꼴이 더 보기좋을듯
         * 그렇담 문제는 해당스킬이겠음 골드의 값변동 스테이터스에서 골드를 건드리도록?
         * 만약이걸쓴다면? 빈공간으로 만들고 일안함? 나오자마자 나에게 패시브를 주고 소멸하겠음
         * 그럼 이게 레벨이 오르면? 기존방식이면 레벨이 오르면 그레벨은 다음 스킬오브젝트부터 발동이됨
         * 게다가 무엇보다 패시브는 오브젝트를 call을 통해불러들일수가 없음
         * 배우면 빈오브젝트로 캐릭터에게 주고, 스킬업을할때마다 그 배운 빈오브젝트를 찾아서 거기에 값을 변동시키는형식
         * 빈오브젝트를 할바에 직접 캐릭의 능력치를 수정할듯
         * 그럼 걍직접 수정하는건 < 해당아이디에따른 능력치변동을 적용해야하니싫음
         *   상시적용이니깐 위의 형태를 따르도록함 배우면 바로 해당 오브젝트를 캐스터의 패시브의 자식으로두기
        *         
         */
        if (needSkill.Length >= 1)
        {
            for (int i = 0; i < needSkill.Length; i++)
            {
                if (needSkill[i].lean == false)
                    return;
            }
        }
        InfoBox.instance.Reset();

        if (lean == false)
        {
            img.color = Color.white;
            lean = true;
            skill.lv = 1;
            lvText.gameObject.SetActive(true);

            //만약 이게 패시브일경우
            //패시브스킬설정하고 겸사겸사 인포박스도 설정해줌
            if (skill.type == SKILLTYPE.PASSIVE)
            {
                caster.transform.Find("Passive").GetComponent<PassiveList>().InitSwitch(skill.ID);
                //                caster.transform.Find("Passive").GetComponent<PassiveList>().AddList(skill.ID, prefab.GetComponent<PassiveSkill>());
                //ㄴ 2번째 안될확률이 높음 스킬오브젝트 자체를 찾기에,
                //프리팹을 캐스터의 패시브의 자식으로둠
                InfoBox.instance.ShowSkill(skill, caster.transform.Find("Passive").GetComponent<PassiveList>().GetSkill(skill.ID).gameObject);

            }
            //패시브가 아니면 쇼스킬은 그냥 스킬만 담아감
            else InfoBox.instance.ShowSkill(skill);


        }


        else if (skill.lv < skill.maxlv)
        {//여기서 업그레이드 발동
            skill.lv += 1;

            if(skill.type == SKILLTYPE.PASSIVE)
            {
                //스킬업이 알아서 할지도? < 패시브니깐 알아서는 못할지도
                caster.transform.Find("Passive").GetComponent<PassiveList>().PassiveUpgrade(skill.ID,bonus.etc,bonus.size);
                InfoBox.instance.ShowSkill(skill, caster.transform.Find("Passive").GetComponent<PassiveList>().GetSkill(skill.ID).gameObject);
            }
            else      SkillUp();
        }
        if (skill.lv == skill.maxlv)
        {
            lvText.text = "LV : M";
            upButton.gameObject.SetActive(false);

        }
        else
            lvText.text = "LV : " + skill.lv.ToString();
        
        InfoBox.instance.Info.SetActive(true);
    }
    public void Draw()//스킬의 정보를 전부 그려낼거임
    {
        //그림그리기
        this.img.sprite = skill.image;
        if(needSkill != null)
        {//니드스킬이 있으면 필요한 얘들이 전부 얘를 가리키는?화살표를만듬
            //ㄴ그럼 반복문 에서 인스턴스를 그려내도록해야지? arrow를 복제하도록 하자
            //ㄴ맨마지막에ㅇ
          for(int i = 0; i< needSkill.Length; i++)//만약 가르켜야할 스킬이 1개면 << for로 바꿀수있음
            {

                //길어서 간단하게 치기위해
                var arrow = Instantiate(needArrow);
                arrow.SetActive(true);
                arrow.transform.SetParent(this.transform,false);//원래 이미지의 부모로 뒀었는데 그렇게하니깐이미지를 따라가게되서 보기 안좋음
                RectTransform ArrowRect = arrow.GetComponent<RectTransform>();
                //두 물체의 각도 구하기
//                print(arrow.transform.position.x);

                Vector3 dir = needSkill[i].GetComponent<RectTransform>().localPosition - GetComponent<RectTransform>().localPosition;
                dir = needSkill[i].GetComponent<RectTransform>().InverseTransformDirection(dir);
                float angle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

                //y값을 가져옴
                float SkillRectY = needSkill[i].GetComponent<RectTransform>().localPosition.y;


                //                needArrow.SetActive(true);
                //이쯤에서 인스턴스를 소환 셋액티브 하지않고 needarrow를 복제함

                //화살표의위치정의
                ArrowRect.localPosition = new Vector3(needArrow.GetComponent<RectTransform>().localPosition.x, SkillRectY - this.GetComponent<RectTransform>().localPosition.y, ArrowRect.localPosition.z);
                //5.6오면서 바뀐건지 기존대로하면 필요스킬 앞에 생성이되서 원래카피본인 니드에로우의 렉트 트랜스폼의 로컬포지션x를 가져옴
                //문제는 크기 <어찌된일인지 크기가 커짐ㅡ? 따라서 기존 스케일을 1로함 < 이전엔 너무작아서 키웠었음

                //방향 돌리기!
                  ArrowRect.localRotation = Quaternion.Euler(0, 0, -(angle+90f) );
                //수학못하는땔깜이니 다음꼼수를씀 현x위치, 상대y측위치를 위치로 잡는다
                //회전값은 y축은 0도고정, z축은 x축의 반전값으로한다
            }
            img.color = Color.gray;
        }
        else
            img.color = Color.white;
    }
    //스킬업그레이드는 어떻게?
    public void OnPointerExit(PointerEventData data)
    {
        if (InfoBox.instance != null)
        {
            InfoBox.instance.Reset();
//            InfoBox.instance.gameObject.SetActive(false);
        }
    }
    public void OnPointerEnter(PointerEventData data)
    {//스킬에 인포박스달아줘야지
        if (InfoBox.instance != null)
        {
            InfoBox.instance.slot = this;
            if (skill.ID != 0 )
            {
                InfoBox.instance.Info.SetActive(true);

                if(skill.type == SKILLTYPE.PASSIVE)
                InfoBox.instance.ShowSkill(skill, caster.transform.Find("Passive").GetComponent<PassiveList>().GetSkill(skill.ID).gameObject);
                else
                    InfoBox.instance.ShowSkill(skill);

            }
        }
        
    }
    public void OnDrag(PointerEventData data)
    {
        if (lean == true && skill.type != SKILLTYPE.PASSIVE)//안배웠으면 드래그 못해 +추가로 패시브도 드래그못해
        {
            //해당이미지의 아비를 자신으로
            this.img.transform.SetParent(SkillPanel.instance.dragHelper);

            if (SkillPanel.instance != null)
                SkillPanel.instance.dragHelper.GetChild(0).position = data.position;
        }
            return;
    }
    public void OnEndDrag(PointerEventData data)
    {
        SkillPanel.instance.dragHelper.GetChild(0).SetParent(transform);
        this.img.transform.localPosition = Vector3.zero;
        //이미지 돌려주고 실제로 변환술!
        if (data.pointerEnter.GetComponent<QuickSlot>() != null)
        {
            QuickSlot quickslot = data.pointerEnter.GetComponent<QuickSlot>();
            var iTemp = quickslot.GetComponent<InvenSlot>().item;
            //여기서 아이템 유무를 체크함 혹시모르니
            if (iTemp.ID != 0)// 아이템이 존재한다면
            {
                //아이템을 null로하고
                quickslot.GetComponent<InvenSlot>().item = null;
                //itemp를 통해백업했던 아이템을 인벤의 카운트만큼늘리고
                GameManager.inst.GetComponent<Inventory>().AddItem(iTemp.ID, quickslot.GetComponent<InvenSlot>().count,true);
                //이카운트를 0으로
                quickslot.GetComponent<InvenSlot>().count = 0;
            }
            quickslot.slot = this;

            quickslot.transform.GetChild(0).GetComponent<Image>().sprite = img.sprite;
            quickslot.transform.GetChild(0).gameObject.SetActive(true);
            quickslot.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
        } 
    }
    public float ShowCoolTime()
    {
        //        current = Time.deltaTime;

        if (skill.lastTime + skill.coolTime > Time.time)//만약 스킬을 마지막쏜시간과 + 쿨타임이 더크면
        {
            current = skill.lastTime + skill.coolTime - Time.time;//스킬을 쓰면 초기화
            //슬롯에서 벗어나면 current가 멈춘셈이된다
            //이벤트ㄱㄱ? 
            //ㄴ ㄴㄴ 스킬쿨과의 차이를 활용하니 끝남ㅎ
  
        }
        return (current) / skill.coolTime;
    }


    void Start () {
        caster = GameManager.inst.player;

        FindSkill();
        if (skill.ID == 0)
        {
            this.gameObject.SetActive(false);
        }
        else if(skill.type != SKILLTYPE.PASSIVE)
        //테이블값으로 초기화
        skill.prefab.GetComponent<SkillObject>().ResetObject(skill.type);
        //        Destroy(prefab);
        else//패시브스킬을 미리 플레이어에 넣어줌 << 왜이럼? 스킬설명 쓸때 패시브를 복제하고 그실체가 실제로 활동하는데 스킬설명으로 읽히려고하는데 배울때까지 작동안함
        {
            GameObject prefab = Instantiate(skill.prefab);
            prefab.transform.SetParent(caster.transform.Find("Passive"));//또한 레벨업을 하면 이프리팹을 찾아가서 값의 증가를 행할꺼같음\

            prefab.GetComponent<PassiveSkill>().caster = caster.GetComponent<Player>();
            caster.transform.Find("Passive").GetComponent<PassiveList>().AddList(skill.ID, prefab.GetComponent<PassiveSkill>());
            prefab.SetActive(false);
//            caster.transform.Find("Passive").GetComponent<PassiveList>().PassiveSwitch(skill.ID);
            //프리팹을 캐스터의 패시브의 자식으로둠
        }

        Draw();

    }

    // Update is called once per frame
    void Update () {

    }
}
