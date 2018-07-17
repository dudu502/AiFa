using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VisualCode
{
    public class SetVarNode : VisualNode
    {
        public SetVarNode() { }
        public SetVarNode(Vector2 pos) : base(pos)
        {
            SetRectSize(new Vector2(200, 40));
            Title = "Set";
            for (int i = 0; i < 1; ++i)
            {
                var field = new FieldNode(i, this);
                field.OutRect.Enable = false;
                field.InRect.Enable = false;
                field.Name = "name";
                field.Value = "value";
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
                //field.Access = (AccessNode.AccessMode)EditorGUILayout.EnumPopup(field.Access);
                field.Type = EditorGUILayout.Popup(field.Type, FieldNode.S_FIELDS);
                field.Name = EditorGUILayout.TextField(field.Name);
                EditorGUILayout.Space();
                field.Value = EditorGUILayout.TextField(field.Value);
                GUILayout.EndHorizontal();
                OnAccessNodeModify(field,AccessNode.AccessModifyMode.SetVar);
            }
            GUILayout.EndVertical();

            GUI.DragWindow(new Rect(0, 0, 1000, 20));
        }
        protected override Color GetNodeColor()
        {
            return Color.green;
        }


        public override NodeType GetNodeType()
        {
            return NodeType.SetVar;
        }

        public static byte[] Write(SetVarNode node)
        {
            ByteBuffer bfs = VisualNode.Write(node);
            return bfs.Getbuffer();
        }
        public static SetVarNode Read(byte[] bytes)
        {
            SetVarNode node = new SetVarNode();
            ByteBuffer bfs = new ByteBuffer(bytes);
            VisualNode.Read(bfs, node);
            return node;
        }
    }
}