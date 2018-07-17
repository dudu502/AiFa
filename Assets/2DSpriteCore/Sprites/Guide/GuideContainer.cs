using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuideContainer : MonoBehaviour {
    public Rect m_RectTarget;
    public List<Image> m_Imgs;
    public RectTransform m_Target;
    public Camera m_CameraUI;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        UpdateTargetRect();
        UpdatePoses();
    }
    void UpdateTargetRect()
    {
        var pos = m_CameraUI.WorldToScreenPoint(m_Target.position);
        m_RectTarget.position = pos;
        m_RectTarget.size = m_Target.rect.size;
        
    }

    void UpdatePoses()
    {
        SetImgPosSize(m_Imgs[0].rectTransform, 0, 0, m_RectTarget.x, m_RectTarget.y);
        SetImgPosSize(m_Imgs[1].rectTransform, m_RectTarget.x, 0, m_RectTarget.width, m_RectTarget.y);
        SetImgPosSize(m_Imgs[2].rectTransform, m_RectTarget.x + m_RectTarget.width, 0, Screen.width - m_RectTarget.x - m_RectTarget.width, m_RectTarget.y);

        SetImgPosSize(m_Imgs[3].rectTransform, 0, m_RectTarget.y, m_RectTarget.x, m_RectTarget.height);
        SetImgPosSize(m_Imgs[4].rectTransform, m_RectTarget.x + m_RectTarget.width, m_RectTarget.y, Screen.width - m_RectTarget.x - m_RectTarget.width, m_RectTarget.height);

        SetImgPosSize(m_Imgs[5].rectTransform, 0, m_RectTarget.y + m_RectTarget.height, m_RectTarget.x, Screen.height - m_RectTarget.y - m_RectTarget.height);
        SetImgPosSize(m_Imgs[6].rectTransform, m_RectTarget.x, m_RectTarget.y + m_RectTarget.height, m_RectTarget.width, Screen.height - m_RectTarget.y - m_RectTarget.height);
        SetImgPosSize(m_Imgs[7].rectTransform, m_RectTarget.x + m_RectTarget.width, m_RectTarget.y + m_RectTarget.height, Screen.width - m_RectTarget.x - m_RectTarget.width, Screen.height - m_RectTarget.y - m_RectTarget.height);

    }
    void SetImgPosSize(RectTransform trans, float x,float y,float w,float h)
    {
        trans.anchoredPosition = new Vector2(x, y);
        trans.sizeDelta = new Vector2(w, h);
    }
}
