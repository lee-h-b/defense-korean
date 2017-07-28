using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
/// <summary>
/// 
/// 5번스킬은 클릭+퍼지도록 해보고싶음
/// 자식들이 논타겟이고 <포이즌노바처럼
/// place스킬이 논타겟으로 하니 움직이는거보고 영감얻음
/// </summary>
/// //TODO:: 스킬과 아이템의 db읽는 방식,설명있는방식은 똑같음 그렇담?
/// 게임매니저에서 할수도 있겠지 << 난관이있다면 반환방식 << 타입추론 걸던지 일부는 받는식이면 쉬울거임
//아이디,이름,타입,최대레벨,배율,이름

//스킬슬롯이 스킬을 가지고있다가 내가 해당 슬롯을 사용했다면
//해당스킬슬롯에서 prefab을 발사함, 그프리팹은또한 이스킬을 가지고있도록
public enum SKILLTYPE { PASSIVE, PEACE, NONTARGET, GUIDED, TARGET, PLACE }
public class SkillDB : MonoBehaviour {
    public string[] TYPENAME;
    public Dictionary<int, Skill> skills = new Dictionary<int, Skill>();
    public Skill test;
    public static SkillDB instance;

    [SerializeField]
    private TextAsset dbPath;
    [SerializeField]
    private TextAsset DescriptionPath;

//    private string dbPath = "Assets/Resources/DB/SkillDB.txt";
//    private string DescriptionPath = "Assets/Resources/DB/SkillDescription.txt";
    // Use this for initialization
    void ReadTheSkill()
    {//스킬db순서 id,이름,타입,맥스레벨,코스트,위력,이름
        StringReader reader = new StringReader(dbPath.text);

        string fileData = reader.ReadLine();//라인읽기
        
        while (fileData != null)//라인이 없을때까지 읽음
        {
            string[] lineData = fileData.Split(',');//아이템 db참고
            double[] skillValues = new double[8];//스킬중 1개는 float니깐편의상 이렇게
            string prefabName = " ";
            string name = " ";
            //스킬의 이미지는 따로안받음 이걸해봤자 내가할거같지도 않고 하니깐
            if (lineData[0].Length >= 1)
                skillValues[0] = double.Parse(lineData[0]);//0번째,즉 ID가 있다면 읽어서가져감
            else
            {
                fileData = reader.ReadLine();//없다면 다음장
                continue;
            }
            name = lineData[1];//첫번째는 무조건 스킬의 이름이 될거임
            for(int i = 2; i< lineData.Length; i++)
                //스킬의 0,1번째는 이미했으니 2번째부터함 
            {
                if(i == 7)
                {
                    prefabName = lineData[7];
                    continue;
                }
                if (lineData[i].Length == 0 || lineData[i].Length == 1 && lineData[i] == " ")
                    //내가 아무것도없을때 넣을경우엔 0으로하도록만듬 
                    lineData[i] = "0";
                //스킬밸류 0번째는 위에서했고, 더블로변환을 하면됨
                skillValues[i - 1] = double.Parse(lineData[i]);
            }
            //반복을통해 대입이 끝났다면 스킬추가의시간
            //스킬db순서 id,이름,타입,맥스레벨,코스트,위력,이름
            SkillAdd((int)skillValues[0], (int)skillValues[2], name, 
                (SKILLTYPE)skillValues[1], (int)skillValues[3], 
                (float)skillValues[4],skillValues[5], prefabName);
            //인트형변환이 많은데? float를 온전히 담을려고 double로 했음
            fileData = reader.ReadLine();
        }
    }
    private void ReadTheDecription()
    {
        StringReader Reader = new StringReader(DescriptionPath.text);
        string fileData = Reader.ReadLine();
        while (fileData != null)
        {
            string[] line = fileData.Split(',');
            int id;
            if (int.TryParse(line[0], out id))
                id = int.Parse(line[0]);
            else
            {
                fileData = Reader.ReadLine();
                continue;
            }
            //아이디와 같은 아이템을찾기
            if (skills.ContainsKey(id))
            {
                skills[id].description += line[1];
            }
            fileData = Reader.ReadLine();
        }
    }
    void SkillAdd(int _ID = 0, int _maxlv = 0, string _name = "none", SKILLTYPE _type = SKILLTYPE.PEACE,
        int _cost = 0, double _power = 0,double _coolTime = 0, string _prefabName = "", Sprite _image = null)
    {
        skills.Add(_ID, new Skill(_ID, _maxlv, _name, _type, _cost, _power,_coolTime, _prefabName , Resources.Load<Sprite>("Skill/" + _ID) ) );
    }

