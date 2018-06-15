using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

namespace ECS
{
    public class Component
    {
        public Entity Entity
        {
            get;set;
        }
        public int Pri
        {
            private set;
            get;
        }
        public Component(int pri)
        {
            Pri = pri;
        }

        public virtual void Dispose()
        {
            if (Entity != null)
                Entity.Remove(this);
        }
    }
}