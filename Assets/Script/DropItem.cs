using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour {

    public enum Type { GOLD, CONSUME, EQUIP};
    public uint value = 1;//우선 밸류만 만들어둠 <<갯수를 겸할거임
      //어떻게넣어줌?
      //적을잡으면, 그적이 아이템 오브젝트를 뱉게한다
      //그렇다면 적이 아이템을 가지고 있어야하는가?
      //http://answers.unity3d.com/questions/155003/item-database.html
      //해당링크처럼 데이터베이스로한다
      //일단 아이템 자체는 미구현이니 골드만 하도록함
      //골드ID는 0
      //플레이어를 하나 타겟으로두고, 그플레이어가 아이템 루팅 범위안에 들어온다면 다가가게
      //이범위는 Eye로 한다 왜냐면 아이템 충돌체로 하면 아이템 오브젝트 아래에도 eye같은걸 넣게됨

      //우선 적이 죽으면 이 아이템이 나오는것부터함
    public int ID = 0;
    public Type type = Type.GOLD;
    public float dropTime;//떨어진시간, 이시간 +특정시간이 지났다면 아이템을 주울수 있음

    public void ChangeItem(int id, Type t, uint val)//그냥 아이템 아이디만 가져가면될거같음
    {
        ID = id;
        type = t;//타입
        value = val;//돈이나, 금액
    }
     // Use this for initialization
    void Start () {
        dropTime = Time.time + 2f;//떨어지고 n초뒤
	}
	
	// Update is called once per frame
	void Update () {
        
		if(dropTime <= Time.time)
        {
            this.GetComponent<BoxCollider>().enabled = true;//이전 처럼 될거같음 
        }
	}

    private void OnTriggerStay(Collider other)
    {
        Player player = null;
        if (other.transform.parent != null &&
            other.transform.parent.tag == "Player")
        {
            //충돌체의 부모가 플레이어 즉아래의 Eye같은 무언가를 찾는셈
            player = other.transform.parent.transform.GetComponent<Player>();

        }
        if (player != null)
        {
            switch (type)
            {
                case Type.GOLD:
                    //원랜 *=으로 하려했으나, 그렇게하면 왠지 value *= 1이 될거같음
                    value = (uint)(value * (1 + player.GetComponent<Player>().goldRate) );
                    player.gold += value;
                    break;
                //그외의 케이스의 경우 아이템 추가함수를 통해서 아이템 넣어줌 
                case Type.CONSUME:
                case Type.EQUIP:
                    GameManager.inst.GetComponent<Inventory>().AddItem(ID, (int)value);
                    break;
/*
                    Inventory.instance.AddItem(ID);//그냥합체?
                    break;
                    */
            }
            //다쫓아갔을경우 << collider로 해도될듯?
            Destroy(gameObject);//일단이걸지움
        }

    }
}
