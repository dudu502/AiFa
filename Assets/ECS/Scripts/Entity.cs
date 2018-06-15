using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ECS
{
    public class Entity 
    {
        public Action<Entity> OnDispose;
        public Action<Entity, Component> OnAddComponent;
        public Action<Entity, Component> OnRemoveComponent;
        private List<Component> m_Components;
        public Entity()
        {
            m_Components = ListCache.GetList<Component>();
        }

        public void Add(Component c)
        {
            if (Contain(c))
                return;
            c.Entity = this;
            m_Components.Add(c);
            m_Components.Sort((a, b) => a.Pri-b.Pri);
            if (OnAddComponent != null)
                OnAddComponent(this, c);
        }

        public bool Contain(Component c)
        {
            return m_Components.Contains(c);
        }


        public bool Remove(Component c)
        {
            if (!Contain(c)) return false;
            c.Entity = null;
            bool rst = m_Components.Remove(c);
            if (rst && OnRemoveComponent!=null)
                OnRemoveComponent(this, c);
            return rst;
        }
        public List<Component> Remove<T>() where T : Component
        {
            List<Component> rst = ListCache.GetList<Component>();
            for (int i = m_Components.Count - 1; i > -1; --i)
            {
                if(typeof(T) == m_Components[i].GetType())
                {
                    Remove(m_Components[i]);
                    rst.Add(m_Components[i]);
                }
            }
            return rst;
        }      
        
        public void Dispose()
        {
            if (OnDispose != null)
            {
                OnDispose(this);
            }
            ListCache.PushList<Component>(m_Components);
        }
    }
}