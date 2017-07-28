using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorChange : MonoBehaviour {
    
    public List<Texture2D> cursorTexture;
    public int cursorNumber;
//    public bool Center = false;//텍스처중심을 좌표로?
    public Vector2 adjustHotspot = Vector2.zero;

    private Vector2 hotSpot;//사용할필드선언
	// Use this for initialization
	void Start () {
  	
	}
//	IEnumerator MyCursor()
 //   {
//        yield return new WaitForEndOfFrame();//렌더링 끝날때까지 대기후리턴
 //       if(Center)
  //  }
	// Update is called once per frame
	void Update () {
	}
    public void ChangeCursor(int num)
    {
        if(num == -1)
        {
            Cursor.SetCursor(null, Vector2.zero,CursorMode.Auto);
        }
        else if (cursorTexture[num] != null)
        {
            Cursor.SetCursor(cursorTexture[num], Vector2.zero,
                CursorMode.Auto);
        }
        

    }
}
