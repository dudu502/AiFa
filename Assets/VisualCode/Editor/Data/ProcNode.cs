﻿using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VisualCode
{

    public class ProcNode : VisualNode
    {
        public ProcNode() { }
        public ProcNode(Vector2 pos) : base(pos)
        {
            SetRectSize(new Vector2(120, 40));
            Title = "Proc";

            resultField = new FieldNode(0, this);
            resultField.InRect.Enable = false;
            resultField.OutRect.Enable = false;
            currentFlow = new FlowNode(this);
        }
        public void AddParam(bool input, bool output)
        {
            var field = new FieldNode(fields.Count, this);
            field.OutRect.Enable = output;
            field.InRect.Enable = input;
            fields.Add(field);
            resultField.Index = fields.Count;
        }
        protected override void DrawWindowFunc(int id)
        {
            base.DrawWindowFunc(id);
            GUILayout.BeginVertical();
            foreach (var field in fields)
            {
                GUILayout.BeginHorizontal();
                field.Type = EditorGUILayout.Popup(field.Type, FieldNode.S_FIELDS);
                field.Name = EditorGUILayout.TextField(field.Name);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            resultField.InRect.Enable = EditorGUILayout.BeginToggleGroup("Set Return", resultField.InRect.Enable);
            resultField.Type = EditorGUILayout.Popup(resultField.Type, FieldNode.S_FIELDS);
            EditorGUILayout.EndToggleGroup();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 1000, 20));
        }

   

        protected override Color GetNodeColor()
        {
            return new Color(0.9f,0.7f,0.4f,1);
        }
        public override NodeType GetNodeType()
        {
            return NodeType.Proc;
        }
        public static byte[] Write(ProcNode node)
        {
            ByteBuffer bfs = VisualNode.Write(node);
            bfs.WriteBytes(FieldNode.Write(node.resultField));
            return bfs.Getbuffer();
        }

        public static ProcNode Read(byte[] bytes)
        {
            ProcNode node = new ProcNode();
            ByteBuffer bfs = new ByteBuffer(bytes);
            VisualNode.Read(bfs, node);
            node.resultField = FieldNode.Read(bfs.ReadBytes());
            node.resultField.Target = node;
            return node;
        }
    }
}