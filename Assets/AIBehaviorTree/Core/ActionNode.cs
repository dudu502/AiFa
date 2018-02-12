using UnityEngine;

namespace AIBehaviorTree
{
    public class ActionNode:AINode
    {
        /// <summary>
        /// 正在执行中
        /// </summary>
        protected bool m_IsExecuting = false;

        public ActionNode( AI ai,AINodeConfigData config):base( ai, config)
        {
            
        }

        public override void Update(float dt)
        {
            base.Update(dt);
            if (m_BlActive&&m_UpdateAction != null) 
                m_UpdateAction(dt);
        }

        public override void TriggerFunc(string type, object obj)
        {
            base.TriggerFunc(type, obj);
            if (m_BlActive&&m_TriggerAction != null)
                m_TriggerAction(type, obj);
        }

        public override void Enter()
        {
            base.Enter();
            if (m_DetectAction())
            {
                if (m_EnterAction != null)
                {
                    SetExecuting(true);
                    m_EnterAction();
                }                 
            }
            else
            {
                Break();
            }
        }

        public override bool GetExecuting()
        {
            return m_IsExecuting;            
        }
        private void SetExecuting(bool value)
        {
            if (value)
                m_Ai.SetExecutingNode(this);
            else
                m_Ai.RemoveExecutingNode(this);
        }

        public override void Exit(bool exitAll)
        {
            SetExecuting(false);
            base.Exit(exitAll);        
        }

        public override string ToString()
        {
            return string.Format("[{0}:{1}]", "ActionNode" , Config.ToString());
        }
    }
}