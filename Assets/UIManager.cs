using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager InitUiMng()
    {
        UIManager mng = GameObject.Find("Canvas").GetComponent<UIManager>();
        return mng;
    }

    [SerializeField] Text timeText;

    public void UpdateTimeText(string str)
    {
        if(timeText == null)
        {
            timeText = transform.Find("TimeTxt").GetComponent<Text>();
        }
        timeText.text = str;
    }

}
