using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
//inst용? 아마 여러가지 합치게될지도

//데미지 텍스트구상 \
/* 현재 플레이어의 위치와 내위치를찾음?
 */
public class GameManager : MonoBehaviour {
    public static GameManager inst;
    //플레이어,하우스 2개의 생존체크는 게임매니저에서 할거임 둘다 죽으면 GameOver..이런게 뜰거고 스테이지 시작할때도 Start!는 떴음 좋겠음
    public Player player;
    public House house;
    public bool InBattle = false;//맨처음은 배틀상태가아님
    public GameObject attackText;//나1개로충분할까? 다굴맞는상황에선 여러개가 필요하게되는데
                                 //ㄴ공격받을때마다 이걸양산해냄 < 그리고 이거의수명이 따로존재해서 수명이 다되면 삭제
                                 //위치는 캐릭에서 -2위치 자식으로보내게되겠음
    public Text waveStartText;
    public Text gameOverText;
    public void CreateText(GameObject client,string text,Color _color = default(Color) )
    {
        GameObject board = Instantiate(attackText);

        board.GetComponent<TextMesh>().color = new Color(0, 0, 0, 255);
       
        board.GetComponent<TextMesh>().text = text;
        board.GetComponent<TextMesh>().color = _color;
//        board.transform.position = client.transform.position + new Vector3(0, 0, -1.5f);
        board.GetComponent<RectTransform>().position = new Vector3(0, 0, -1.5f) + client.transform.position ;

        //여기서 작성하고 보냄? no 여기서 리스트로 물건을관리?
        //ㄴ나에게 가장쉬운방법은 해당 복제품에 스크립트가 있어서 거기서 개별적으로 파괴되는거임
    }
    public void AnimationWaveStart()
    {
        Animation ani = waveStartText.GetComponent<Animation>();
        if(ani != null || ani.isPlaying == false)//전자는 없으면 실행안하려고지만 후자는 혹시 모르니 해봄
        {
            ani.Play(ani.clip.name);//속도등의 제어는 안함 몬스터를 가리는 일은 없을거라고봄
            
        }
    }
	void Awake () {
        inst = this;
        
        player = GameObject.Find("Player").GetComponent<Player>();
        house = GameObject.Find("House").GetComponent<House>();
        waveStartText = GameObject.Find("Canvas").transform.Find("TextHelper").transform.Find("WaveText").GetComponent<Text>() ;//이걸 텍스트형으로 둔이유는 다른걸 받는 경우를 조금이라도 줄이기위함
        gameOverText = GameObject.Find("Canvas").transform.Find("TextHelper").transform.Find("GameOverText").GetComponent<Text>();
	}
    //상시체크를 하기위해 Update로 바꿔도될듯
	IEnumerator GameOver()
    {

            
            Animation ani = gameOverText.GetComponent<Animation>();
            if (ani != null)//TODO코루틴으로바꾸기
            {
                ani.Play(ani.clip.name);
                yield return new WaitForSeconds(ani.clip.length +0.3f);
            }
            else yield return new WaitForSeconds(2f);//만약없을경우는 2초기다림 <<이걸 하는이유는 2초의 텀을 어떻게든 마드는거임 스위치는 싫고

                SceneManager.LoadScene("GameOver");
                SceneManager.UnloadSceneAsync("Playing");

    }
	// Update is called once per frame
	void Update () {
    if (house == null || player == null)
        StartCoroutine(GameOver());
        
	}
}
