using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace AIBehaviorTree
{
    public class NodeHelpData
    {
        public class FuncHelpData
        {
            public MethodInfo Func ;
            public string FuncDescribe = "";
  
        }
        public class FieldHelpData
        {
            public FieldInfo Field;
            public string FieldDescribe = "";
      
        }
        public class PropertyHelpData
        {
            public PropertyInfo Property;
            public string PropertyDescribe = "";
          
        }
        public string ClassName = "";
        public string ClassDescribe = "";

        public List<FuncHelpData> Funcs = new List<FuncHelpData>();
        public List<FieldHelpData> Fields = new List<FieldHelpData>();
        public List<PropertyHelpData> Propertys = new List<PropertyHelpData>();

    }

}