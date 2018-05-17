using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class ConfigData 
{
    private static Dictionary<Type, IDictionary> _DictConfig = new Dictionary<Type, IDictionary>();
    private static Dictionary<Type, IList> _DictList = new Dictionary<Type, IList>();
    public static void AddConfig2List<T>(IList list)
    {
        if (_DictList.ContainsKey(typeof(T)))
            throw new Exception("Error");
        _DictList[typeof(T)] = list;
    }

    public static List<T> GetConfigList<T>()
    {
        if (!_DictList.ContainsKey(typeof(T)))
            throw new Exception("Null");
        IList list = _DictList[typeof(T)];
        return (List<T>)list;
    }

    /// <summary>
    /// 添加配置字典
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="d"></param>
    public static void AddConfig2Dict<T>(IDictionary d)
    {
        if (_DictConfig.ContainsKey(typeof(T)))
            throw new Exception("Error");
        _DictConfig[typeof(T)] = d;
    }

    /// <summary>
    /// 根据类型获取配置数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="id"></param>
    /// <returns></returns>
    public static T GetConfigFromDict<T>(int id)
    {
        if (!_DictConfig.ContainsKey(typeof(T)))
            throw new Exception("NULL");
        IDictionary d = _DictConfig[typeof(T)];
        return (T)(d[id]);
    }

    public static IDictionary<int, T> GetConfigDict<T>()
    {
        if (!_DictConfig.ContainsKey(typeof(T)))
            throw new Exception("NULL");
        return (IDictionary<int, T>)_DictConfig[typeof(T)];
    }
}
