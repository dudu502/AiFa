using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Farmer : MonoBehaviour
{
    public Text m_TxtCount;
    public int flowerCount = 1;
    public AIBehaviorTree.AI m_AI;
    public RectTransform m_GoHome;
    public RectTransform m_GoWork;
    public RectTransform m_GoShop;
    public RectTransform m_GoGirl;
    // Use this for initialization
    RectTransform m_Rect;

    RectTransform target;
    bool movestate = false;
	void Start ()
    {
        m_Rect = GetComponent<RectTransform>();
        m_AI.Init();
        m_AI.UserData = this;
    }
    public void SendFlower()
    {
        int sendcount = UnityEngine.Random.Range(1, flowerCount);
        flowerCount -= sendcount;
    }
	public void BuySomeFlower()
    {
        int count = UnityEngine.Random.Range(1, 6);
        flowerCount += count;
    }
    public bool HasFlower()
    {
        return flowerCount > 0;
    }
    public void Run2FlowerShop()
    {
        target = m_GoShop;
        movestate = true;
    }

    public void Run2GirlHome()
    {
        target = m_GoGirl;
        movestate = true;
    }
    public void Run2Home()
    {
        target = m_GoHome;
        movestate = true;
    }

    public void Run2Work()
    {
        target = m_GoWork;
        movestate = true;
    }
    
	// Update is called once per frame
	void Update ()
    {
        m_TxtCount.text = flowerCount+"";
        if (movestate)
            Move2Target(target);
	}

    float movespeed = 12;
    private void Move2Target(RectTransform target)
    {
        var dv = target.anchoredPosition - m_Rect.anchoredPosition;
        if(dv.magnitude> movespeed)
            m_Rect.anchoredPosition += movespeed * dv.normalized;
        else
        {
            movestate = false;
            string obj = "";
            if(target == m_GoHome)
            {
                obj = "home";
            }
            else if(target == m_GoWork)
            {
                obj = "work";
            }
            else if(target == m_GoShop)
            {
                obj = "shop";
            }
            else if(target == m_GoGirl)
            {
                obj = "girl";
            }
            m_AI.TriggerFunc("MoveEnd", obj);
        }        
    }
}
