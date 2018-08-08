using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using PathFinding;
using UnityEngine;
using UnityEngine.UI;
public class CirclePointSprite:MonoBehaviour
{
    public Action<CirclePointSprite> OnMouseUpHandler;
    /// <summary>
    /// OnMouseUp is called when the user has released the mouse button.
    /// </summary>
    void OnMouseUp()
    {
        if(OnMouseUpHandler!=null)
            OnMouseUpHandler(this);
    }
}
