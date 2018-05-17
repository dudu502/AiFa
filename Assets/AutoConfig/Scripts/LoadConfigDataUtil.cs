using UnityEngine;
using System.Collections;
using System;


public class LoadConfigDataUtil
{
    public class TypePair
    {
        public Type m_TypeMain;
        public Type m_TypeChild;

        public TypePair(Type mtm, Type mtc)
        {
            m_TypeChild = mtc;
            m_TypeMain = mtm;
        }

        public string GetFinalName()
        {
            return m_TypeMain.ToString().Replace("BinaryConfig", "_Final");
        }
    }
    public static TypePair[] s_Types = new TypePair[]
    {
        new TypePair(typeof(CFG_FallsLevelBinaryConfig),typeof(CFG_FallsLevelBinaryConfig.CFG_FallsLevel)),
        new TypePair(typeof(CFG_FallsScoreBinaryConfig),typeof(CFG_FallsScoreBinaryConfig.CFG_FallsScore)),
        new TypePair(typeof(CFG_ItemBinaryConfig),typeof(CFG_ItemBinaryConfig.CFG_Item)),
        new TypePair(typeof(CFG_AchievementBinaryConfig),typeof(CFG_AchievementBinaryConfig.CFG_Achievement)),
    };


    public static void Load()
    {
        LoadConfigImpl<CFG_FallsLevelBinaryConfig, CFG_FallsLevelBinaryConfig.CFG_FallsLevel>();
        LoadConfigImpl<CFG_FallsScoreBinaryConfig, CFG_FallsScoreBinaryConfig.CFG_FallsScore>();
        LoadConfigImpl<CFG_ItemBinaryConfig, CFG_ItemBinaryConfig.CFG_Item>();
        LoadConfigImpl<CFG_AchievementBinaryConfig, CFG_AchievementBinaryConfig.CFG_Achievement>();
    }

    static void LoadConfigImpl<M, C>()
    {
        TypePair tp = GetTypeByClass<M>();
        TextAsset result = Resources.Load("StaticConfigs/" + tp.GetFinalName()) as TextAsset;
        object config = tp.m_TypeMain.GetMethod("Read").Invoke(null, new object[] { result.bytes });
        IDictionary dic = tp.m_TypeMain.GetField("dict").GetValue(config) as IDictionary;
        ConfigData.AddConfig2Dict<C>(dic);

        IList list = tp.m_TypeMain.GetField("items").GetValue(config) as IList;
        ConfigData.AddConfig2List<C>(list);
    }



    public static TypePair GetTypeByClass<T>()
    {
        foreach (TypePair t in s_Types)
        {
            if (t.m_TypeMain == typeof(T))
                return t;
        }
        return null;
    }

    public static TypePair GetTypeByName(string className)
    {
        foreach (TypePair t in s_Types)
        {
            if (t.m_TypeMain.Name == className)
                return t;
        }
        return null;
    }
}


