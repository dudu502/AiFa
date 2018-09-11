using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System;

public class TestCast : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        print("click test cast");
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
