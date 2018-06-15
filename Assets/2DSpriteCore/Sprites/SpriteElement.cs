using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteElement : MonoBehaviour {

    private SpriteRenderer m_SpriteRenderer;
    private List<SpriteElement> m_Children;
    private void Awake()
    {       
        m_Children = new List<SpriteElement>();
        m_SpriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void Add(SpriteElement element,Vector2 localPos)
    {
        if (m_Children.IndexOf(element) > -1)
            m_Children.Remove(element);
        m_Children.Add(element);
        element.transform.SetParent(transform);
        element.transform.localPosition = localPos;
        element.transform.localRotation = Quaternion.identity;
    }
    public void Remove(SpriteElement element)
    {
        if (m_Children.IndexOf(element) > -1)
            m_Children.Remove(element);
        GameObject.Destroy(element.gameObject);
    }

    public int GetSortOrder() { return m_SpriteRenderer.sortingOrder; }
    public void SetSortOrder(int order) { m_SpriteRenderer.sortingOrder = order; }
    public static int UpdateZSort(SpriteElement rootElement)
    {
        int order = rootElement.GetSortOrder();
        for (int i = 0; i < rootElement.m_Children.Count; ++i)
        {
            SpriteElement child = rootElement.m_Children[i];
            child.SetSortOrder(++order);
            order = UpdateZSort(child);
        }
        return order;
    }
    // Use this for initialization
    void Start ()
    {


    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
