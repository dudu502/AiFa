using UnityEngine;
using System.Collections;
using System;
using System.Threading;

namespace ECS
{
    public class System 
    {
        public enum State
        {
            NonUpdate,
            Updating,
            Updated,
        }
        public State m_State = State.NonUpdate;
        public virtual void Init()
        {
            
        }
        public virtual void Update(object state)
        {
            m_State = State.NonUpdate;
            Execute();
            Updated(state);
        }

        protected virtual void Updated(object state)
        {
            m_State = State.Updated;
        }

        public virtual void Execute()
        {
            m_State = State.Updating;
        }

        public virtual void End()
        {

        }
    }
    public class PrioritySystem : System
    {
        
    }

    public class LogicSystem:System
    {
        protected override void Updated(object state)
        {
            base.Updated(state);
            ((MutipleThreadResetEvent)state).SetOne();
        }
    }
    public class RenderSystem : System
    {

    }

}