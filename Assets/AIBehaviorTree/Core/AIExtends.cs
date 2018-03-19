using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace AIBehaviorTree
{
    public static class AIExtends
    {

        
        public static object ExecuteMember(this object obj, string pathName,params object[] paramObjs)
        {
            string[] subs = pathName.Split('.');
            return obj.FindMemberData(subs, 0,paramObjs);     
        }

        static object FindMemberData(this object obj,string[] pathPoints, int index, params object[] paramObjs)
        {
            string name = pathPoints[index];
            FieldInfo field = obj.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.GetField);
            if (field != null)
            {
                if (index == pathPoints.Length - 1)
                    return field.GetValue(obj);
                else
                    return field.GetValue(obj).FindMemberData(pathPoints, ++index,paramObjs);
            }
            else
            {   
                PropertyInfo prop = obj.GetType().GetProperty(name);
                if(prop != null)
                {
                    if (index == pathPoints.Length - 1)
                        return prop.GetValue(obj,null);      
                    else
                        return prop.GetValue(obj,null).FindMemberData(pathPoints, ++index,paramObjs);
                }
                else
                {
                    MethodInfo method = obj.GetType().GetMethod(name,BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);
                    if(method != null)
                    {
                        if(index == pathPoints.Length - 1)
                            return method.Invoke(obj,paramObjs);
                        else
                            return obj.FindMemberData(pathPoints, ++index,paramObjs);
                    }
                    else{
                        throw new Exception("Error");
                    }
                }                        
            }
        }
        

    }























}












