using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using PathFinding;
using UnityEngine;
using UnityEngine.UI;
public class AStarDemo:MonoBehaviour
{
    public GameObject m_Rect;
    public Texture2D m_Txt;
    /// <summary>
    /// Start is called on the frame when a script is enabled just before
    /// any of the Update methods is called the first time.
    /// </summary>
    void Start()
    {
        int[,] array = new int[(int)128,(int)64];
        
        
        for(int i = 0;i<array.GetLength(0);++i)
        {
            for(int j=0;j<array.GetLength(1);++j)
            {
                var obj = GameObject.Instantiate(m_Rect);
                obj.SetActive(true);
                obj.transform.SetParent(transform);
                obj.transform.localScale=Vector3.one;
                obj.GetComponent<RectTransform>().anchoredPosition=new Vector2(i*10,j*10);
                var img = obj.GetComponent<Image>();
                array[i,j] = m_Txt.GetPixel(i,j) == Color.white?0:1;
                img.color =array[i,j]==0?Color.white:Color.black;       
                
            }
        }
        
                           
        AStarMap map = new AStarMap(new AStarMapData(array));
        float t = Time.time;
        //var rst = map.GetPath(new AStarNode(1,1),new AStarNode(6,10),false);

        AStar.GetInstance().FindPathAsyn(map,new AStarNode(10,10),new AStarNode(115,32),true,(e)=>
        {
            print("1cost time "+(Time.time-t));
            StartCoroutine(showPath(e));      
        });      

        AStar.GetInstance().FindPathAsyn(map.Clone(),new AStarNode(11,9),new AStarNode(115,8),true,(e)=>
        {
            print("2cost time "+(Time.time-t));
            StartCoroutine(showPath(e));      
        });       

        AStar.GetInstance().FindPathAsyn(map.Clone(),new AStarNode(17,23),new AStarNode(66,8),true,(e)=>
        {
            print("3cost time "+(Time.time-t));
            StartCoroutine(showPath(e));      
        });    

        
        AStar.GetInstance().FindPathAsyn(map.Clone(),new AStarNode(66,7),new AStarNode(15,22),true,(e)=>
        {
            print("4cost time "+(Time.time-t));
            StartCoroutine(showPath(e));      
        }); 

        AStar.GetInstance().FindPathAsyn(map.Clone(),new AStarNode(115,31),new AStarNode(11,11),true,(e)=>
        {
            print("5cost time "+(Time.time-t));
            StartCoroutine(showPath(e));      
        }); 
    }
    IEnumerator showPath(IList<AStarNode> list){
        var obj = GameObject.Instantiate(m_Rect);
        obj.SetActive(true);
        obj.transform.SetParent(transform);
        obj.transform.localScale=Vector3.one;
        
        var img = obj.GetComponent<Image>();
        img.color = Color.red;
        foreach(var i in list){
            yield return new WaitForSeconds(0.1f);
            img.GetComponent<RectTransform>().anchoredPosition = new Vector2(10*i.X,10*i.Y);
        }
    }
}
