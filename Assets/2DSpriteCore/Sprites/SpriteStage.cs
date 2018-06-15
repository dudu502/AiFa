using UnityEngine;
using System.Collections;

public class SpriteStage : MonoBehaviour
{
    public SpriteElement m_Root;
    // Use this for initialization
    void Start()
    {
        StartCoroutine(_delay());
    }
    IEnumerator _delay()
    {
        var sprite = Resources.Load("SpriteRect") as GameObject;
        yield return new WaitForSeconds(1);
        var s = GameObject.Instantiate(sprite).GetComponent<SpriteElement>();
        m_Root.Add(s,new Vector2(0,0));
        s.Add(GameObject.Instantiate(sprite).GetComponent<SpriteElement>(), new Vector2(0, 0.1f));
        yield return new WaitForSeconds(1);
        s = GameObject.Instantiate(sprite).GetComponent<SpriteElement>();
        m_Root.Add(s, new Vector2(0.1f, 0));
        yield return new WaitForSeconds(1);
        s = GameObject.Instantiate(sprite).GetComponent<SpriteElement>();
        m_Root.Add(s, new Vector2(0.2f, 0));
    }
    // Update is called once per frame
    void Update()
    {
        SpriteElement.UpdateZSort(m_Root);
    }
}
