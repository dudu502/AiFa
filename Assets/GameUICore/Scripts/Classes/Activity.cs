using UnityEngine;
using System.Collections;
namespace Game.Core
{
    public class Activity : Actor
    {
        public static void Active<T>()where T:Activity
        {
            T act = Get<T>();
            if (act.m_State == State.None)
            {
                act.OnCreate();
            }
            else if(act.m_State == State.Create)
            {

            }
            else
            {
                act.Send(UIRoot.NotificationType.AddActivity, act);
            }
        }

        public static void Inactive<T>()where T:Activity
        {
            T act = Get<T>();
            act.Send(UIRoot.NotificationType.RemoveActivity, act);
        }

        public enum State
        {
            None = 0,
            Create = 1,
            Inited = 2,
            Enabled = 3,
            Disabled = 4,
        }
        State m_State = State.None;
        public Activity()
        {

        }

       
        public virtual UIRoot.Layer GetLayer()
        {
            return UIRoot.Layer.Game;
        }

        public virtual void OnCreate()
        {
            m_State = State.Create;
            LoadAssets();
        }
        protected virtual string GetActivityFullName()
        {
            return "";
        }

        GameObject m_Asset = null;
        public bool HasAsset()
        {
            return m_Asset != null;
        }
        void LoadAssets()
        {
            LoadAssetProcess Process = new LoadAssetProcess();
            Debug.Log("Load");
            Process.FullName = GetActivityFullName();
            Process.Layer = GetLayer();
            Process.FinishHandler = (asset) => 
            {
                m_Asset = asset as GameObject;
                Init();
            };
            Send(UIRoot.NotificationType.EnqueueAProcess, Process);
        }

        void Init()
        {
            Send(UIRoot.NotificationType.AddActivity, this);
            OnInit();
            m_State = State.Inited;
            OnInited();
            Debug.Log("init");
        }

        protected virtual void OnInit()
        {

        }
        protected virtual void OnInited()
        {

        }

        public virtual void OnEnable()
        {
            m_State = State.Enabled;
        }
      

        public virtual void OnDisable()
        {
            m_State = State.Disabled;
        }
 

        public void RemoveAsset()
        {
            m_State = State.None;
            if(HasAsset())
            {
                GameObject.Destroy(m_Asset);
                m_Asset = null;
            }
        }
        
        public void SetActive(bool value)
        {
            if (!HasAsset()) return;
            m_Asset.SetActive(value);
            if(value)
            {
                if(m_State!=State.Enabled)OnEnable();
            }
            else
            {
                if(m_State!=State.Disabled)OnDisable();
            }
        }
    }
}
