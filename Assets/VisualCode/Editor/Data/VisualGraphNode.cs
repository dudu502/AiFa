using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
namespace VisualCoding
{
    public class VisualGraphNode
    {
        public static readonly string[] TYPES = new string[] 
        {
            typeof(int).ToString(),
            typeof(float).ToString(),
            typeof(short).ToString(),
            typeof(string).ToString(),
        };
        public enum NodeType
        {
            Variable,
            Function,
            Event,
            Notification,
        }
        public enum AccessAuthority
        {
            Private,
            Protected,
            Public,
        }
  

        public AccessAuthority m_AccessAuthority = AccessAuthority.Private;
        public NodeType m_NodeType = NodeType.Variable;

        public NodeDataBase m_DataBase;
        NodeDataBase m_Output;
        List<NodeDataBase> m_Inputs = new List<NodeDataBase>();
        public void AddInput(NodeDataBase data)
        {
            m_Inputs.Add(data);
        }
        public void SetOutput(NodeDataBase data)
        {
            m_Output = data;
        }
        public VisualGraphNode()
        {

        }
    }

    public class NodeDataBase
    {

        public Rect m_Rect = new Rect();
        public virtual void Draw(int id)
        {
            
        }
 

        public virtual string ToTitle()
        {
            return "";
        }
    }
    public class VariableData: NodeDataBase
    {
        public string m_DataType;
        public string m_DataName;

        public override void Draw(int id)
        {
            base.Draw(id);
            m_DataType = VisualGraphNode.TYPES[EditorGUILayout.Popup("Type", Array.IndexOf<string>(VisualGraphNode.TYPES, m_DataType), VisualGraphNode.TYPES,GUILayout.ExpandWidth(true))];
            m_DataName = EditorGUILayout.TextField("Name", m_DataName);
            GUI.DragWindow(new Rect(0, 0, 1000, 20));
        }
        public override string ToTitle()
        {
            return "Variable";
        }
    }
    public class FunctionData : NodeDataBase
    {
        public string m_ReturnType;
        public string m_FunctionName;
        public List<VariableData> m_ParamsTypeDatas;

        public override void Draw(int id)
        {
            base.Draw(id);
        }
    }
    public class EventData : NodeDataBase
    {
        public List<VariableData> m_ParamsTypeDatas;
        public string m_EventFunctionName;

        public override void Draw(int id)
        {
            base.Draw(id);
        }
    }
    public class NotificationData : NodeDataBase
    {
        public VariableData m_ParamsTypeData;
        public string m_NotificationName;

        public override void Draw(int id)
        {
            base.Draw(id);
        }
    }

}
