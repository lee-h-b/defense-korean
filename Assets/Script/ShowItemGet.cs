using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//단순히 아이템을 얻었다는걸 알리는 함수 끝까지내려가고 3초뒤에소멸하는식으로할거임
//테일즈위버때처럼 Rect.overap을 쓰면된다함
//물론 아래가 먼저 끝나면 그다음이 다시내려가야겠지
//인자를어떻게? 콜라이더달아봄ㅎ

//이거 UI를 일종의 queue처럼 인식해야할듯함
//그리고 만약이게 ovelap이될거같거나 되면 어떻게든하고 특정조건내에서 일정시간지나면 dequeue하고
//https://stackoverflow.com/questions/42043017/check-if-ui-elements-recttransform-are-overlapping
//ㄴ이걸응용
/// <summary>
/// 만들고나니깐 queue는 커녕 리스트를 쓰게됐음 왜일까 queue의 중간에 있는값을 오버랩했는지 체크를 못하기 때문
/// </summary>
/// //버그1 사라질때 인벤키/버튼 먹통이됨 << 실제게임에선 잘되는 눈치니 크게 상관없을지도?
public class ShowItemGet : MonoBehaviour {
    //얘는 인스턴스안쓰고 find로 찾아야징
    public Vector3 startPosition;
    public GameObject ItemBarSample;
    public List<RectTransform> bars;

    List<int> test;
    private float deleteTime = 2f;
    public float editDeleteTime;
    //여기서도 코루틴은 무리라고봄 왜냐면 3초기다리자마자 바로 밀린애들이 실행이됨 하지만난 최하단에서 3초는 있다가 사라지면좋겠음
    //즉 조건충족하는 원래 0번째가 계속 실행을 부르고 있는셈
    /*
    IEnumerator TimeUpDelete()
    {
        if (bars[0] != null)
        {
            yield return new WaitForSeconds(3f);
            print("tsu!");
            if (bars[0] == null) yield return null;
            if(bars[0] != null)
            Destroy(bars[0].gameObject);//3초뒤 bars[0]은 삭제함
            //문제 1. 값이 2개동시삭제
            //2 그 2개 동시삭제때문에 1개 대기중에따른 오류
            ListControl();//리스트 재조절
        }

    }
    */

    bool CheckOverlap(RectTransform pos1, RectTransform pos2)
    {
        Rect rect1, rect2;
        rect1 = new Rect(pos1.localPosition.x, pos1.localPosition.y, pos1.rect.width, pos1.rect.height);
        rect2 = new Rect(pos2.localPosition.x, pos2.localPosition.y, pos2.rect.width, pos2.rect.height);

        return rect1.Overlaps(rect2);//참이면 오버랩된거야
    }

    public void AddBars(string name)//어짜피 아이템 이름일거임
    {
        //스크립트 폴더 추가하기싫어서 좀특이하게 진행될듯
        GameObject temp = Instantiate(ItemBarSample,transform);//일단 얘의이름등을 정의해야하니 이렇게둠
        temp.transform.GetChild(0).GetComponent<Text>().text = name + "을(를) 얻었습니다";

        temp.GetComponent<RectTransform>().localPosition = startPosition;//시작위치잡아둠
        if(bars.Count == 0) bars.Add(temp.GetComponent<RectTransform>());

        else if (bars[0] == null)
        {
            bars[0] = temp.GetComponent<RectTransform>();
        }
        else bars.Add(temp.GetComponent<RectTransform>());
    }

    // Update is called once per frame
    private void Start()
        //메커니즘이 1. 끝까지내려감
        //2.도착도중 이미 앞에간것과 오버랩이된다면멈춤 << rect의좌표가 수정안되
        //3.시간돌기시작함
        //4.중간에 사라지면 마저내려감
        //queue로는 안될듯 < list로함 ,딕셔너리는 id값을 찾아서 하는거고 하니 크게 필요없는듯함
    {
        //        if(startPosition == null)//아마 스타트포지션값이 없을경우
        //      {
        //      startPosition = transform.position;
        startPosition = Vector3.zero;
            
    //    }
        deleteTime = editDeleteTime;
//        transform.position = new Vector3(0,Screen.height);//이리스트 패널의위치를 정의해줌
    }
	void Update () {

        //최대 -1까지 그리고 overlap체크하고, 겸사겸사i번째는 또내려가고
        //마지막에 0번째가 끝나면 i번째값들이 1칸씩옮겨감 << 추가스크립트 필요없게될듯함
        if (bars.Count != 0)
        {
            //우선 얘가 내려가도록
            for (int i = 0; i < bars.Count; i++)
            {
                if (i > 0 && bars.Count >= 1 && CheckOverlap(bars[i], bars[i - 1]))//오버랩되있다면 멈추도록 구성쯤
                    continue;
                if (bars[i].localPosition.y >= -transform.parent.GetComponent<RectTransform>().rect.height +20)//본래 스크린.height였으나 뒤죽박죽이여서 캔버스높이를 땀
                {
                    bars[i].localPosition += Vector3.down * 3f;//todo::빌드하면 여기에 델타타임 붙여야 할지도 모름 유의
                                                               //                bars[i].transform.position += Vector3.down * Time.deltaTime;
                }
            }
            if (bars[0] != null && bars[0].localPosition.y < -transform.parent.GetComponent<RectTransform>().rect.height +20)//끝가지 도착했으면서 bars[0]이 값이 있을경우에만 행한다
            {
                deleteTime -= Time.deltaTime;
                if (deleteTime <= 0f)
                {

                    //            StartCoroutine(Timer());
                    Destroy(bars[0].gameObject);
                    bars.Remove(bars[0]);
                    deleteTime = editDeleteTime;
                }
            }
        }
        //        if (bars[0] == null && bars.Count > 0)//만약 0번째가 없고 길이는 1이상일경우
        

    }
}
