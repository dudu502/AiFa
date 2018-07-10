using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VisualCode
{

    public class GetVarNode : VisualNode
    {
        public GetVarNode(Vector2 pos) : base(pos)
        {
            SetRectSize(new Vector2(80, 40));

            for (int i = 0; i < 1; ++i)
            {
                var field = new FieldNode(i, this);
                field.InRect.Enable = false;
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
                
                field.Value = EditorGUILayout.TextField(field.Value);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 1000, 20));

        }
        protected override string GetTitle()
        {
            return "Get Var";
        }
    }
}