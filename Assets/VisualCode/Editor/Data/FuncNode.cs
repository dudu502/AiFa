using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VisualCode
{
    public class FuncNode : VisualNode
    {
        public FuncNode() { }
        public FuncNode(Vector2 pos) : base(pos)
        {
            SetRectSize(new Vector2(120, 40));
            Title = "func";
           
            resultField = new FieldNode(0, this);
            resultField.InRect.Enable = false;
            resultField.OutRect.Enable = false;
            currentFlow = new FlowNode(this);
        }

        public void AddParam(bool input,bool output)
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
            resultField.OutRect.Enable = EditorGUILayout.BeginToggleGroup("Return",resultField.OutRect.Enable);
            resultField.Type = EditorGUILayout.Popup(resultField.Type, FieldNode.S_FIELDS);
            EditorGUILayout.EndToggleGroup();
            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 1000, 20));
        }


        protected override Color GetNodeColor()
        {
            return Color.yellow;
        }
        public override NodeType GetNodeType()
        {
            return NodeType.Func;
        }

        public static byte[] Write(FuncNode node)
        {
            ByteBuffer bfs = VisualNode.Write(node);           
            bfs.WriteBytes(FieldNode.Write(node.resultField));
            return bfs.Getbuffer();
        }

        public static FuncNode Read(byte[] bytes)
        {
            FuncNode node = new FuncNode();
            ByteBuffer bfs = new ByteBuffer(bytes);
            VisualNode.Read(bfs, node);          
            node.resultField = FieldNode.Read(bfs.ReadBytes());
            node.resultField.Target = node;
            return node;
        }
    }
}