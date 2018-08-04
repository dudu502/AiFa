using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding{

public class AStarNode
{    
    public int X{get;set;}
    public int Y{get;set;}
    public int F{get;set;}
    public int G{get;set;}
    public int H{get;set;}

    public AStarNode ParentNode{get;set;}
    public AStarNode(int xparam,int yparam)
    {
        X = xparam;
        Y = yparam;
    }
    override public string ToString()
    {
        return string.Format("{0},{1}",X,Y);
    }
    public void CalcF()
    {
        F = G+H;
    }

}
}
