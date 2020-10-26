
using System;
using System.Collections.Generic;

public class PETimer 
{
    public Action<string> taskLog;

    //时间定时任务列表
    private List<PETimeTask> taskTimeList = new List<PETimeTask>();
    //时间定时任务缓存列表
    private List<PETimeTask> tmpTimeList = new List<PETimeTask>();

    //帧定时计数器
    int frameCounter;
    //帧定时任务列表
    private List<PEFrameTask> taskFrameList = new List<PEFrameTask>();
    //帧定时缓存列表
    private List<PEFrameTask> tmpFrameList = new List<PEFrameTask>();

    //倒计时任务列表
    private List<PECountdownTask> taskCountdownList = new List<PECountdownTask>();
    //倒计时缓存列表
    private List<PECountdownTask> tmpTaskCountdownList = new List<PECountdownTask>();

    //tid锁
    private static readonly string lockObj = "lock";
    private int tid;
    //定时任务的所有Tid
    private List<int> tidList = new List<int>();
    //需要回收的Tid数组
    private List<int> recTidList = new List<int>();

    private DateTime startDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
    private double nowTime;

    public PETimer()
    {
        taskTimeList.Clear();
        tmpTimeList.Clear();

        taskFrameList.Clear();
        tmpFrameList.Clear();

        taskCountdownList.Clear();
        tmpTaskCountdownList.Clear();

        tidList.Clear();
        recTidList.Clear();
    }

