using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Observer;
namespace Game.Core
{
    public class Actor : Notifier
    {
        static Dictionary<Type, Actor> s_Dict = new Dictionary<Type, Actor>();
        public static void Create(Actor ac)
        {
            if (!s_Dict.ContainsKey(ac.GetType()))
                s_Dict.Add(ac.GetType(), ac);
        }
        public static T Get<T>() where T : Actor
        {
            Type tp = typeof(T);
            if (s_Dict.ContainsKey(tp))
                return (T)s_Dict[tp];
            throw new Exception("Create");
        }

        public Actor()
        {
            
        }
    }

}
