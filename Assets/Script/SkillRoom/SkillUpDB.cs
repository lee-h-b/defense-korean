using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*스킬업에 대해 개별적으로 적용을 못할거 같아서 DB로만든것
 * 스킬ID를받고 int형 1개 실수형 4개를 받게됨
 * power비율,소모값,크기,속도(지속시간) << 자기버프에선 power랑 속도 역활이바뀜
 * //ㄴ바뀌는게 낫다고봄 파워인플레 감안하면
 */
using System.IO;
public class SkillUpDB : MonoBehaviour {
    [SerializeField]//시험
    public Dictionary<int, Increase> increase = new Dictionary<int, Increase>();
    public static SkillUpDB instance;

    [SerializeField]
    private TextAsset dbPath;
//    private string dbPath = "Assets/Resources/DB/SkillUpDB.txt";
    // Use this for initialization
    private void ReadSkillUpDB()
    {//이걸 게임매니저에 넣어서 쓸수 있지않을까?
        StringReader reader = new StringReader(dbPath.text);

        string file = reader.ReadLine();//리더가 가져온 라인을 읽음
        
        while(file != null)//라인 없을때까지 읽어댐
        {
            string[] line = file.Split(',');//,를 기준으로 나뉨
            double[] upValues = new double[6];//인자가 전부 더블ㅎ
            
            if(line[0].Length == 0 )//0번째 즉 id없으면 넘어감
            {
                file = reader.ReadLine();
                continue;
            }
            for(int i = 0; i< line.Length; i++)
            {
                if (line[i].Length >= 1 && line[i] == " ")
                    line[i] = "0";//빈값이면 0으로함
                upValues[i] = double.Parse(line[i]);
            }
            IncreaseAdd((int)upValues[0], upValues[1], upValues[2], upValues[3], upValues[4],upValues[5]);
            file = reader.ReadLine();
        }
    }

    void IncreaseAdd(int _ID = 0, double power = 0, double cost = 0, double _coolDown = 0, double _size = 0, 
        double _etc = 0)
    {
        increase.Add(_ID, new Increase(_ID, power, cost,_coolDown, _size, _etc) );
    }

    void Start () {
        instance = this;
        /*
        IncreaseAdd(1, 0.26, 0.3, 0.3, 0.3);
        IncreaseAdd(2, 0.23, 0.4, 0.25, 0.44);
        IncreaseAdd(3, 0.17, 0.2, 0.2, 0.33);
        IncreaseAdd(4, 0.275, 0.35, 0.275, 0.4);


        IncreaseAdd(11, 0.1, -0.2, 0.1, 0.25);
        IncreaseAdd(12, 0.3, -0.7, 0, 0.32);
        IncreaseAdd(13, 0.4, -0.2, 0.3, 0.1);
        IncreaseAdd(14, 0.3, -0.2, 0.05, 0.27);
        IncreaseAdd(15, 0.6, 0, 0.02, 0.15);
        IncreaseAdd(16, 0, -0.5, 0.02, 0.3);

        IncreaseAdd(101, 0.3, -1, 0, 0);
        IncreaseAdd(102, 1, -3);
        IncreaseAdd(103, 0.6, -3);//여기까진 버프가 없으니 ㄱㅊ

        //버프는 데미지가 지속시간이고 etc가 증감값임
        IncreaseAdd(104, 0.5, -10, 0, 1.5);
        IncreaseAdd(105, 0.6, -0.2, 0, 2);
        IncreaseAdd(106, 0.8, -0.4, 0, 1.6);
        */
        ReadSkillUpDB();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
[System.Serializable]
public class Increase
{
    public int ID;
    public double powerUp;
    public double costDown;
    //아래2개는 프리팹에 영향을 끼치게됨
    //프리팹의 겟컴포넌트를 해서 범위기면 etc가 radius만큼을 할것이고
    //타겟기이면 그 컴포넌트 내부의 속도를 올릴것 셀프기면 지속시간이 늘겠지
    public double size;
    public double etc;
    public double coolDown;

    public Increase(int _ID = 0, double power = 0, double cost = 0,double _coolDown = 0,
        double _size = 0,   double _etc = 0)
    {
        ID = _ID;
        powerUp = power;
        costDown = cost;
        size = _size;
        etc = _etc;
        coolDown = _coolDown;
    }/*

    public static Increase operator+(Increase me)
    {
        Increase temp = me;
            temp.costDown += me.costDown;
            temp.powerUp += me.powerUp;
            temp.size += me.size;
            temp.etc += me.etc;
        return temp;
    }//오퍼레이터 하려했으나 실패
    */
}