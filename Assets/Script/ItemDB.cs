using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

//아이템 데이터베이스의 대장쯤
//이걸 추가했으니 기존의 드랍아이템, 장비쪽이 대거 바뀔가능성이 있음
//이건 DB니깐 txt를 기반으로 가져올 가능성이 농후

    //todo : 크리,회피의경우에는 나누기 를 해서 넣도록, 여긴다 int임
public class ItemDB : MonoBehaviour {
    public Dictionary<int,ITEM> items = new Dictionary<int,ITEM>();//리스트 하나만듬
    public static ItemDB instance;//아이템 db인스턴스화
    public string[] TYPENAME = new string[7] { "소모품", "무기","방패","헬멧","갑옷","부츠","링"};
    /*장거리무기를 그냥 지우고 Ranged 같은걸로 바꿀까?
     * 불편할듯 < 스킬처럼 오브젝트를 소환하는식은? <db많아지면 오브젝트도 기하급수적
     * ㄴ 어디가불편함 아이템추가할때 bool형으로 뭔가 그려ㄴ면될텐데 
     * ㄴ EquipItem에 불필요한 bool이생김, 이 bool은 무기만있음 되는데 방패 등등도생겨남
     * 그렇다면 이걸 Weapon클래스를 하나 만들어서 하는수밖에 없다고생각함
     */

