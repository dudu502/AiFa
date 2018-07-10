﻿using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VisualCode
{

    public class SetVarNode : VisualNode
    {
        public SetVarNode(Vector2 pos) : base(pos)
        {
            SetRectSize(new Vector2(160, 40));

            for (int i = 0; i < 1; ++i)
            {
                var field = new FieldNode(i, this);
                field.OutRect.Enable = false;
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
                field.Access = (AccessNode.AccessMode)EditorGUILayout.EnumPopup(field.Access);
                field.Type = EditorGUILayout.Popup(field.Type, FieldNode.S_FIELDS);
                field.Name = EditorGUILayout.TextField(field.Name);
                field.Value = EditorGUILayout.TextField(field.Value);
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 1000, 20));
        }
        protected override string GetTitle()
        {
            return "Set Var";
        }
    }
}