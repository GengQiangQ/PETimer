using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// 计时系统
/// 支持时间定时，帧定时
/// 定时任务可循环，可取消，可替换
/// </summary>

public class TimerSys
{
    public static TimerSys Instance;

    PETimer timer;

    /// <summary>
    /// 初始化脚本
    /// </summary>
    public void InitSys()
    {
        Instance = this;
        timer = new PETimer();
        timer.SetLog((info) => Debug.Log(info));
	}

    //需要在Update中注册该方法
    public void TimerSysUpdate()
    {
        timer.TimerSysUpdate();
    }

    #region 时间定时
    /// <summary>
    /// 添加一个时间定时任务
    /// </summary>
    /// <param name="callback">回调</param>
    /// <param name="delayTime">延时时间</param>
    /// <param name="unit">时间单位(默认毫秒)</param>
    /// <param name="count">执行次数(0循环)</param>
    /// <returns>任务ID</returns>
    public int AddTimeTask(Action callback, float delayTime, PETimeUnit unit = PETimeUnit.Millsecond,int count = 1)
    {
       return timer.AddTimeTask(callback, delayTime, unit, count);
    }

   /// <summary>
   /// 删除一个时间定时任务
   /// </summary>
   /// <param name="tid">任务ID</param>
   /// <returns></returns>
    public bool DeleteTimeTask(int tid)
    {
        return timer.DeleteTimeTask(tid);
    }

    /// <summary>
    /// 替换一个已经存在的时间定时任务
    /// </summary>
    /// <param name="tid">任务ID</param>
    /// <param name="callback">回调</param>
    /// <param name="delayTime">延时时间</param>
    /// <param name="unit">时间单位(默认毫秒)</param>
    /// <param name="count">循环次数(0循环)</param>
    /// <returns>替换结果</returns>
    public bool ReplaceTimeTask(int tid, Action callback, float delayTime, PETimeUnit unit = PETimeUnit.Millsecond, int count = 1)
    {
        return timer.ReplaceTimeTask(tid, callback, delayTime, unit, count);
    }

  
    #endregion 

    #region 帧定时

    /// <summary>
    /// 添加一个帧定时任务
    /// </summary>
    /// <param name="callback">回调</param>
    /// <param name="delayTime">定时间隔</param>
    /// <param name="count">循环次数(0循环)</param>
    /// <returns></returns>
    public int AddFrameTask(Action callback, int delayTime,int count = 1)
    {
        return timer.AddFrameTask(callback, delayTime, count);
    }
    
    /// <summary>
    /// 删除一个已经存在的帧定时任务
    /// </summary>
    /// <param name="tid">任务ID</param>
    /// <returns>删除结果</returns>
    public bool DeleteFrameTask(int tid)
    {
        return timer.DeleteFrameTask(tid);
    }

    /// <summary>
    /// 替换一个已经存在的帧定时任务
    /// </summary>
    /// <param name="tid">任务ID</param>
    /// <param name="callback">回调</param>
    /// <param name="delayTime">定时间隔</param>
    /// <param name="count">循环次数(0循环)</param>
    /// <returns>替换结果</returns>
    public bool ReplaceFrameTask(int tid, Action callback, int delayTime, int count = 1)
    {
        return timer.ReplaceFrameTask(tid, callback, delayTime, count);
    }

    #endregion

    #region 倒计时

    /// <summary>
    /// 添加一个倒计时任务
    /// </summary>
    /// <param name="callback">回调</param>
    /// <param name="totalTime">倒计时总时间(单位:秒)</param>
    /// <returns>任务ID</returns>
    public int AddCountdownTask(Action<int> callback, int totalTime)
    {
        return timer.AddCountdownTask(callback, totalTime);
    }

    /// <summary>
    /// 添加一个倒计时任务
    /// </summary>
    /// <param name="callback">回调</param>
    /// <param name="totalTime">倒计时总时间(单位:秒)</param>
    /// <returns>任务ID</returns>
    public int AddCountdownTask(Action<string> callback, int totalTime)
    {

        return timer.AddCountdownTask(callback, totalTime);
    }

    /// <summary>
    /// 添加一个倒计时任务
    /// </summary>
    /// <param name="callback">回调</param>
    /// <param name="totalTime">倒计时总时间(单位:秒)</param>
    /// <returns>任务ID</returns>
    public int AddCountdownTask(Action<int,string> callback, int totalTime)
    {
        return timer.AddCountdownTask(callback, totalTime);
    }

    /// <summary>
    /// 删除一个已经存在的倒计时任务
    /// </summary>
    /// <param name="tid">任务ID</param>
    /// <returns>删除结果</returns>
    public bool DeleteCountdownTask(int tid)
    {
        return timer.DeleteCountdownTask(tid);
    }

    /// <summary>
    /// 替换一个已经存在的倒计时任务
    /// </summary>
    /// <param name="tid">任务ID</param>
    /// <param name="callback">回调(int)</param>
    /// <param name="totalTime">任务总时间</param>
    /// <returns>替换结果</returns>
    public bool ReplaceCountdownTask(int tid, Action<int> callback, int totalTime)
    {
        return timer.ReplaceCountdownTask(tid, callback, totalTime);
    }

    /// <summary>
    /// 替换一个已经存在的倒计时任务
    /// </summary>
    /// <param name="tid">任务ID</param>
    /// <param name="callback">回调(string)</param>
    /// <param name="totalTime">任务总时间</param>
    /// <returns>替换结果</returns>
    public bool ReplaceCountdownTask(int tid, Action<string> callback, int totalTime)
    {
        return timer.ReplaceCountdownTask(tid, callback, totalTime);
    }


    /// <summary>
    /// 替换一个已经存在的倒计时任务
    /// </summary>
    /// <param name="tid">任务ID</param>
    /// <param name="callback">回调(int/string)</param>
    /// <param name="totalTime">任务总时间</param>
    /// <returns>替换结果</returns>
    public bool ReplaceCountdownTask(int tid, Action<int,string> callback, int totalTime)
    {
        return timer.ReplaceCountdownTask(tid, callback, totalTime);
    }

    #endregion
}
