using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Observer;
namespace Game.Core
{
    public class UIRoot : MonoBehaviour
    {
        public enum Layer
        {
            Game,
            System,
            Alert,
        }

        public enum NotificationType
        {
            EnqueueAProcess,
            CurrentProcessInfoUpdate,
            AddActivity,
            RemoveActivity,
        }
        public GameObject m_LayerGame;
        public GameObject m_LayerSystem;
        public GameObject m_LayerAlert;

        Dictionary<Layer, List<Activity>> m_DictActivity;
        WaitForEndOfFrame m_WaitNextFrame;
        Notifier m_Notifier;
        Queue<LoadAssetProcess> m_QueueProcess;
        // Use this for initialization
        private void Awake()
        {
            m_DictActivity = new Dictionary<Layer, List<Activity>>();
            m_DictActivity[Layer.Game] = new List<Activity>();
            m_DictActivity[Layer.System] = new List<Activity>();
            m_WaitNextFrame = new WaitForEndOfFrame();
            m_QueueProcess = new Queue<LoadAssetProcess>();
            m_Notifier = new Notifier(this);
            m_Notifier.Add(NotificationType.EnqueueAProcess,OnEnqueueAProcessHandler);
            m_Notifier.Add(NotificationType.AddActivity, OnAddActivityHandler);
            m_Notifier.Add(NotificationType.RemoveActivity, OnRemoveActivityHandler);
        }
        void OnAddActivityHandler(Notification note)
        {
            Activity act = note.Params[0] as Activity;
            AddActivity(act);
        }
        void OnRemoveActivityHandler(Notification note)
        {
            Activity act = note.Params[0] as Activity;
            RemoveActivity(act);
        }
        void AddActivity(Activity activity)
        {
            if(IsActivityActive(activity))
            {
                m_DictActivity[activity.GetLayer()].Remove(activity);
            }
            m_DictActivity[activity.GetLayer()].Add(activity);
            DisplayActivity(activity.GetLayer());
        }

        void RemoveActivity(Activity activity)
        {
            if(IsActivityActive(activity))
            {
                m_DictActivity[activity.GetLayer()].Remove(activity);
            }
            DisplayActivity(activity.GetLayer());
        }

        void DisplayActivity(Layer layer)
        {
            for(int i=m_DictActivity[layer].Count-1;i>-1;--i)
            {
                Activity acti = m_DictActivity[layer][i];
                acti.SetActive(i== m_DictActivity[layer].Count - 1);
            }
        }

        bool IsActivityActive(Activity activity)
        {
            return m_DictActivity[activity.GetLayer()].Contains(activity);
        }
        /// <summary>
        /// 添加一个加载进程
        /// </summary>
        /// <param name="note"></param>
        void OnEnqueueAProcessHandler(Notification note)
        {
            LoadAssetProcess newProcess = note.Params[0] as LoadAssetProcess;
            
            bool state = false;
            foreach (var process in m_QueueProcess)
            {
                if (process.FullName == newProcess.FullName)
                {
                    state = true;
                    break;
                }                   
            }
            if(!state)
            {
                m_QueueProcess.Enqueue(newProcess);
            }
        }
        void Start()
        {
            StartCoroutine(_LoadElements());
        }
        IEnumerator _LoadElements()
        {
            while(true)
            {
                if(m_QueueProcess.Count>0)
                {
                    LoadAssetProcess process = m_QueueProcess.Peek();
                    if (!process.IsStart)
                    {
                        yield return process.Start();

                        AddLayer(process.Asset as GameObject, process.Layer);
                        
                        m_QueueProcess.Dequeue();

                    }
                }
                yield return m_WaitNextFrame;
            }
        }

        void AddLayer(GameObject obj,Layer layer)
        {
            GameObject root;
            if (layer == Layer.Game)
                root = m_LayerGame;
            else if (layer == Layer.System)
                root = m_LayerSystem;
            else
                root = m_LayerAlert;
            obj.transform.SetParent(root.transform);
            obj.transform.localPosition = Vector3.one;
            obj.transform.localRotation = Quaternion.identity;

        }

        // Update is called once per frame
        void Update()
        {

        }




        [ContextMenu("TestAdd")]
        void Test()
        {
            Activity.Create(new TestActivity());

            Activity.Active<TestActivity>();
        }

        [ContextMenu("TestRemove")]
        void Test1()
        {
            //Activity.Create(new TestActivity());

            Activity.Inactive<TestActivity>();
        }
    }
}