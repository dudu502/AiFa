using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PathFinding{

/// <summary>
/// AStar Path 
/// </summary>
public class AStar:MonoBehaviour
{ 
    /// <summary>
    /// 异步执行数据结构
    /// </summary>
    public class AyncPathData
    {
        public IList<AStarNode> Paths = null;
        public bool HasPath = true;
    }

    /// <summary>
    /// Next帧
    /// </summary>
    WaitForEndOfFrame m_WaitforEndOfFrame;
    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    void Awake()
    {
        m_WaitforEndOfFrame = new WaitForEndOfFrame();
        _ins=this;
    }
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        
    }

    /// <summary>
    /// 单例
    /// </summary>
    private static AStar _ins;
    public static AStar GetInstance()
    {
        return _ins;
    }

    /// <summary>
    /// 根据坐标同步寻路
    /// </summary>
    /// <param name="map"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="range"></param>
    /// <param name="IsIgnoreCorner"></param>
    /// <returns></returns>
    public IList<AStarNode> FindPathSync(AStarMap map,Vector3 start,Vector3 end , float range,bool IsIgnoreCorner)
    {
        IList<AStarNode> startNodes = map.GetMapData().GetAStarNodes(start,range);
        IList<AStarNode> endNodes = map.GetMapData().GetAStarNodes(end,range);
        if(startNodes.Count==0||endNodes.Count==0)
            return null;
        return FindPathSync(map,startNodes[0],endNodes[0],IsIgnoreCorner);
    }

    /// <summary>
    /// 同步寻路
    /// </summary>
    /// <param name="map"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="IsIgnoreCorner"></param>
    /// <returns></returns>
    public IList<AStarNode> FindPathSync(AStarMap map, AStarNode start,AStarNode end,bool IsIgnoreCorner)
    {
        return map.GetPath(start,end,IsIgnoreCorner);
    }
    
    
    /// <summary>
    /// 异步寻路
    /// </summary>
    /// <param name="map"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="IsIgnoreCorner"></param>
    /// <param name="OnComplete"></param>
    public void FindPathAsyn(AStarMap map,AStarNode start,AStarNode end,bool IsIgnoreCorner, Action< IList<AStarNode> > OnComplete)
    {
        StartCoroutine(FindPathAsynImpl(map,start,end,IsIgnoreCorner,OnComplete));
    }

    /// <summary>
    /// 异步坐标寻路
    /// </summary>
    /// <param name="map"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="range"></param>
    /// <param name="IsIgnoreCorner"></param>
    /// <param name="OnComplete"></param>
    public void FindPathAsyn(AStarMap map,Vector3 start,Vector3 end,float range,bool IsIgnoreCorner,Action<IList<AStarNode>> OnComplete)
    {
        StartCoroutine(FindPathAsynImpl(map,start,end,range,IsIgnoreCorner,OnComplete));
    }
    
    /// <summary>
    /// 异步寻路采用线程池管理
    /// </summary>
    /// <param name="map"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="IsIgnoreCorner"></param>
    /// <param name="OnComplete"></param>
    /// <returns></returns>
    IEnumerator FindPathAsynImpl(AStarMap map,AStarNode start,AStarNode end,bool IsIgnoreCorner, Action< IList<AStarNode> > OnComplete)
    {
        AyncPathData data = new AyncPathData();        
        ThreadPool.QueueUserWorkItem((state)=>
        {
            AyncPathData stateData = state as AyncPathData;
            stateData.Paths = map.GetPath(start,end,IsIgnoreCorner);
        },data);
        while(data.Paths == null)
            yield return m_WaitforEndOfFrame;
        if(OnComplete != null)
            OnComplete(data.Paths);
    }

    /// <summary>
    /// 异步坐标点具体实现
    /// </summary>
    /// <param name="map"></param>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <param name="range"></param>
    /// <param name="IsIgnoreCorner"></param>
    /// <param name="OnComplete"></param>
    /// <returns></returns>
    IEnumerator FindPathAsynImpl(AStarMap map,Vector3 start,Vector3 end,float range,bool IsIgnoreCorner,Action<IList<AStarNode>> OnComplete)
    {
        AyncPathData data = new AyncPathData();        
        ThreadPool.QueueUserWorkItem((state)=>
        {
            AyncPathData stateData = state as AyncPathData;
            IList<AStarNode> startNodes = map.GetMapData().GetAStarNodes(start,range);
            IList<AStarNode> endNodes = map.GetMapData().GetAStarNodes(end,range);
            if(startNodes.Count == 0||endNodes.Count==0)
            {
                stateData.HasPath = false;
            }               
            else
            {
                stateData.Paths = map.GetPath(startNodes[0],endNodes[0],IsIgnoreCorner);
                if(stateData.Paths == null)
                    stateData.HasPath=false;
            }
                
        },data);

        while(data.HasPath && data.Paths == null)
            yield return m_WaitforEndOfFrame;
        if(OnComplete != null)
            OnComplete(data.Paths);
    }
}
}
