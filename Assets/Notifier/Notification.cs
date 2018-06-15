using System;
namespace Observer
{
    public class Notification
    {
        public Enum Type { set; get; }

        public Object[] Params { set; get; }

        public Object Target { set; get; }

        public Notification()
        {

        }
    }
}

