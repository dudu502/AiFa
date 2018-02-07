using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using System.Xml;
using LitJson;
using UnityEngine;

namespace AIBehaviorTree
{
    public class AI:MonoBehaviour
    {
        //public TextAsset xml;
        public TextAsset m_JsonAiTree;
        
        public float m_AutoRestartIntervalSecs;
        public List<string> m_StrExecutings = new List<string>();
        AINode m_Root = null;
        List<AINode> m_ListExecutingNodes = new List<AINode>();

        private bool m_BlAiActive = false;
        void Start()
        {   
            
        }

        public object UserData { get; set; }
        public void Init()
        {
            /*
            var list = new List<AINodeConfigData>(); 
            XmlDocument x = new XmlDocument();
            x.LoadXml(xml.text);
            DataInput.ParseXML(list, typeof(AINodeConfigData),x.GetElementsByTagName("Root")[0].ChildNodes);
            AINode node = AINodeConfigData.CreateNode(null, "", list[0]);
            print(node);
            node.Enter();
            */

            //m_Root = AINodeConfigData.CreateNode(null, JsonMapper.ToObject<JsonData>(m_JsonAiTree.text), this);

            m_Root = AINodeConfigData.Create(JsonMapper.ToObject<JsonData>(m_JsonAiTree.text), this);
            m_Root.OnExitHandler = OnAiExitHandler;
            //StartAi();
        }
         
        void OnAiExitHandler(bool exitAll)
        {
            if(!exitAll)
                StartCoroutine(_CoroutineRestart());
        }

        IEnumerator _CoroutineRestart()
        {
            yield return new WaitForSeconds(m_AutoRestartIntervalSecs);
            StartAi();
        }

        public void TriggerFunc(Enum type, object obj)
        {
            TriggerFunc(type.ToString(), obj);
        }
        public void TriggerFunc(string type, object obj)
        {
            for (int i = m_ListExecutingNodes.Count - 1; i > -1; --i)
            {
                if (m_ListExecutingNodes[i] != null)
                    m_ListExecutingNodes[i].TriggerFunc(type, obj);
            }
        }

        [ContextMenu("Start AI")]
        public void StartAi()
        {
            m_BlAiActive = true;
            m_Root.Enter();    
        }

        [ContextMenu("Stop AI")]
        public void StopAi()
        {
            m_BlAiActive = false;
            m_Root.Exit(true);
        }

        [ContextMenu("Pause AI")]
        public void PauseAi()
        {
            m_BlAiActive = false;
        }

        [ContextMenu("Resume AI")]
        public void ResumeAi()
        {
            m_BlAiActive = true;
        }

        void Update()
        {
            m_StrExecutings.Clear();
            if (m_BlAiActive && m_ListExecutingNodes.Count>0)
            {              
                for (int i = m_ListExecutingNodes.Count - 1; i > -1; --i)
                {
                    if (m_ListExecutingNodes[i] != null)
                    {
                        m_StrExecutings.Add(m_ListExecutingNodes[i].ToString());
                        m_ListExecutingNodes[i].Update();
                    }
                }
            }
        }
        public void SetExecutingNode(AINode node)
        {
            if (!m_ListExecutingNodes.Contains(node))
                m_ListExecutingNodes.Add(node);              
        }
        public void RemoveExecutingNode(AINode node)
        {
            if (m_ListExecutingNodes.Contains(node))
                m_ListExecutingNodes.Remove(node);
        }


        void OnDestroy()
        {
            m_ListExecutingNodes.Clear();
        }
    }
}