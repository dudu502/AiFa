using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VisualCode
{

    public class AddOpNode : VisualNode
    {
        public AddOpNode() { }

        public AddOpNode(Vector2 pos):base(pos)
        {
            SetRectSize(new Vector2(80, 40));
            for (int i = 0; i < 2; ++i)
            {
                var field = new FieldNode(i, this);
                field.OutRect.Enable = false;
                fields.Add(field);
            }
            resultField = new FieldNode(2, this);
            resultField.InRect.Enable = false;
            currentFlow = new FlowNode(this);
        }

        protected override string GetTitle()
        {
            return "Add";
        }
        protected override void DrawWindowFunc(int id)
        {
            base.DrawWindowFunc(id);
            GUILayout.BeginVertical();
            for(int i=0;i<fields.Count;++i)
            {
                GUILayout.BeginHorizontal();
                var field = fields[i];
                field.Type = EditorGUILayout.Popup(i==0?"Left":"Right",field.Type, FieldNode.S_FIELDS);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            resultField.OutRect.Enable = EditorGUILayout.BeginToggleGroup("Return", true);
            resultField.Type = EditorGUILayout.Popup(resultField.Type, FieldNode.S_FIELDS);
            EditorGUILayout.EndToggleGroup();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 1000, 20));
        }
        protected override Color GetNodeColor()
        {
            return new Color(0.5f,1,0.7f,1) ;
        }
        public override NodeType GetNodeType()
        {
            return NodeType.AddOp;
        }

        public static byte[] Write(AddOpNode node)
        {
            ByteBuffer bfs = VisualNode.Write(node);
            bfs.WriteBytes(FieldNode.Write(node.resultField));
            return bfs.Getbuffer();
        }

        public static AddOpNode Read(byte[] bytes)
        {
            AddOpNode node = new AddOpNode();
            ByteBuffer bfs = new ByteBuffer(bytes);
            VisualNode.Read(bfs, node);
            node.resultField = FieldNode.Read(bfs.ReadBytes());
            node.resultField.Target = node;
            return node;
        }
    }
}
