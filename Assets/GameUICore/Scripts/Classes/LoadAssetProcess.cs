using UnityEngine;
using System.Collections;
using System;

namespace Game.Core
{
    public class LoadAssetProcess
    {
        public static WaitForEndOfFrame Wait = new WaitForEndOfFrame();
        public string FullName { set; get; }
        public float Process { set; get; }
        public Action<object> FinishHandler { set; get; }
        public object Asset { set; get; }

        ResourceRequest Request = null;

        public bool IsStart
        {
            get {
                return Request != null;
            }
        }

        public UIRoot.Layer Layer { get;  set; }

        public IEnumerator Start()
        {
            ResourceRequest request = Resources.LoadAsync(FullName);
            yield return request;
            yield return Wait;
            Asset = GameObject.Instantiate( request.asset);
            FinishHandler(Asset);
            yield return Wait;
        }
    }
}