    //여기서 DB(텍스트를 읽어서 아이템 리스트에 추가하게될거같음) 
    //이대로는 퍼블릭이라 그렇게됨 메모장을 읽고 그걸 포함시키는 형식으로 바꾸고싶음
    [SerializeField]
    private TextAsset DBpath;
    [SerializeField]
    private TextAsset DescriptionPath;
    //아래로하면 빌드할때 내컴아니면 안되게됨
//    private string DBpath = "Assets/Resources/DB/ItemDB.txt";
//  private string DescriptionPath = "Assets/Resources/DB/ItemDescription.txt";

// Use this for initialization
   void ItemAdd(int _ID = 0, string _name = "Nothing", ITEMTYPE _type = ITEMTYPE.CONSUMPTION,
        int _price = 0, int _hp = 0, int _mp = 0, float _delay = 0,Sprite _image = null)
    {
        //아이템 추가함수, 그냥 아이템생성자 카피했음 텍스트로 바뀐다면 이런거 필요없을듯
        //이미지는 다른방식으로, resources를 이용해서 넣을거임
        //강좌에서는 아이템이름을 통해 했는데 나는 이름을 한글로하고 그럴꺼임
        //따라서 아이템 아이콘이될 이미지를 ID로함
        
        items.Add(_ID,new ITEM(_ID, _name, _type, _price, _hp, _mp,_delay,Resources.Load<Sprite>("Item/"+_ID)));
    }
    void EquipAdd(int _ID = 0, string _name = "none", ITEMTYPE _type = ITEMTYPE.WEAPON, bool _twoHand = false,
        int _price = 0, int _hp = 0, int _mp = 0, int _atk = 0, int _def = 0,
        float _speed = 0.0f, float _range = 0.0f, float _Delay = 0.0f,
        float _cri = 0.0f, float _dodge = 0.0f, Sprite _image = null)
    {
        if (_type == ITEMTYPE.WEAPON || _type == ITEMTYPE.RANGEDWEAPON)//만약 얘가 무기타입일경우
            /* 범위면 참을, 아니면 거짓할려 했는데 이러면 양손무기의경우 어찌할지 대처가 안됨
             * //bool을 타입처럼 앞측에 두는것과 맨뒤에 박아두기가 있는데 전자면 소모품의경우 쓸모가없는거고 후자면 원거리는 전부 표시하는 촌극이 발생함
             * 만약 얘의 타입이 양손무기일경우 price위치의값을 0이면 ~하는식으로하는건? linedata의 시작점이 바뀌어야함
             * //i를 밖으로두면됨
             */
            items.Add(_ID, new Weapon(_ID, _name, _type, _twoHand, _price, _hp, _mp, _atk, _def, _speed, _range, _Delay, _cri, _dodge));
        else
        items.Add(_ID,new EquipItem(_ID, _name, _type, _price, _hp, _mp, _atk, _def, _speed, _range, _Delay, _cri, _dodge) );

    }
    private void ReadTheItem()
    {
        StringReader Reader = new StringReader(DBpath.text);

        string fileData = Reader.ReadLine();
        while (fileData != null)
        {
            //        var lines = fileData.Split('\n');//ㅁㄹ 
            string[] lineData = fileData.Split(',');//var는 보기가 어려움
            //lineData스트링 모음에 split,간격으로 잘라서 넣음
            //            if (lineData.Length == 0)//라인의 데이터가없으면
            //              Reader.Close();//리더닫기
            int[] itemValues = new int[6];
            float[] itemfloats = new float[5];//소수점형태로 나올게 실제로 몇개 있음

            string itemName = " ";
            ITEMTYPE itemType = ITEMTYPE.CONSUMPTION;
            int intpos = 1, floatpos = 0;//인트는 시작하자마자 하니깐 1로함
            int temp = 0;
            int i = 3;
            bool twoHand = false;
            Sprite itemImg = null;
            ////////////////TwoHand 위가 아랠르 하는데 필요한 변수들
            if (lineData[0].Length >= 1)
                itemValues[0] = int.Parse(lineData[0]);
            else
            {
                fileData = Reader.ReadLine();
                continue;
            }
            //아이템의 타입이 무기나 랜지드면 twoHand에 변화를줌
            //ㄴ 반복문에 있던지 타입또한 빼내는게 자연스럽다고봄
            itemType = (ITEMTYPE)int.Parse(lineData[2]);//인트 Parse로 string인 문장을 바꿔내고, 아이템타입의 값으로 변환
            //그리고 여기서 얘가 무기쪽이면 bool에 값을 넣어주고 i의 값이 증가해서 무기는3부터, 비무기는 4부터 시작하게해줌
            if(itemType == ITEMTYPE.WEAPON || itemType == ITEMTYPE.RANGEDWEAPON)
            {
                int num = int.Parse(lineData[3]);
                if (num != 0)
                    twoHand = true;//장거리무기라는 놈은 이제없음 이게참이면 양손무기임
                else twoHand = false;
                i++;
            }
            itemName = lineData[1];

            for (; i < lineData.Length; i++)
            {
                if (i == 10)//없겠지만 스프라이트 대입쯤
                {
                    itemImg = Resources.Load<Sprite>("Item/" + lineData[i]);
                    continue;
                }
                //아이템아이디다음은 무조건 이름으로 할건지라 안바뀔듯
                if (lineData[i].Length == 0 || lineData[i].Length == 1 && lineData[i] == " ")
                    //내가 아무것도없을때 넣을경우엔 0으로하도록만듬 
                    lineData[i] = "0";//do 0
                if(int.TryParse(lineData[i], out temp) == false || intpos >= itemValues.Length)//만약 인트형변환실패 혹은 이미 인트가 다찼으면
                {
                    itemfloats[floatpos] = float.Parse(lineData[i]);
                    floatpos++;
                }
                else
                {
                    itemValues[intpos] = int.Parse(lineData[i]);//없는건 범위에서 벗어난거임
                    intpos++;
                }
            }
            if (itemType == (int)ITEMTYPE.CONSUMPTION)
                ItemAdd(itemValues[0], itemName, itemType, itemValues[1], itemValues[2], itemValues[3],itemfloats[0], itemImg);
            else
                EquipAdd(itemValues[0], itemName, itemType,twoHand, itemValues[1], itemValues[2], itemValues[3],
                    itemValues[4], itemValues[5], itemfloats[0], itemfloats[1], itemfloats[2], itemfloats[3], itemfloats[4],
                    itemImg);
            //하드코딩냄새나서 싫음
            fileData = Reader.ReadLine();
        }
        Reader.Close();
    }
    private void ReadTheDecription()
    {
        StringReader Reader = new StringReader(DescriptionPath.text);
        string fileData = Reader.ReadLine();
        while(fileData != null)
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
            if (items.ContainsKey(id))
            {
                items[id].description += line[1];
            }
            fileData = Reader.ReadLine();

            //여기서 아이템에 접근 접근방법은 해당아이템 있나찾기
            //if  (ItemDB.instance.items.ContainsKey(ID))
        }
    }
