using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class A1Code : MonoBehaviour
{
    public Button m_Start;
    public Button m_Stop;
    public Farmer m_F;
    // Use this for initialization
    void Start()
    {
        m_Start.onClick.AddListener(()=> {
            m_F.m_AI.StartAi();
        });
        m_Stop.onClick.AddListener(() => {
            m_F.m_AI.StopAi();
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
