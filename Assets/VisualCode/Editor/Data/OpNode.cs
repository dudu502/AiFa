using UnityEngine;
using System.Collections;
using UnityEditor;

namespace VisualCode
{
    public class AddOpNode : OpNode
    {
        public AddOpNode() { }
        public AddOpNode(Vector2 pos) : base(pos) { }


        public override NodeType GetNodeType()
        {
            return NodeType.AddOp;
        }
        protected override string GetResultString()
        {
            return "A + B";
        }
    }
    public class MinusOpNode : OpNode
    {
        public MinusOpNode() { }
        public MinusOpNode(Vector2 pos) : base(pos) { }


        public override NodeType GetNodeType()
        {
            return NodeType.MinusOp;
        }
        protected override string GetResultString()
        {
            return "A - B";
        }
    }

    public class MultiplyOpNode : OpNode
    {
        public MultiplyOpNode() { }
        public MultiplyOpNode(Vector2 pos) : base(pos) { }
   

        public override NodeType GetNodeType()
        {
            return NodeType.MultiplyOp;
        }
        protected override string GetResultString()
        {
            return "A * B";
        }

    }

    public class DivisionOpNode : OpNode
    {
        public DivisionOpNode() { }
        public DivisionOpNode(Vector2 pos) : base(pos) { }
        protected override string GetResultString()
        {
            return "A / B";
        }
        public override NodeType GetNodeType()
        {
            return NodeType.DivisionOp;
        }
      
    }


    public class OpNode : VisualNode
    {
        public OpNode() { }

        public OpNode(Vector2 pos):base(pos)
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



        protected override void DrawWindowFunc(int id)
        {
            base.DrawWindowFunc(id);
            GUILayout.BeginVertical();
            for(int i=0;i<fields.Count;++i)
            {
                GUILayout.BeginHorizontal();
                var field = fields[i];
                field.Type = EditorGUILayout.Popup(i==0?"A":"B",field.Type, FieldNode.S_FIELDS);
                GUILayout.EndHorizontal();
            }

            GUILayout.BeginHorizontal();
            resultField.OutRect.Enable = EditorGUILayout.BeginToggleGroup(GetResultString(), true);
            resultField.Type = EditorGUILayout.Popup(resultField.Type, FieldNode.S_FIELDS);
            EditorGUILayout.EndToggleGroup();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUI.DragWindow(new Rect(0, 0, 1000, 20));
        }
        protected virtual string GetResultString()
        {
            return "";
        }
        protected override Color GetNodeColor()
        {
            return new Color(0.5f,1,0.7f,1) ;
        }
        public override NodeType GetNodeType()
        {
            return NodeType.AddOp;
        }

        public static byte[] Write(OpNode node)
        {
            ByteBuffer bfs = VisualNode.Write(node);
            bfs.WriteBytes(FieldNode.Write(node.resultField));
            return bfs.Getbuffer();
        }

        public static OpNode Read(byte[] bytes,OpNode node)
        {
            ByteBuffer bfs = new ByteBuffer(bytes);
            VisualNode.Read(bfs, node);
            node.resultField = FieldNode.Read(bfs.ReadBytes());
            node.resultField.Target = node;
            return node;
        }
    }
}
