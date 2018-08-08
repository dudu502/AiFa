using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace PathFinding
{

public class AStarMapData
{
    readonly AStarPoint[,] m_MazeMap;
    public AStarMapData(AStarPoint[,] map)
    {
        m_MazeMap = map;
    }

    public AStarPoint[,] GetMapPoints()
    {
        return m_MazeMap;
    }

    public List<AStarNode> GetAStarNodes(Vector3 position,float range)
    {
        List<AStarNode> result = new List<AStarNode>();
        for(int i=0;i<m_MazeMap.GetLength(0);++i)
        {
            for(int j=0;j<m_MazeMap.GetLength(1);++j)
            {
                if(m_MazeMap[i,j].Value!=0)continue;
                float dis = (m_MazeMap[i,j].Position-position).magnitude;
                if(dis<=range)
                {
                    var node = m_MazeMap[i,j].ToNode();
                    result.Add(m_MazeMap[i,j].ToNode());
                }
            }
        }  
        return result;
    }
}
public class AStarMap
{
    public static readonly int OBLIQUE = 14;
    public static readonly int STEP = 10;
    List<AStarNode> CloseList;
    List<AStarNode> OpenList;
    AStarMapData m_MapData;

    public AStarMap(AStarMapData map)
    {
        m_MapData = map;
        CloseList = new List<AStarNode>(map.GetMapPoints().Length);
        OpenList = new List<AStarNode>(map.GetMapPoints().Length);
    }
    public AStarMapData GetMapData(){return m_MapData;}
    public AStarMap Clone()
    {
        return new AStarMap(m_MapData);
    }
    public AStarNode[] GetPath(AStarNode start,AStarNode end,bool IsIgnoreCorner)
    {
        Stack<AStarNode> stackPath = new Stack<AStarNode>();
        if(CloseList.Count>0)CloseList.Clear();
        if(OpenList.Count>0)OpenList.Clear();
        AStarNode result = FindPath(start,end,IsIgnoreCorner);
        while(result!=null)
        {            
            stackPath.Push(result);
            result = result.ParentNode;
        }
      
        return stackPath.Count>0?stackPath.ToArray():null;
    }
    AStarNode FindPath(AStarNode start,AStarNode end,bool IsIgnoreCorner)
    {
        OpenList.Add(start);
        while(OpenList.Count!=0)
        {
            //找出F值最小的节点           
            AStarNode tempStart = OpenList.MinFNode();
            OpenList.Remove(tempStart);
            CloseList.Add(tempStart);

            CalcSurroundNodes(tempStart,IsIgnoreCorner);
            for(int i = 0;i<m_SorroundNodes.Count;++i)
            {
                AStarNode node = m_SorroundNodes[i];
                if(OpenList.Exists(node.X,node.Y))
                    FoundNode(tempStart,node);
                else
                    NotFoundNode(tempStart,end,node);
            }

            AStarNode openNode = OpenList.Get(end);
            if(openNode!=null)
                return openNode;
        }
        return OpenList.Get(end);
    }

    void FoundNode(AStarNode tempStart,AStarNode node)
    {
        int G = CalcG(tempStart,node);
        if(G < node.G)
        {
            node.ParentNode = tempStart;
            node.G = G;
            node.CalcF();
        }
    }
    void NotFoundNode(AStarNode tempStart,AStarNode end,AStarNode node)
    {
        node.ParentNode = tempStart;
        node.G = CalcG(tempStart,node);
        node.H = CalcH(end,node);
        node.CalcF();
        OpenList.Add(node);
    }
    int CalcG(AStarNode start,AStarNode node)
    {
        int G = (Math.Abs(start.X - node.X)+Math.Abs(start.Y-node.Y))==2?STEP:OBLIQUE;
        int ParentG = node.ParentNode != null ? node.ParentNode.G : 0;
        return G + ParentG;
    }
    int CalcH(AStarNode end,AStarNode node)
    {
        return STEP * (Math.Abs(end.X-node.X)+Math.Abs(end.Y-node.Y));
    }

    readonly List<AStarNode> m_SorroundNodes = new List<AStarNode>(9);
    void CalcSurroundNodes(AStarNode node,bool IsIgnoreCorner)
    {
        if(m_SorroundNodes.Count>0)
            m_SorroundNodes.Clear();
        for(int x = node.X - 1;x<=node.X+1;++x)
        {
            for(int y=node.Y-1;y<=node.Y+1;++y)
            {
                if(CanReach(node,x,y,IsIgnoreCorner))
                    m_SorroundNodes.Add(new AStarNode(x,y));
            }
        }       
    }
    bool CanReach(int x,int y)
    {
        return m_MapData.GetMapPoints()[x,y].Value==0;
    }
    bool CanReach(AStarNode start,int x,int y,bool IsIgnoreCorner)
    {
        if(!CanReach(x,y) || CloseList.Exists(x,y))
        {
            return false;
        }
        else
        {
            if(Math.Abs(x - start.X) + Math.Abs(y - start.Y)==1)
            {
                 return true;
            }               
            else
            {
                if(CanReach(Math.Abs(x-1),y) && CanReach(x,Math.Abs(y-1)))
                {
                    return true;
                }
                else
                {
                    return IsIgnoreCorner;
                }
            }
        }
    }
}


public static class ListHelper
{
    public static bool Exists(this List<AStarNode> nodes, int x, int y)
    {
        int count = nodes.Count;
        for(int i=0;i<count;++i)
        {
            AStarNode p = nodes[i];
            if ((p.X == x) && (p.Y == y))
                return true;
        }       
        return false;
    }

    public static AStarNode MinFNode(this List<AStarNode> nodes)
    {
        AStarNode n = nodes[0];
        int count = nodes.Count;
        for(int i=0;i<count;++i)
        {
            if(nodes[i].F<n.F)
                n = nodes[i];
        }
        return n;        
    }
    

    public static AStarNode Get(this List<AStarNode> nodes, AStarNode node)
    {
        int count = nodes.Count;
        for(int i=0;i<count;++i)
        {
            AStarNode p = nodes[i];
            if ((p.X == node.X) && (p.Y == node.Y))
                return p;
        }       
        return null;
    }

   
}
}