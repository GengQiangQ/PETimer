using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameRoot : MonoBehaviour {

    TimerSys timerSys;

    UIManager uiMng;

    int tid;

    void Start ()
    {
        timerSys = new TimerSys();
        timerSys.InitSys();
        uiMng = UIManager.InitUiMng();
	}
	
	void Update ()
    {
        timerSys.TimerSysUpdate();
	}

    public void OnClickBtnAdd()
    {
        Debug.Log("Delay start");
        tid = TimerSys.Instance.AddTimeTask(FuncA,500f,PETimeUnit.Millsecond,0);
    }

    public void OnClickBtnReplace()
    {
        TimerSys.Instance.ReplaceTimeTask(tid,FuncB, 1000f, PETimeUnit.Millsecond, 3);
    }

    void FuncA()
    {
        Debug.Log("Delay Log tid = " + tid);
    }

    void FuncB()
    {
        Debug.Log("Replace Task Done. tid = " +tid);
    }

    public void OnClickBtnDel()
    {
       
       bool ret = TimerSys.Instance.DeleteTimeTask(tid);

        Debug.Log("Delay Del : " + ret);
    }

    public void OnClickBtnFrameAdd()
    {
        Debug.Log("Delay frame start ");
        tid = TimerSys.Instance.AddFrameTask(FuncFrameA, 50, 0);
    }

    public void OnClickBtnFrameReplace()
    {
        TimerSys.Instance.ReplaceFrameTask(tid, FuncB, 10, 3);
    }

    void FuncFrameA()
    {
        Debug.Log("Delay Frame Log tid = " + tid);
    }

    void FuncFrameB()
    {
        Debug.Log("Replace Frame Task Done. tid = " + tid);
    }

    public void OnClickBtnFrameDel()
    {

        bool ret = TimerSys.Instance.DeleteFrameTask(tid);

        Debug.Log("Delay Frame Del : " + ret);
    }

    public void OnClickBtnCntdownAdd()
    {
        Debug.Log("Delay countdown start ");
        tid = TimerSys.Instance.AddCountdownTask(FuncCntdownA, 50);
    }

    public void OnClickBtnCntdownReplace()
    {
        TimerSys.Instance.ReplaceCountdownTask(tid, FuncCntdownB, 20000);
    }

    void FuncCntdownA(int totalTime)
    {
        Debug.Log("Delay countdown Log totalTime = " + totalTime);
    }

    void FuncCntdownB(string totalTime)
    {
        Debug.Log("Replace Cntdown Task Done. totalTime = " + totalTime);
        uiMng.UpdateTimeText(totalTime);
    }

    public void OnClickBtnCntdownDel()
    {

        bool ret = TimerSys.Instance.DeleteCountdownTask(tid);

        Debug.Log("Delay cntdown Del : " + ret);
    }

}
