using System;
using Random = UnityEngine.Random;

namespace AIBehaviorTree
{
    public class ControlNode:AINode
    {
        public ControlNode( AI ai,AINodeConfigData config):base( ai, config)
        {
            
        }
         
        public override void Enter()
        {
            base.Enter();
            if (m_DetectAction())
            {
                if (m_EnterAction != null)
                    m_EnterAction();
                switch (Config.type)
                {                 
                    case NODE_TYPE_SEQUENCE:
                        m_Children[m_ChildSeqIndex].Enter();
                        break;
                    case NODE_TYPE_RANDOMSELECT:
                        GetRandomSelectChildNode().Enter();
                        break;
                    case NODE_TYPE_PAIALLEL:
                        m_Children.ForEach((c)=>c.Enter());                      
                        break;                        
                    default:
                        throw new System.Exception("Need TODO");
                }             
            }
            else
            {
                Break();               
            }
        }

        AINode GetRandomSelectChildNode()
        {
            int randomWeight = Random.Range(0, m_IntChildWeight);
            int addValue = 0;
            for (int i = 0; i < m_Children.Count; ++i)
            {
                if (addValue <= randomWeight && randomWeight < (addValue += m_Children[i].Config.weight))
                {
                    return m_Children[i];
                }
            }
            throw new Exception("random error");
        }

        public override string ToString()
        {
            return string.Format("[{0}:{1}]", "ControlNode", Config.ToString());
        }
    }
}