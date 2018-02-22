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
        /// <summary>
        /// 树导出路径
        /// </summary>
        public const string TREE_OUTPUTPATH = "/AIBehaviorTree/Resources/tree/";
        /// <summary>
        /// 脚本导出路径
        /// </summary>
        public const string SCRIPT_OUTPUTPATH = "/AIBehaviorTree/Resources/";
        /// <summary>
        /// json树结构数据
        /// </summary>
        public TextAsset m_JsonAiTree;        
        /// <summary>
        /// 自动重新开始时间间隔（秒）
        /// </summary>
        public float m_AutoRestartIntervalSecs;
        /// <summary>
        /// 更新时间间隔（秒）
        /// </summary>
        public float m_UpdateIntervalSecs = 0.1f;       
       
        float m_UpdatePassedSecs = 0;
        AINode m_Root = null;
        List<AINode> m_ListExecutingNodes = new List<AINode>();
        bool m_BlAiActive = false;
        void Start()
        {   
            
        }

        public object UserData { get; set; }
        public void Init()
        {
            m_Root = AINodeConfigData.Create(JsonMapper.ToObject<JsonData>(m_JsonAiTree.text), this);
            m_Root.OnExitHandler = OnAiExitHandler;
        }
         
        void OnAiExitHandler(bool exitAll)
        {
            m_BlAiActive = false;
            if (!exitAll)
                StartCoroutine(_CoroutineRestart());
        }

        IEnumerator _CoroutineRestart()
        {
            yield return new WaitForSeconds(m_AutoRestartIntervalSecs);
            StartAi();
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
            m_UpdatePassedSecs += Time.deltaTime;
            if (m_UpdatePassedSecs >= m_UpdateIntervalSecs)
            {
                if (m_BlAiActive && m_ListExecutingNodes.Count > 0)
                {
                    for (int i = m_ListExecutingNodes.Count - 1; i > -1; --i)
                    {
                        if (m_ListExecutingNodes[i] != null)
                        {
                            m_ListExecutingNodes[i].Update(m_UpdatePassedSecs);
                        }
                    }
                }
                m_UpdatePassedSecs = 0;
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

        public List<AINode> GetExecutingNodes()
        {
            return m_ListExecutingNodes;
        }

        void OnDestroy()
        {
            m_ListExecutingNodes.Clear();
        }
    }
}