    void Start () {
        TYPENAME = new string[6] { "패시브", "비전투", "논타겟", "유도", "타겟", "범위" };
        instance = this;
        /*
                SkillAdd(1, 3, "파이어볼트", SKILLTYPE.NONTARGET, 7, 0.8, "FireBolt");
                SkillAdd(2, 3, "아이스피어", SKILLTYPE.GUIDED, 5, 0.65, "IcePear");
                SkillAdd(3, 3, "정전기", SKILLTYPE.TARGET, 3, 0.4, "Static");
                SkillAdd(4, 5, "아이스파이어", SKILLTYPE.GUIDED, 5, 0.85, "IceFire");

                SkillAdd(11, 5, "독장판", SKILLTYPE.PLACE, 10, 0.5, "PoisonField");
                SkillAdd(12, 5, "돌팔매질", SKILLTYPE.PLACE, 15, 1.2, "StoneShower");//위력만큼의 공방감소가 일어나게됨
                SkillAdd(13, 5, "윈드포스", SKILLTYPE.PLACE, 4, 2.4, "WindForce");//위력만큼의 속도감소
                */
//        string data = new StreamReader(dbPath).ReadLine();//혹시될까해서 해봄
        ReadTheSkill();
        ReadTheDecription();
        
    }

    // Update is called once per frame
    void Update () {
//        test = skills[1];
	}
}


//스킬사용했을때 얘가 버프스킬이면 버프지속시간 + 쿨타임으로 해서 무한지속이 안되게함
[System.Serializable]
public class Skill
{
    public int ID;
    public int lv;//업그레이드는 스킬슬롯이 함
    public int maxlv;
    public string skillName;
    public double cost;
    public double power;//스킬데미지 배율 스킬 << 그냥 고정데미지형식?
    public SKILLTYPE type;
    public Sprite image;
    public GameObject prefab;//그냥 이미지와 똑같은이름으로 돌릴지도
    public string description = "";//설명
    public float coolTime;//스킬쿨타임 실제로 자주쓰는건 슬롯이 될거같은데 잘될까
    public float lastTime;//최근쏜시간 <<슬롯이 가지고 있으면 된다고생각함, 나자신이 가져서뭐에씀
    //쏘는 슬롯이 소유
    //ㄴ 나자신이 가져서 뭐에쓰냐고? 클릭형 스킬이 쿨타임을 기록하는데 쓰지

    //힘,민,지가없으니 보너스 status는 없음 순수하게 캐스터의 atk만

    public Skill(int _ID = 0,int _maxlv = 0, string _name = "none", SKILLTYPE _type = SKILLTYPE.PEACE,
        int _cost = 0, double _power = 0, double _coolTime = 0,string _prefab = null, Sprite _image = null)
    {
        ID = _ID;
        lv = 0;
        maxlv = _maxlv;
        skillName = _name;
        cost = _cost;
        power = _power;
        type = _type;
        image = _image;

        if (ID != 0)
        {
            prefab = Resources.Load("Skill/Prefab/" + _prefab) as GameObject;
            //해당타입에 맞는 컴포넌트가 없다면 넣어주기
            switch(type)
            {//아직안만들었으면 주석처리 해둠
//                case SKILLTYPE.PASSIVE:if(prefab.gameObject.GetComponent break;
//                case SKILLTYPE.SELF: break;
                case SKILLTYPE.NONTARGET:
                case SKILLTYPE.GUIDED:
                    //넌타겟과 유도는 같은 컴포넌트를 사용함
                    if (prefab.gameObject.GetComponent<NonTargetSkill>() == null)
                        prefab.gameObject.AddComponent<NonTargetSkill>(); break;
                case SKILLTYPE.TARGET:
                    if (prefab.gameObject.GetComponent<TargetSkill>() == null)
                        prefab.gameObject.AddComponent<TargetSkill>();
                    break;
                case SKILLTYPE.PLACE:
                    if (prefab.gameObject.GetComponent<PlaceSkill>() == null)
                        prefab.gameObject.AddComponent<PlaceSkill>();
                    break;

            }


        }
        coolTime = (float)_coolTime;//얘는 뒤늦게 추가된거니깐 뒤에 추가했음
        lastTime = -(float)_coolTime;//라스트타임 + 쿨타임이조건임 그렇다면 쿨타임만큼빼면 사실상 0초임 복잡한계산대신 이게짱인듯ㅎㅎ

    }
}