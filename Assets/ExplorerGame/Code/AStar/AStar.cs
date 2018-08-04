using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PathFinding{


public class AStar:MonoBehaviour
{ 
    public class AyncPathData
    {
        public IList<AStarNode> Paths = null;
    }
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

    private static AStar _ins;
    public static AStar GetInstance()
    {
        return _ins;
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
}
}
