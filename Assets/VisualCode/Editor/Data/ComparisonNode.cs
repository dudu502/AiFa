using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VisualCode
{
    public class ComparisonNode : VisualNode
    {
        public ComparisonNode() { }
        public ComparisonNode(Vector2 pos):base(pos)
        {
            SetRectSize(new Vector2(80, 40));
            for (int i = 0; i < 8; ++i)
            {
                var field = new FieldNode(i, this);
                if (i<2)
                {               
                    field.OutRect.Enable = false;                 
                }
                else
                {
                    field.InRect.Enable = false;
                }
                fields.Add(field);
            }
            currentFlow = new FlowNode(this);
        }
        protected override void DrawWindowFunc(int id)
        {
            base.DrawWindowFunc(id);
            GUILayout.BeginVertical();
            for (int i = 0; i < fields.Count; ++i)
            {
                GUILayout.BeginHorizontal();
                var field = fields[i];
                field.Type = EditorGUILayout.Popup(i == 0 ? "A" : "B", field.Type, FieldNode.S_FIELDS);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
        
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 1000, 20));
        }

        protected override Color GetNodeColor()
        {
            return new Color(0.9f, 1, 0.3f, 1);
        }
        public override NodeType GetNodeType()
        {
            return NodeType.AddOp;
        }
    } 
}