    //需要在Update中注册该方法
    public void TimerSysUpdate()
    {
        CheckTimeTask();

        CheckFrameTask();

        CheckCountdownTask();

        //清理已经完成的任务的Tid
        if (recTidList.Count > 0) RecycleTid();
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
    public int AddTimeTask(Action callback, float delayTime, PETimeUnit unit = PETimeUnit.Millsecond, int count = 1)
    {
        //时间换算为毫秒
        if (unit != PETimeUnit.Millsecond)
        {
            switch (unit)
            {
                case PETimeUnit.Second:
                    delayTime = delayTime * 1000;
                    break;
                case PETimeUnit.Minute:
                    delayTime = delayTime * 1000 * 60;
                    break;
                case PETimeUnit.Hour:
                    delayTime = delayTime * 1000 * 60 * 60;
                    break;
                case PETimeUnit.Day:
                    delayTime = delayTime * 1000 * 60 * 60 * 24;
                    break;
                default:
                    break;
            }
        }

        int tid = GetTid();
        tidList.Add(tid);

        // Time.realtimeSinceStartup 从游戏启动到现在为止的时间
        nowTime = GetUTCMilliseconds() + delayTime;
        PETimeTask tmpTask = new PETimeTask(tid, nowTime, delayTime, count, callback);
        tmpTimeList.Add(tmpTask);

        return tid;
    }

    /// <summary>
    /// 删除一个时间定时任务
    /// </summary>
    /// <param name="tid">任务ID</param>
    /// <returns></returns>
    public bool DeleteTimeTask(int tid)
    {
        bool exist = false;

        for (int i = 0; i < taskTimeList.Count; i++)
        {
            if (tid == taskTimeList[i].tid)
            {
                exist = true;
                taskTimeList.RemoveAt(i);

                for (int j = 0; j < tidList.Count; j++)
                {
                    if (tid == tidList[j])
                    {
                        tidList.RemoveAt(j);
                        break;
                    }
                }
                break;
            }
        }

        if (!exist)
        {
            for (int i = 0; i < tmpTimeList.Count; i++)
            {
                if (tid == tmpTimeList[i].tid)
                {
                    exist = true;
                    tmpTimeList.RemoveAt(i);

                    for (int j = 0; j < tidList.Count; j++)
                    {
                        if (tid == tidList[j])
                        {
                            tidList.RemoveAt(j);
                            break;
                        }
                    }
                    break;
                }
            }
        }

        return exist;
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
        bool result = false;

        //时间换算为毫秒
        if (unit != PETimeUnit.Millsecond)
        {
            switch (unit)
            {
                case PETimeUnit.Second:
                    delayTime = delayTime * 1000;
                    break;
                case PETimeUnit.Minute:
                    delayTime = delayTime * 1000 * 60;
                    break;
                case PETimeUnit.Hour:
                    delayTime = delayTime * 1000 * 60 * 60;
                    break;
                case PETimeUnit.Day:
                    delayTime = delayTime * 1000 * 60 * 60 * 24;
                    break;
                default:
                    break;
            }
        }


        // Time.realtimeSinceStartup 从游戏启动到现在为止的时间
        nowTime = GetUTCMilliseconds() + delayTime;
        PETimeTask newTask = new PETimeTask(tid, nowTime, delayTime, count, callback);

        for (int i = 0; i < taskTimeList.Count; i++)
        {
            if (newTask.tid == taskTimeList[i].tid)
            {
                result = true;
                taskTimeList[i] = newTask;
                break;
            }
        }

        if (!result)
        {
            for (int i = 0; i < tmpTimeList.Count; i++)
            {
                if (newTask.tid == tmpTimeList[i].tid)
                {
                    result = true;
                    tmpTimeList[i] = newTask;
                    break;
                }
            }
        }

        return result;
    }

    private void CheckTimeTask()
    {
        //加入上一帧的缓存区的定时任务
        for (int tmpIndex = 0; tmpIndex < tmpTimeList.Count; tmpIndex++)
        {
            taskTimeList.Add(tmpTimeList[tmpIndex]);
        }
        tmpTimeList.Clear();

        //遍历检测定时任务是否达到条件
        for (int i = 0; i < taskTimeList.Count; i++)
        {
            PETimeTask task = taskTimeList[i];
            nowTime = GetUTCMilliseconds();
            if (nowTime < task.destTime) continue;
            else
            {
                Action cb = task.callback;
                try
                {
                    if (cb != null) cb();
                }
                catch (Exception e)
                {
                    Log(e.ToString());
                }

                //移除已经完成的定时任务
                if (task.count == 1)
                {
                    recTidList.Add(taskTimeList[i].tid);
                    taskTimeList.RemoveAt(i);
                    i--;
                }
                else
                {
                    if (task.count != 0)
                    {
                        task.count--;
                    }
                    task.destTime += task.delayTime;
                }
            }
        }
    }

    #endregion 

    #region 帧定时

    private void CheckFrameTask()
    {
        //加入上一帧的缓存区的定时任务
        for (int tmpIndex = 0; tmpIndex < tmpFrameList.Count; tmpIndex++)
        {
            taskFrameList.Add(tmpFrameList[tmpIndex]);
        }
        tmpFrameList.Clear();

        frameCounter += 1;

        //遍历检测定时任务是否达到条件
        for (int i = 0; i < taskFrameList.Count; i++)
        {
            PEFrameTask task = taskFrameList[i];

            if (frameCounter < task.destFrame) continue;
            else
            {
                Action cb = task.callback;
                try
                {
                    if (cb != null) cb();
                }
                catch (Exception e)
                {
                    Log(e.ToString());
                }

                //移除已经完成的定时任务
                if (task.count == 1)
                {
                    recTidList.Add(taskFrameList[i].tid);
                    taskFrameList.RemoveAt(i);
                    i--;
                }
                else
                {
                    if (task.count != 0)
                    {
                        task.count--;
                    }
                    task.destFrame += task.delay;
                }
            }
        }
    }

    /// <summary>
    /// 添加一个帧定时任务
    /// </summary>
    /// <param name="callback">回调</param>
    /// <param name="delayTime">定时间隔</param>
    /// <param name="count">循环次数(0循环)</param>
    /// <returns></returns>
    public int AddFrameTask(Action callback, int delayTime, int count = 1)
    {

        int tid = GetTid();
        tidList.Add(tid);

        PEFrameTask tmpTask = new PEFrameTask(tid, frameCounter + delayTime, delayTime, count, callback);
        tmpFrameList.Add(tmpTask);

        return tid;
    }

    /// <summary>
    /// 删除一个已经存在的帧定时任务
    /// </summary>
    /// <param name="tid">任务ID</param>
    /// <returns>删除结果</returns>
    public bool DeleteFrameTask(int tid)
    {
        bool exist = false;

        for (int i = 0; i < taskFrameList.Count; i++)
        {
            if (tid == taskFrameList[i].tid)
            {
                exist = true;
                taskFrameList.RemoveAt(i);

                for (int j = 0; j < tidList.Count; j++)
                {
                    if (tid == tidList[j])
                    {
                        tidList.RemoveAt(j);
                        break;
                    }
                }
                break;
            }
        }

        if (!exist)
        {
            for (int i = 0; i < tmpFrameList.Count; i++)
            {
                if (tid == tmpFrameList[i].tid)
                {
                    exist = true;
                    tmpFrameList.RemoveAt(i);

                    for (int j = 0; j < tidList.Count; j++)
                    {
                        if (tid == tidList[j])
                        {
                            tidList.RemoveAt(j);
                            break;
                        }
                    }
                    break;
                }
            }
        }

        return exist;
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
        bool result = false;


        PEFrameTask newTask = new PEFrameTask(tid, frameCounter + delayTime, delayTime, count, callback);

        for (int i = 0; i < taskFrameList.Count; i++)
        {
            if (newTask.tid == taskFrameList[i].tid)
            {
                result = true;
                taskFrameList[i] = newTask;
                break;
            }
        }

        if (!result)
        {
            for (int i = 0; i < tmpFrameList.Count; i++)
            {
                if (newTask.tid == tmpFrameList[i].tid)
                {
                    result = true;
                    tmpFrameList[i] = newTask;
                    break;
                }
            }
        }

        return result;
    }

    #endregion

    #region 倒计时
    private void CheckCountdownTask()
    {
        //加入上一帧的缓存区的定时任务
        for (int tmpIndex = 0; tmpIndex < tmpTaskCountdownList.Count; tmpIndex++)
        {
            taskCountdownList.Add(tmpTaskCountdownList[tmpIndex]);
        }
        tmpTaskCountdownList.Clear();

        //遍历检测定时任务是否达到条件
        for (int i = 0; i < taskCountdownList.Count; i++)
        {
            PECountdownTask task = taskCountdownList[i];
            nowTime = GetUTCMilliseconds();
            if (nowTime < task.destTime) continue;
            else
            {
                Action<int> cb = task.intCallback;
                Action<string> cb_str = task.strCallback;
                try
                {
                    if (cb != null) cb(task.totalTime/1000);
                    if (cb_str != null) cb_str(GetDateTime(task.totalTime/1000));
                }
                catch (Exception e)
                {
                    Log(e.ToString());
                }

                //移除已经完成的定时任务
                if (task.totalTime <= 0)
                {
                    recTidList.Add(task.tid);
                    taskCountdownList.RemoveAt(i);
                    i--;
                }
                else
                {
                    if (task.totalTime > 0)
                    {
                        task.totalTime-=1000;
                    }
                    task.destTime += task.delayTime;
                }
            }
        }
    }

    /// <summary>
    /// 添加一个倒计时任务
    /// </summary>
    /// <param name="callback">回调</param>
    /// <param name="totalTime">倒计时总时间</param>
    /// <returns>任务ID</returns>
    public int AddCountdownTask(Action<int> callback, int totalTime)
    {

        int tid = GetTid();
        tidList.Add(tid);

        PECountdownTask tmpTask = new PECountdownTask(tid, totalTime*1000, callback);
        nowTime = GetUTCMilliseconds();
        tmpTask.destTime = nowTime + tmpTask.delayTime;
        tmpTaskCountdownList.Add(tmpTask);

        Log("添加了一个倒计时任务,Tid = " + tid);
        return tid;
    }

    /// <summary>
    /// 添加一个倒计时任务
    /// </summary>
    /// <param name="callback">回调</param>
    /// <param name="totalTime">倒计时总时间</param>
    /// <returns>任务ID</returns>
    public int AddCountdownTask(Action<string> callback, int totalTime)
    {

        int tid = GetTid();
        tidList.Add(tid);

        PECountdownTask tmpTask = new PECountdownTask(tid, totalTime*1000, callback);
        nowTime = GetUTCMilliseconds();
        tmpTask.destTime = nowTime + tmpTask.delayTime;
        tmpTaskCountdownList.Add(tmpTask);
        Log("添加了一个倒计时任务,Tid = " + tid);
        return tid;
    }

    /// <summary>
    /// 添加一个倒计时任务
    /// </summary>
    /// <param name="callback">回调</param>
    /// <param name="totalTime">倒计时总时间</param>
    /// <returns>任务ID</returns>
    public int AddCountdownTask(Action<int, string> callback, int totalTime)
    {

        int tid = GetTid();
        tidList.Add(tid);

        PECountdownTask tmpTask = new PECountdownTask(tid, totalTime*1000, callback);
        nowTime = GetUTCMilliseconds();
        tmpTask.destTime = nowTime + tmpTask.delayTime;
        tmpTaskCountdownList.Add(tmpTask);
        Log("添加了一个倒计时任务,Tid = " + tid);
        return tid;
    }

    /// <summary>
    /// 删除一个已经存在的倒计时任务
    /// </summary>
    /// <param name="tid">任务ID</param>
    /// <returns>删除结果</returns>
    public bool DeleteCountdownTask(int tid)
    {
        bool exist = false;

        for (int i = 0; i < taskCountdownList.Count; i++)
        {
            if (tid == taskCountdownList[i].tid)
            {
                exist = true;
                taskCountdownList.RemoveAt(i);

                for (int j = 0; j < tidList.Count; j++)
                {
                    if (tid == tidList[j])
                    {
                        tidList.RemoveAt(j);
                        break;
                    }
                }
                break;
            }
        }

        if (!exist)
        {
            for (int i = 0; i < tmpTaskCountdownList.Count; i++)
            {
                if (tid == tmpTaskCountdownList[i].tid)
                {
                    exist = true;
                    tmpTaskCountdownList.RemoveAt(i);

                    for (int j = 0; j < tidList.Count; j++)
                    {
                        if (tid == tidList[j])
                        {
                            tidList.RemoveAt(j);
                            break;
                        }
                    }
                    break;
                }
            }
        }
        return exist;
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
        bool result = false;


        PECountdownTask newTask = new PECountdownTask(tid, totalTime*1000, callback);
        nowTime = GetUTCMilliseconds();
        newTask.destTime = nowTime + newTask.delayTime;
        for (int i = 0; i < taskCountdownList.Count; i++)
        {
            if (newTask.tid == taskCountdownList[i].tid)
            {
                result = true;
                taskCountdownList[i] = newTask;
                break;
            }
        }

        if (!result)
        {
            for (int i = 0; i < tmpTaskCountdownList.Count; i++)
            {
                if (newTask.tid == tmpTaskCountdownList[i].tid)
                {
                    result = true;
                    tmpTaskCountdownList[i] = newTask;
                    break;
                }
            }
        }

        return result;
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
        bool result = false;


        PECountdownTask newTask = new PECountdownTask(tid, totalTime*1000, callback);
        nowTime = GetUTCMilliseconds();
        newTask.destTime = nowTime + newTask.delayTime;
        for (int i = 0; i < taskCountdownList.Count; i++)
        {
            if (newTask.tid == taskCountdownList[i].tid)
            {
                result = true;
                taskCountdownList[i] = newTask;
                break;
            }
        }

        if (!result)
        {
            for (int i = 0; i < tmpTaskCountdownList.Count; i++)
            {
                if (newTask.tid == tmpTaskCountdownList[i].tid)
                {
                    result = true;
                    tmpTaskCountdownList[i] = newTask;
                    break;
                }
            }
        }

        return result;
    }


    /// <summary>
    /// 替换一个已经存在的倒计时任务
    /// </summary>
    /// <param name="tid">任务ID</param>
    /// <param name="callback">回调(int/string)</param>
    /// <param name="totalTime">任务总时间</param>
    /// <returns>替换结果</returns>
    public bool ReplaceCountdownTask(int tid, Action<int, string> callback, int totalTime)
    {
        bool result = false;


        PECountdownTask newTask = new PECountdownTask(tid, totalTime*1000, callback);
        nowTime = GetUTCMilliseconds();
        newTask.destTime = nowTime + newTask.delayTime;
        for (int i = 0; i < taskCountdownList.Count; i++)
        {
            if (newTask.tid == taskCountdownList[i].tid)
            {
                result = true;
                taskCountdownList[i] = newTask;
                break;
            }
        }

        if (!result)
        {
            for (int i = 0; i < tmpTaskCountdownList.Count; i++)
            {
                if (newTask.tid == tmpTaskCountdownList[i].tid)
                {
                    result = true;
                    tmpTaskCountdownList[i] = newTask;
                    break;
                }
            }
        }

        return result;
    }

    #endregion

    #region 工具方法
    /// <summary>
    /// 获取唯一的一个定时任务ID
    /// </summary>
    /// <returns></returns>
    int GetTid()
    {
        lock (lockObj)
        {
            tid += 1;

            while (true)
            {
                if (tid == int.MaxValue) tid = 0;

                bool used = false;
                for (int i = 0; i < tidList.Count; i++)
                {
                    if (tid == tidList[i])
                    {
                        used = true;
                        tid += 1;
                        break;
                    }
                }
                if (!used) break;
            }
        }
        return tid;
    }

    /// <summary>
    /// 回收Tid
    /// </summary>
    void RecycleTid()
    {
        for (int j = 0; j < recTidList.Count; j++)
        {
            for (int k = 0; k < tidList.Count; k++)
            {
                if (recTidList[j] == tidList[k])
                {
                    tidList.RemoveAt(k);
                    break;
                }
            }
        }

        recTidList.Clear();
    }

    /// <summary>
    /// 获取时间格式的字符串
    /// </summary>
    /// <param name="second">总秒数</param>
    /// <returns></returns>
    string GetDateTime(int second)
    {
        string str = string.Empty;
        int hour = 0;
        int minute = 0;

        if (second > 60)
        {
            minute = second / 60;
            second = second % 60;
        }
        if (minute > 60)
        {
            hour = minute / 60;
            minute = minute % 60;
        }
        return hour.ToString().PadLeft(2, '0') + ":" + minute.ToString().PadLeft(2, '0') + ":"
            + second.ToString().PadLeft(2, '0');

    }

    public void SetLog(Action<string>log)
    {
        taskLog = log;
    }

    private void Log(string log)
    {
        if (taskLog != null) taskLog(log);
    }

    private double GetUTCMilliseconds()
    {
        //UtcNow 世界标准时间  now本地时间
        TimeSpan ts = DateTime.UtcNow - startDateTime;
        return ts.TotalMilliseconds;
    }

    #endregion

}