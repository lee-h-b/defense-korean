using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//여기서 하우스 업그레이드를 다루도록하고, 스테이터스를 상속받아서 쓸지도모르겠음
public class HouseManager : MonoBehaviour
{
    public GameObject upgrade;
    public GameObject actionUI;
    public Text hpText;
    public Text defText;
    public Text text;
    public Text repairRateText;
    public Text RepairButton;//짜피 뒤에.Text붙이니깐 버튼까지만씀
    public Text RepairRateButton;
    public Text HpUpButton;
    public Text DefUpButton;

    int repairPay;//수리비 최대체력의 15%
    int repairRateUpPay;//수리량 업그레이드 한번할때마다 + 50원쯤
    int hpUpPay;//체력량 업그레이드 hp/10원쯤 증가량은 50?100?
    int defUpPay;//방어력 업그레이드 300원

    private House house;
    private Player player;//플레이어는 안중요하고, 그의 골드가 중요함 하지만 값수정했을때 그값을 다시 넣어줘야하는데 그때마다 플레이어찾아야됨 즉 성능저하

    // Use this for initialization

    // Update is called once per frame
    public void Repair()//성을수리
    {
        if (player.gold >= repairPay && house.realStatus.hp < house.realStatus.max_Hp)
        {
            player.gold -= (uint)Mathf.Max(repairPay, 0);

            int valRepairper;//수리 비율을 인트값으로 한것
            valRepairper = (int)(house.realStatus.max_Hp * house.Repair_Per);
            int result = Mathf.Max(valRepairper, house.Repair_Val);
            //전체체력의 n%와 수리값중 가장큰것
            house.virtualStatus.hp += result;
            house.CheckOverHpMp();
            text.text = "수리 완료!";
        }
        else if (player.gold >= repairPay)
            text.text = "체력이" + " 가득입니다";
        else
            text.text = "돈이" + " 부족합니다";
        //아닐경우 텍스트에서 쑈하도록
    }
    public void RepairRateUp()//성수리하는 비율?체력?회복량증가
    {
        int cost = (int)(repairRateUpPay * (1 - player.GetComponent<Player>().goldRate));

        if (player.gold >= cost)
        {
            player.gold -= (uint)Mathf.Max(cost, 0);

            house.Repair_Per += 0.005f;
            house.Repair_Val += 5;
            repairRateUpPay += 50;
            text.text = "수리술 강화 완료!";
        }
        else
            text.text = "돈이" + " 부족합니다";

    }
    public void HpUp()//최대체력?내구도? 증가
    {
        if (player.gold >= hpUpPay)
        {
            player.gold -= (uint)hpUpPay;
            house.virtualStatus.hp += 50;
            house.virtualStatus.max_Hp += 50;
            text.text = "증축 완료!";
        }
        else
            text.text = "돈이" + " 부족합니다";

    }
    public void DefUp()//방어력증가 <<너무 잘늘면 노잼될거임 찔끔+비쌈
    {
        int cost = (int)(defUpPay * (1 - player.GetComponent<Player>().goldRate));
        if (player.gold >= cost)
        {
            player.gold -= (uint)cost;
            house.virtualStatus.def += 1;
            defUpPay += 150;
            text.text = "보강 완료!";
        }
        else
            text.text = "돈이" + " 부족합니다";

    }
    public void Exit()
    {

        upgrade.SetActive(false);
        Time.timeScale = 1.0f;
    }
    void Start()
    {
        //        upgrade = transform.Find("HouseUpgrade");
        //        upgrade = GameObject.Find("HouseUpgrade");
        //        ActionUI = GameObject.Find("Canvas").transform.FindChild("ActionUI").gameObject;
        //비활성화는 find로 안찾아 준다함 그냥 public으로해서 대입시킴
        house = this.GetComponent<House>();
        //여기서 성의 수리비율을 정의함 체력도 여기로?
        house.Repair_Per = 0.10f;
        house.Repair_Val = 50;
        //이중 높은걸로함

        repairRateUpPay = 200;
        defUpPay = 300;
    }

    private void Update()
    {
        //pay값을 스태틱으로 할까 싶었지만 스태틱은 왠지 피하고싶음
        if (player != null)
        {
            //이 맥스F는 골드 할인이 100%일경우를 감안한것
            repairPay = Mathf.Max((int)(house.realStatus.max_Hp * 0.15 * (1 - player.GetComponent<Player>().goldRate)), 0);
            hpUpPay = Mathf.Max((int)(house.realStatus.max_Hp *0.23 * (1 - player.GetComponent<Player>().goldRate)), 0);
            //디펜하고 리페어의 경우에는 값이 상수로 시작하는데 어떻게 해결 할것인가?
            hpText.text = "내구도 : " + house.realStatus.hp + " / " + house.realStatus.max_Hp;
            defText.text = "방어력 : " + house.realStatus.def;
            repairRateText.text = "수리량 : " + house.Repair_Val + "혹은 " + house.Repair_Per * 100 + "%";
            RepairButton.text = "수리하기 " + repairPay + "원";
            RepairRateButton.text = "수리량증가 " + (int)(repairRateUpPay * (1 - player.GetComponent<Player>().goldRate)) + "원";
            HpUpButton.text = "내구도확장" + hpUpPay + "원";
            DefUpButton.text = "방어력 증가" + (int)(defUpPay * (1 - player.GetComponent<Player>().goldRate)) + "원";
        }
    }
    private void OnTriggerEnter(Collider other)//트리거가 들어오면
    {
        if (other.tag == "Player" && GameManager.inst.InBattle == false)
        {
            player = other.GetComponent<Player>(); //이렇게하면 할때마다 골드대입해줘야하잖아
            //걍플레이어로 함

            other.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            Time.timeScale = 0;//시간멈추고 이동멈추고
                               //버튼활성화시킨다
                               //액션ui도띄울필요o
                               //        ActionUI.SetActive(true);

            upgrade.SetActive(true);//exit하면 이걸 거짓으로돌림
                                    //텍스트 갱신을위해 update가 필요
                                    //        SceneManager.LoadScene("Upgrade", LoadSceneMode.Additive);
            other.GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;//활성을 거짓으로 함으로써 멈추게만들고
                                                                             //신을 부르고 다시 참으로 돌려서 움직이기가 되게함
        }
    }


}
