using System;
namespace Observer
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Subscribe : Attribute
    {
        protected Object m_Type { set; get; }
        public Subscribe(Object type)
        {
            m_Type = type;
        }
        public Enum GetSubscription()
        {
            return (Enum)m_Type;
        }
    }
}
