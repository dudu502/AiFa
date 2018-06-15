using UnityEngine;
using System.Collections.Generic;
using System.Threading;

namespace ECS
{
    public class ECS : MonoBehaviour
    {
        List<PrioritySystem> m_PriortySystems;
        MutipleThreadResetEvent m_ThreadEvtPriorityReset;
        List<LogicSystem> m_LogicSystems;
        MutipleThreadResetEvent m_ThreadEvtLogicReset;
        RenderSystem m_RenderSystem;
        // Use this for initialization
        void Start()
        {
            m_PriortySystems = new List<PrioritySystem>();
            m_PriortySystems.Add(new PrioritySystem());
            m_LogicSystems = new List<LogicSystem>();
            
            InitSystems();
        }

        private void InitSystems()
        {       
            m_RenderSystem = new RenderSystem();
            m_ThreadEvtPriorityReset = new MutipleThreadResetEvent(1);
            m_LogicSystems.Add(new LogicSystem());
            m_LogicSystems.Add(new LogicSystem());
            m_ThreadEvtLogicReset = new MutipleThreadResetEvent(m_LogicSystems.Count);
        }

        // Update is called once per frame
        void Update()
        {
            ThreadPool.QueueUserWorkItem(BeforeHandSystemsUpdater, m_ThreadEvtPriorityReset);            
            m_ThreadEvtPriorityReset.WaitAll();
            m_ThreadEvtPriorityReset.Reset();
            for (int i = 0; i < m_LogicSystems.Count; ++i)
                ThreadPool.QueueUserWorkItem(m_LogicSystems[i].Update, m_ThreadEvtLogicReset);
            m_ThreadEvtLogicReset.WaitAll();
            m_ThreadEvtLogicReset.Reset();
            m_RenderSystem.Update(Time.deltaTime);       
        }
        void BeforeHandSystemsUpdater(object state)
        {
            for (int i = 0; i < m_PriortySystems.Count; ++i)
                m_PriortySystems[i].Update(state);
            ((MutipleThreadResetEvent)state).SetOne();
        }       
    }
}