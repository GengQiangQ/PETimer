using System;
/// <summary>
/// 时间定时任务数据类
/// </summary>
public class PETimeTask
{
    public int tid;

    public double delayTime;
    //延时的时间 单位:毫秒
    public double destTime;
    //要执行的任务
    public Action callback;
    //执行次数 0循环
    public int count;

    public PETimeTask(int tid, double destTime, double delayTime,int count,Action callback)
    {
        this.tid = tid;
        this.destTime = destTime;
        this.delayTime = delayTime;
        this.count = count;
        this.callback = callback;
    }

}

/// <summary>
/// 时间计时任务类时间单位
/// </summary>
public enum PETimeUnit
{
    Millsecond,
    Second,
    Minute,
    Hour,
    Day
}

/// <summary>
/// 帧定时任务数据类
/// </summary>
public class PEFrameTask
{
    public int tid;

    public int delay;
    //延时帧
    public int destFrame;
    //要执行的任务
    public Action callback;
    //执行次数 0循环
    public int count;

    public PEFrameTask(int tid, int destFrame, int delay, int count, Action callback)
    {
        this.tid = tid;
        this.destFrame = destFrame;
        this.delay = delay;
        this.count = count;
        this.callback = callback;
    }
}

/// <summary>
/// 倒计时任务类
/// </summary>
public class PECountdownTask
{
    public int tid;

    public double delayTime = 1000.0;

    public double destTime;

    public int totalTime;
  
    public Action<int> intCallback;

    public Action<string> strCallback;

    //int 时间(单位秒) str 格式化时间(时:分:秒)
    public Action<int, string> secondAndTimeCallback;

    //public int count;

    public PECountdownTask(int tid, int totalTime, Action<int> callback)
    {
        this.tid = tid;
        this.totalTime = totalTime;
        this.intCallback = callback;
        if (this.strCallback != null) this.strCallback = null;
    }

    public PECountdownTask(int tid, int totalTime, Action<string> callback)
    {
        this.tid = tid;
        this.totalTime = totalTime;
        this.strCallback = callback;
        if (this.intCallback != null) this.intCallback = null;
    }

    public PECountdownTask(int tid, int totalTime, Action<int,string> callback)
    {
        this.tid = tid;
        this.totalTime = totalTime;
        this.secondAndTimeCallback = callback;
        if (this.intCallback != null) this.intCallback = null;
        if (this.strCallback != null) this.strCallback = null;
    }
}



