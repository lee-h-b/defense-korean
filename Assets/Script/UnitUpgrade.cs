using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UnitUpgrade : MonoBehaviour {
    public Text GoldText;
    public Player player;
    int atkUpLv;
    uint atkUpMoney;
    Button atkButton;//버튼이아닌 텍스트로?


    int defUpLv;
    uint defUpMoney;
    Button defButton;

    int hpUpLv;
    uint hpUpMoney;
    Button hpButton;

    public int mpUpLv;
    public uint mpUpMoney;
    public Button mpButton;

    int criUpLv;
    uint criUpMoney;
    Button criButton;

    int     dodgeUpLv;
    uint dodgeUpMoney;
    Button dodgeButton;

    int speedUpLv;
    uint speedUpMoney;
    Button speedButton;

    Button exitButton;
    Text whiteBoard;

    static public GameObject me;
    float saleRate;
    // Use this for initialization
    private void Awake()
    {
        if (me)
        {
            Destroy(gameObject);//이미내가 있음지우고 없으면 지우지말라함정도
        }
        else
            DontDestroyOnLoad(gameObject);
    }
    void Start () {
//            DontDestroyOnLoad(gameObject);
            atkUpLv = 1;
            atkUpMoney = 50;
            defUpLv = 1;
            defUpMoney = 50;
            hpUpLv = 1;
            hpUpMoney = 30;
            mpUpLv = 1;
            mpUpMoney = 35;
            criUpLv = 1;
            criUpMoney = 100;
            dodgeUpLv = 1;
            dodgeUpMoney = 150;
            speedUpLv = 1;
            speedUpMoney = 200;
            me = this.gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        //하드코딩 스멜 간단하게 체력버튼만체크함 왜냐면 여기중 하나라도 ㅇ벗으면 전부다 지워졌다는뜻일거라는 가정하에임ㅇ
        //그리고 기본에 메소드는 여기서 처리하도록함 왜냐면 1번클릭에 2번 적용되는 괴현상이 해결할방도가없음
        //이 업데이트 정보랑, ui정보를 따로 보관하는게 어떨까? 널널거리니깐
        //ㄴ업그레이드가 로드중일때만 움직이도록 변경함
        
        if (hpButton == null && SceneManager.GetSceneByName("Upgrade").isLoaded == true)
        {
            //계속 왔다갔다기에 Find들은 항상 새로해야함..
            whiteBoard = GameObject.Find("Output").GetComponent<Text>();
            player = GameObject.FindWithTag("Player").GetComponent<Player>();
            
            GoldText = GameObject.Find("GoldBar").GetComponent<Text>();

            atkButton = GameObject.Find("Atk").GetComponent<Button>();//.transform.FindChild("Text").GetComponent<Text>();
            //아래는 만약 없으면 하는게 어떨까
            atkButton.onClick.AddListener(() => { UpgradeToAtk(); });

            defButton = GameObject.Find("Def").GetComponent<Button>();//.transform.FindChild("Text").GetComponent<Text>();
            defButton.onClick.AddListener(() => { UpgradeToDef(); });

            hpButton = GameObject.Find("Hp").GetComponent<Button>();// transform.FindChild("Text").GetComponent<Text>();
            hpButton.onClick.AddListener(() => { UpgradeToHp(); });

            mpButton = GameObject.Find("Mp").GetComponent<Button>();//.transform.FindChild("Text").GetComponent<Text>();
            mpButton.onClick.AddListener(() => { UpgradeToMp(); });

            criButton = GameObject.Find("Cri").GetComponent<Button>();//.transform.FindChild("Text").GetComponent<Text>();
            criButton.onClick.AddListener(() => { UpgradeToCri(); });

            dodgeButton = GameObject.Find("Dodge").GetComponent<Button>();//.transform.FindChild("Text").GetComponent<Text>();
            dodgeButton.onClick.AddListener(() => { UpgradeToDodge(); });

            speedButton = GameObject.Find("Speed").GetComponent<Button>();//.transform.FindChild("Text").GetComponent<Text>();
            speedButton.onClick.AddListener(() => { UpgradeToSpeed(); });

            exitButton = GameObject.Find("SceneExit").GetComponent<Button>();//.transform.FindChild("Text").GetComponent<Text>();
            exitButton.onClick.AddListener(() => { Exit(); });


            exitButton.GetComponentInChildren<Text>().text = "나가기";
        }
        else if(SceneManager.GetSceneByName("Upgrade").isLoaded == true)//하드코드인가?! 쨋든 애가 있을 업그레이드가 로드 된게 참일경우
        {
            GoldText.text = "현재 잔액 : " + player.gold.ToString();

            saleRate = Mathf.Max(1 - player.GetComponent<Player>().goldRate , 0);
            atkButton.GetComponentInChildren<Text>().text = "공격 강화 (Lv : " + atkUpLv + ")\n 소모금액 : " + (int)(atkUpMoney * saleRate);
            defButton.GetComponentInChildren<Text>().text = "방어 강화 (Lv : " + defUpLv + ")\n 소모금액 : " + (int)(defUpMoney * saleRate);
            hpButton.GetComponentInChildren<Text>().text = "체력 강화 (Lv : " + hpUpLv + ")\n 소모금액 : " + (int)(hpUpMoney * saleRate);
            mpButton.GetComponentInChildren<Text>().text = "마력 강화 (Lv : " + mpUpLv + ")\n 소모금액 : " + (int)(mpUpMoney * saleRate);
            criButton.GetComponentInChildren<Text>().text = "크리 강화 (Lv : " + criUpLv + ")\n 소모금액 : " + (int)(criUpMoney * saleRate);
            dodgeButton.GetComponentInChildren<Text>().text = "회피 강화 (Lv : " + dodgeUpLv + ")\n 소모금액 : " + (int)(dodgeUpMoney * saleRate);
            speedButton.GetComponentInChildren<Text>().text = "속도 강화 (Lv : " + speedUpLv + ")\n 소모금액 : " + (int)(speedUpMoney * saleRate);
        }

        if (player == null) Destroy(gameObject);

    }
    public void UpgradeToAtk()
    {
        uint cost = (uint)(atkUpMoney * saleRate);
        if (player.gold >= cost)
        {
            player.gold -= cost;//골드빼고 골드넣고할거심
            atkUpLv++;
            player.virtualStatus.minAtk += 1;
            player.virtualStatus.maxAtk += 1;
            atkUpMoney =(uint)(atkUpMoney* 1.3);
            whiteBoard.text = "공격 업그레이드!";
        }
        else
        whiteBoard.text = "골드가"+"부족합니다";
    }
    public void UpgradeToDef()
    {
        uint cost = (uint)(defUpMoney * saleRate);
        if (player.gold >= cost)
        {
            player.gold -= cost;
            defUpLv++;
            player.virtualStatus.def += 1;
            defUpMoney = (uint)(defUpMoney * 1.24);
            whiteBoard.text = "방어 업그레이드!";
        }
        else
            whiteBoard.text = "골드가" + "부족합니다";
    }
    public void UpgradeToHp()
    {
        uint cost = (uint)(hpUpMoney * saleRate);
        if (player.gold >= cost)
        {
            player.gold -= cost;
            hpUpLv++;
            player.virtualStatus.hp += (int)(10 + (hpUpLv*0.5) );
            player.virtualStatus.max_Hp += (int)(10 + (hpUpLv * 0.5));

            hpUpMoney = (uint)(hpUpMoney * 1.26);
            whiteBoard.text = "체력 업그레이드!";
        }
        else
            whiteBoard.text = "골드가" + "부족합니다";
    }

    public void UpgradeToMp()
    {
        uint cost = (uint)(mpUpMoney * saleRate);

        if (player.gold >= cost)
        {
            player.gold -= cost;//골드빼고 골드넣고할거심
            mpUpLv++;
            player.virtualStatus.mp += (int)(10 + (mpUpLv * 0.5));
            player.virtualStatus.max_Mp += (int)(10 + (mpUpLv * 0.5));

            mpUpMoney = (uint)(mpUpMoney * 1.26);
            whiteBoard.text = "마력 업그레이드!";
        }
        else
            whiteBoard.text = "골드가" + "부족합니다";
    }
    public void UpgradeToCri()//추가필요
    {
        uint cost = (uint)(criUpMoney * saleRate);

        if (player.gold >= cost)
        {
            player.gold -= cost;//골드빼고 골드넣고할거심
            criUpLv++;
            player.virtualStatus.criDmg += 0.05f;
            player.virtualStatus.criPer += 0.025f;

            criUpMoney = (uint)(criUpMoney * 1.3);
            whiteBoard.text = "크리티컬 업그레이드!";
        }
        else
            whiteBoard.text = "골드가" + "부족합니다";
    }
    public void UpgradeToDodge()//추가필요
    {
        uint cost = (uint)(dodgeUpMoney * saleRate);

        if (player.gold >= cost)
        {
            player.gold -= cost;//골드빼고 골드넣고할거심
            dodgeUpLv++;
            player.virtualStatus.dodgePer += 0.1f;

            dodgeUpMoney = (uint)(dodgeUpMoney * 1.3);
            whiteBoard.text = "회피 업그레이드!";
        }
        else
            whiteBoard.text = "골드가" + "부족합니다";
    }
    public void UpgradeToSpeed()
    {
        uint cost = (uint)(speedUpMoney * saleRate);

        if (player.gold >= cost)
        {
            player.gold -= cost;//골드빼고 골드넣고할거심
            speedUpLv++;
            player.virtualStatus.speed += 0.02f;

            speedUpMoney = (uint)(speedUpMoney * 1.2);
            whiteBoard.text = "크리티컬 업그레이드!";
        }
        else
            whiteBoard.text = "골드가" + "부족합니다";
    }

    public void Exit()
    {
        //        SceneManager.LoadScene("portpolio", LoadSceneMode.Single);//나가면 뭔가 가해서 위치를 리셋시켜야ㅏㅁ
        //        SceneManager.UnloadScene("upgrade");

        SceneManager.UnloadSceneAsync("Upgrade");
        Time.timeScale = 1f;
        //플레이어가 일단 멈추도록 하기
    }
}
