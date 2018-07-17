using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;

namespace VisualCode
{


    public class GetVarNode : VisualNode
    {
        public GetVarNode()
        {

        }
        public GetVarNode(Vector2 pos) : base(pos)
        {
            SetRectSize(new Vector2(80, 40));
            Title = "Get";
            for (int i = 0; i < 1; ++i)
            {
                var field = new FieldNode(i, this);
                field.InRect.Enable = false;
                field.Name = " ";
                fields.Add(field);
            }
            currentFlow = new FlowNode(this);
        }
        protected override void DrawWindowFunc(int id)
        {
            base.DrawWindowFunc(id);
            GUILayout.BeginVertical();
            foreach (var field in fields)
            {
                GUILayout.BeginHorizontal();
                field.Domain = (AccessNode.DomainMode)EditorGUILayout.EnumPopup(field.Domain);
                field.Type = EditorGUILayout.Popup(field.Type, FieldNode.S_FIELDS);
                List<FieldNode> targets= OnFieldGetInfo(field, AccessNode.AccessGetInfoMode.GetVar);
                if (targets.Count > 0)
                {
                    List<string> names = new List<string>() { " " };
                    targets.ForEach((tf)=> { names.Add(tf.Name); });
                    if (names.IndexOf(field.Name) > -1)
                        field.Name = names[EditorGUILayout.Popup(names.IndexOf(field.Name), names.ToArray())];                                                  
                    else
                        field.Name = names[EditorGUILayout.Popup(0, names.ToArray())];
                    field.Value = "";
                }
                
                field.Value = EditorGUILayout.TextField(field.Value);               
                
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 1000, 20));
        }

        protected override Color GetNodeColor()
        {
            return Color.cyan;
        }

   

        public override NodeType GetNodeType()
        {
            return NodeType.GetVar;
        }

        public static byte[] Write(GetVarNode node)
        {
            ByteBuffer bfs = VisualNode.Write(node);
            return bfs.Getbuffer();
        }
        public static GetVarNode Read(byte[] bytes)
        {
            GetVarNode node = new GetVarNode();
            ByteBuffer bfs = new ByteBuffer(bytes);
            VisualNode.Read(bfs, node);
            return node;
        }
    }
}