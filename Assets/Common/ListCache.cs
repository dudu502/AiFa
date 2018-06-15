using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ListCache 
{
    static Dictionary<Type, Queue> pool = new Dictionary<Type, Queue>();
    public static List<T> GetList<T>()
    {
        if (!pool.ContainsKey(typeof(T)))
        {
            pool.Add(typeof(T), new Queue());
        }
        if (pool[typeof(T)].Count == 0)
        {
            pool[typeof(T)].Enqueue(new List<T>());
        }
        return pool[typeof(T)].Dequeue() as List<T>;
    }

    public static void PushList<T>(List<T> list)
    {
        list.Clear();
        if (!pool.ContainsKey(typeof(T)))
        {
            pool.Add(typeof(T), new Queue());
        }
        pool[typeof(T)].Enqueue(list);
    }
   
    public static void Reset()
    {
        pool.Clear();
    }
}
