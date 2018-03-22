using UnityEngine;
using System.Collections;
using System;

namespace AIBehaviorTree
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property)]
    public class AIDocAttribute : Attribute
    {
        public string m_Doc;
        public AIDocAttribute(string doc)
        {
            m_Doc = doc;            
        }
    }
}