using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Collections;

public class DataScriptObjectBase : ScriptableObject
{
    public List<DataItemBase> ListItem;

    public void Sort()
    {
        ListItem.Sort((a, b) => a.ID - b.ID);
    }
}
public class DataItemBase
{
    public int ID;

    public T GetFieldValue<T>(string name)
    {
        return (T)GetType().GetField(name).GetValue(this);
    }
   
    public void SetFieldValue(string name,object value)
    {
        GetType().GetField(name).SetValue(this, value);
    }


    public override string ToString()
    {
        string str = "";
        foreach (var f in GetType().GetFields())
            str += f.Name + ":" + f.GetValue(this)+"  ";
        return str;
    }
}

