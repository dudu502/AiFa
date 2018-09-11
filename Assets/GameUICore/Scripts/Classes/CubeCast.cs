using UnityEngine;
using System.Collections;

public class CubeCast : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            Ray mRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit mHit;
            if (Physics.Raycast(mRay, out mHit))
            {
                print("cast cube");
            }
        }
       
    }
}