private void Awake()
{
    instance = this;//인스턴스를 자기자신으로함

        //하드코딩으로 아이템추가한다
        //        ItemAdd(101, "동칼", ITEMTYPE.WEAPON, 1000, 0, 0, 5, 0, 0, 0, 0, 0, 0);
        //        EquipItem item = new EquipItem(101, "동칼", ITEMTYPE.WEAPON, 1000, 0, 0, 5, 0, 0, 0, 0, 0, 0);
        //                EquipAdd(101, "동칼", ITEMTYPE.WEAPON, 1000, 0, 0, 5, 0, 0, 0, 0, 0, 0);
        //       EquipAdd(102, "금칼", ITEMTYPE.WEAPON, 2500, 0, 0, 15, 10, 0, 0, 0, 0, 0);
        //      ItemAdd(501, "사과?", ITEMTYPE.CONSUMPTION, 100, 25, 25);
        ReadTheItem();
        ReadTheDecription();


//        EquipAdd만약 얘가 소모품일경우엔 itemadd를, 아니면 equipadd를 하도록

        // ItemAdd(101, "동칼과자", ITEMTYPE.CONSUMPTION, 0, 10, 10);
        //       ItemAdd(102, "은칼", ITEMTYPE.WEAPON, 2500, 0, 0, 8, 2, 0, 0, 0, 0, 0);
        //ㄴ 이아이템들은 장비임

        //스타트에 있던건데 스타트에있으면 늦게 추가되는 상황이 발생하게됨

    }
    void Start () {
}

// Update is called once per frame
void Update () {
}
}


public enum ITEMTYPE { CONSUMPTION, WEAPON,RANGEDWEAPON, SHIELD, HELMET, ARMOR, BOOTS, RING };
//이것도 equip에서뺐음 일단 item에 넣으면 스크립트가 많아지니 한번 DB와 아이템을 합쳐봣음
[System.Serializable]
public class ITEM 
    //아이템을 상속받는 Equip를 사용함
{
public int ID;
public string itemName;
public int price;//값
//    public int val; //이값은 만약 아이템일 경우 발동될효과? 그냥 hp,mp가 CONSUMPTION타입이면
//hp,mp만큼 회복되게 <<지속시간형태면? ㅁㄹ 생각안해봄
//카운트는? 인벤이 담당해야할것 얘는 그저 아이템의 "정보" 갯수는 인벤의 "정보"임
//public int atk;
//public int def;
public int hp;
public int mp;
public float delay;

//public float speed;//스피드? 안넣어도될꺼같음
//public float range;//공격범위
//public float atkDelay;//공격시간 << \얘도 안쓰던거같은데?
                      //그러면 이 공격시간으로 애니메이션을 행해야하는데 < Lerp로 해결?
//public float criPer;
//public float dodgePer;

//atk~부터 전부 Equip시스템에서 빼옴 < 따라서 모노비헬롭대신 아이템을 상속받아야 할거임
protected ITEMTYPE type;//자세한건 겟타입 참조 무기가 원수임

public Sprite image;
    public string description = "";
public ITEM(int _ID = 0, string _name = "none",ITEMTYPE _type = ITEMTYPE.CONSUMPTION,
    int _price = 0,int _hp = 0, int _mp = 0,float _delay = 0,Sprite _image = null)
{
    ID = _ID;
    itemName = _name;
        type = _type;

        price = _price;
    hp = _hp;
    mp = _mp;
        //약은 간단하게 체력,마력만 
        /*
    atk = _atk;
    def = _def;
    criPer = _cri;
    dodgePer = _dodge;
    speed = _speed;
    range = _range;
    atkDelay = _atkDelay;*/
        image = _image;
        delay = _delay;
//        new GameObject()
}
public ITEM()
{
}
    public virtual ITEMTYPE GetItemType(bool detail = false)//일종의 꼼수, 뷰어쪽에서 오버라이드로 무기쪽 함수를 따로 만드는것도 있겠다만 차라리 그것보단
     //이렇게 타입을 숨기고 타입 호출은 얘를 통해 하도록하는게 좀더 자연스럽다고봄, 또한 원거리무기면 조건을통해 원거리무기라고 반환을 시켜야함
     //왜 원거리무기를 제대로안쓰고 회피해서 쓰느냐? 슬롯에 허가타입을 2개이상 넣기 싫어서
     //ㄴ 지금 이반환때문에 다시 원거리 무기행당하고 있음 << 불편하게도 detail문을 넣어서 간단하게 표시할렴 하고 아님 세부 형태를 반환 하게함ㅎ
    {
        return this.type;
    } 
}
