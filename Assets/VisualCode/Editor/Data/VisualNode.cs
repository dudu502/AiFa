using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace VisualCode
{

    public class VisualNode 
    {
        public enum NodeType
        {
            None=0,
            SetVar = 1,
            GetVar = 2,
            Func = 3,
            AddOp = 4,
            
        }

        public Rect rect = new Rect();
        public List<FieldNode> fields = new List<FieldNode>();
        public FlowNode currentFlow = null;
        public FieldNode resultField = null;
        public static void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = start.center;
            Vector3 endPos = end.center;
            Vector3 startTan = startPos + Vector3.right *80;
            Vector3 endTan = endPos + Vector3.left * 80;
            Color shadowCol = new Color(0f, 0f, 0f, 0.06f);
            for (int i = 0; i < 1; i++) // Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 2);
        }
        public VisualNode()
        {
        }
        public VisualNode(Vector2 pos)
        {
            rect.position = pos;
        }

        protected void SetRectSize(Vector2 size)
        {
            rect.size = size;          
        }
        public bool ContainPos(Vector2 pos)
        {
            return rect.Contains(pos);
        }
        public void DrawPorts()
        {
            foreach(var field in fields)
                field.DrawRect(rect);               
            currentFlow.DrawRect(rect);
            if (resultField != null)
                resultField.DrawRect(rect);
        }
        public void DrawWindow()
        {
            GUI.color = GetNodeColor();
            rect = GUILayout.Window(GetHashCode(),rect,DrawWindowFunc, GetTitle(), GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true));
            GUI.color = Color.white;
        }
        protected virtual Color GetNodeColor()
        {
            return Color.white;
        }
        protected virtual string GetTitle()
        {
            return "";
        }
        public virtual NodeType GetNodeType()
        {
            return NodeType.None;
        }
        public void ResetAllReadyState2Zero()
        {
            foreach (var field in fields)
                field.ResetReadyState2Zero();
            if (currentFlow.InRect.State == 1)
                currentFlow.InRect.State = 0;
            if (currentFlow.OutRect.State == 1)
                currentFlow.OutRect.State = 0;
            if (resultField != null)
                resultField.ResetReadyState2Zero();
        }

        public AccessNode.AccessRect FindReadyStateAccessRect()
        {
            foreach (var field in fields)
            {
                if (field.OutRect.State == 1)
                    return field.OutRect;
                if (field.InRect.State == 1)
                    return field.InRect;
            }
            if (currentFlow.OutRect.State == 1)
                return currentFlow.OutRect;
            if (currentFlow.InRect.State == 1)
                return currentFlow.InRect;
            if (resultField != null&& resultField.OutRect.State == 1)
                return resultField.OutRect;
            return null;
        }
        
        protected virtual void DrawWindowFunc(int id)
        {
            GUI.color = GetNodeColor();
        }

        public void DrawConn(Vector2 vector2)
        {
            fields.ForEach((field) => field.DrawConn(vector2));
            currentFlow.DrawConn(vector2);
            if (resultField != null)
                resultField.DrawConn(vector2);
        }


        public void UpdateAccessRectReadyState(Vector2 pos)
        {
            foreach(var field in fields)
            {
                if (field.OutRect.Contains(pos) && field.OutRect.State == 0)
                    field.OutRect.State = 1;
                else if (field.InRect.Contains(pos) && field.InRect.State == 0)
                    field.InRect.State = 1;               
            }
            if (currentFlow.OutRect.Contains(pos) && currentFlow.OutRect.State == 0)
                currentFlow.OutRect.State = 1;
            else if (currentFlow.InRect.Contains(pos) && currentFlow.InRect.State == 0)
                currentFlow.InRect.State = 1;
            if (resultField != null && resultField.OutRect.Contains(pos) && resultField.OutRect.State == 0)
                resultField.OutRect.State = 1;
        }     
        public AccessNode.AccessRect GetMouseUpAccessRect(Vector2 pos)
        {
            foreach(var field in fields)
            {
                if (field.OutRect.Contains(pos) && field.OutRect.State == 0)
                    return field.OutRect;
                if (field.InRect.Contains(pos) && field.InRect.State == 0)
                    return field.InRect;
            }
            if (currentFlow.OutRect.Contains(pos) && currentFlow.OutRect.State == 0)
                return currentFlow.OutRect;
            if (currentFlow.InRect.Contains(pos) && currentFlow.InRect.State == 0)
                return currentFlow.InRect;
            if (resultField != null && resultField.OutRect.Contains(pos) && resultField.OutRect.State == 0)
                return resultField.OutRect;
            return null;
        }       

        public void Remove()
        {
            foreach (var field in fields)
                field.RemoveImpl();
            currentFlow.RemoveImpl();
            if (resultField != null) resultField.RemoveImpl();
        }   

        public static ByteBuffer Write(VisualNode node)
        {
            ByteBuffer bfs = new ByteBuffer();
            bfs.WriteFloat(node.rect.x);
            bfs.WriteFloat(node.rect.y);
            bfs.WriteFloat(node.rect.width);
            bfs.WriteFloat(node.rect.height);
            bfs.WriteByte((byte)node.fields.Count);
            for (int i = 0; i < node.fields.Count; ++i)
            {
                bfs.WriteBytes(FieldNode.Write(node.fields[i]));
            }
            bfs.WriteBytes(FlowNode.Write(node.currentFlow));
            return bfs;
        }
        public static void Read(ByteBuffer bfs, VisualNode node)
        {
            node.rect.x = bfs.ReadFloat();
            node.rect.y = bfs.ReadFloat();
            node.rect.width = bfs.ReadFloat();
            node.rect.height = bfs.ReadFloat();
            int count = bfs.ReadByte();
            for (int i = 0; i < count; ++i)
            {
                var field = FieldNode.Read(bfs.ReadBytes());
                field.Target = node;
                node.fields.Add(field);
            }
            node.currentFlow = FlowNode.Read(bfs.ReadBytes());
            node.currentFlow.Target = node;
        }
    }
    public class AccessNode : ILinkNode
    {
        public enum AccessMode
        {
            Private,
            Protected,
            Public,
        }
        public enum DomainMode
        {
            Local,
            Global,
        }

        #region rect class
        public class AccessRect 
        {
            public enum RectType
            {
                FieldIn = 1,
                FieldOut = 2,
                FlowIn = 3,
                FlowOut = 4,
            }
            public AccessNode Target;
            public RectType Type = RectType.FieldIn;
            public Rect Rect = new Rect();
            public int State = 0;
            public bool Enable = true;
            public int HashID = 0;
            public AccessRect() { }
            public AccessRect(float x, float y, float w, float h)
            {
                HashID = GetHashCode();
                Rect = new Rect(x, y, w, h);
            }
            public Vector2 size {
                get { return Rect.size; }
            }
            public bool Contains(Vector2 pos)
            {
                return Rect.Contains(pos);
            }

            public static byte[] Write(AccessRect access)
            {
                ByteBuffer bfs = new ByteBuffer();
                bfs.WriteFloat(access.Rect.x);
                bfs.WriteFloat(access.Rect.y);
                bfs.WriteFloat(access.Rect.width);
                bfs.WriteFloat(access.Rect.height);
                bfs.WriteByte((byte)access.State);
                bfs.WriteBool(access.Enable);
                bfs.WriteByte((byte)access.Type);
                bfs.WriteInt32(access.HashID);
                return bfs.Getbuffer();
            }

            public static AccessRect Read(byte[] bytes)
            {
                AccessRect data = new AccessRect();
                ByteBuffer bfs = new ByteBuffer(bytes);
                data.Rect.x = bfs.ReadFloat();
                data.Rect.y = bfs.ReadFloat();
                data.Rect.width = bfs.ReadFloat();
                data.Rect.height = bfs.ReadFloat();
                data.State = bfs.ReadByte();
                data.Enable = bfs.ReadBool();
                data.Type = (RectType)bfs.ReadByte();
                data.HashID = bfs.ReadInt32();
                return data;
            }

            public int GetHashID { get; set; }

            public Vector2 position
            {
                get { return Rect.position; }
            }
        }
        #endregion
        
        public AccessMode Access = AccessMode.Public;
        public DomainMode Domain = DomainMode.Global;
        public AccessRect InRect = new AccessRect(0, 0, 10, 10);
        public AccessRect OutRect = new AccessRect(0, 0, 10, 10);
        
        public ILinkNode Next;
        public ILinkNode Prev;
        public int NextHashID = 0;
        public int PrevHashID = 0;
        public VisualNode Target;
        public int HashID = 0;
        public AccessNode(VisualNode target)
        {
            Target = target;
            InRect.Target = this;
            OutRect.Target = this;
            HashID = GetHashCode();
        }
        public bool HasInRect()
        {
            return InRect.Enable;
        }

        
        public bool HasOutRect()
        {
            return OutRect.Enable;
        }
        public void RemoveImpl()
        {
            this.CutOff();
        }
        public void ResetReadyState2Zero()
        {
            if (OutRect.State == 1)
                OutRect.State = 0;
            if (InRect.State == 1)
                InRect.State = 0;
        }
        public virtual void DrawRect(Rect parent)
        {
            
        }

        public virtual void DrawConn(Vector2 pos)
        {
            if (OutRect.Enable)
            {
                if (OutRect.State == 1)
                {
                    VisualNode.DrawNodeCurve(OutRect.Rect, new Rect(pos, new Vector2(10, 10)));
                }
            }

            if (InRect.Enable)
            {
                if (InRect.State == 1)
                {
                    VisualNode.DrawNodeCurve(new Rect(pos, new Vector2(10, 10)), InRect.Rect);
                }
                else if (GetPrev() != null)
                {
                    var prevRect = ((AccessNode)GetPrev()).OutRect;
                    VisualNode.DrawNodeCurve(prevRect.Rect, InRect.Rect);
                    if (GUI.Button(new Rect() { size = new Vector2(8, 8), center = (prevRect.Rect.center + InRect.Rect.center) / 2 }, ""))
                    {
                        InRect.Target.SetPrev(null);
                        prevRect.Target.SetNext(null);
                    }
                }
            }
        }


        public ILinkNode GetNext()
        {
            return Next;
        }

        public void SetNext(ILinkNode value)
        {    
            OutRect.State = value == null ? 0 : 2;
            Next = value;
        }

        public ILinkNode GetPrev()
        {
            return Prev;
        }

        public void SetPrev(ILinkNode value)
        {
            InRect.State = value == null ? 0 : 2;
            Prev = value;
        }
    }
    public class FlowNode : AccessNode
    {       
        public FlowNode(VisualNode target):base(target)
        {
            OutRect.Type = AccessRect.RectType.FlowOut;
            InRect.Type = AccessRect.RectType.FlowIn;               
        }
        public override void DrawRect(Rect parent)
        {           
            if (HasOutRect())
            {
                OutRect.Rect = new Rect(parent.x + parent.width, parent.y , 16, 16);
                GUI.Button(OutRect.Rect, "", VisualGraphEditor.OutputNodeStyle);
            }
            if (HasInRect())
            {
                InRect.Rect = new Rect(parent.x - 16, parent.y , 16, 16);
                GUI.Button(InRect.Rect, "", VisualGraphEditor.InputNodeStyle);
            }
        }
        public override void DrawConn(Vector2 pos)
        {
            base.DrawConn(pos);
        }

        public static byte[] Write(FlowNode node)
        {
            ByteBuffer bfs = new ByteBuffer();
            bfs.WriteByte((byte)node.Access);
            bfs.WriteByte((byte)node.Domain);
            bfs.WriteBytes(AccessRect.Write(node.OutRect));
            bfs.WriteBytes(AccessRect.Write(node.InRect));
            bfs.WriteInt32(node.HashID);
            if (node.GetPrev() != null)
            {
                bfs.WriteBool(true);
                bfs.WriteInt32(((FlowNode)node.GetPrev()).HashID);
            }
            else
            {
                bfs.WriteBool(false);
            }
            if(node.GetNext()!=null)
            {
                bfs.WriteBool(true);
                bfs.WriteInt32(((FlowNode)node.GetNext()).HashID);
            }
            else
            {
                bfs.WriteBool(false);
            }
            return bfs.Getbuffer();
        }

        public static FlowNode Read(byte[] bytes)
        {
            FlowNode node = new FlowNode(null);
            ByteBuffer bfs = new ByteBuffer(bytes);
            node.Access = (AccessMode)bfs.ReadByte();
            node.Domain = (DomainMode)bfs.ReadByte();
            node.OutRect = AccessRect.Read(bfs.ReadBytes());
            node.OutRect.Target = node;
            node.InRect = AccessRect.Read(bfs.ReadBytes());
            node.InRect.Target = node;
            node.HashID = bfs.ReadInt32();
            if(bfs.ReadBool())
            {
                node.PrevHashID = bfs.ReadInt32();
            }
            if(bfs.ReadBool())
            {
                node.NextHashID = bfs.ReadInt32();
            }
            return node;
        }
    }


    public class FieldNode : AccessNode
    {
        public static string[] S_FIELDS = new string[] {
            "int",
            "short",
            "float",
            "string",
        };
        
        public int Type = 0;
        public string Name="";
        public string Value="";
        public int Index;

        public FieldNode(int index,VisualNode target):base(target)
        {
            OutRect.Type = AccessRect.RectType.FieldOut;
            InRect.Type = AccessRect.RectType.FieldIn;
            Index = index;
        }

        

        public override void DrawConn(Vector2 pos)
        {
            base.DrawConn(pos);          
        }

        int CalcOffset()
        {
            return 23 + 18 * Index;
        }
        override public void DrawRect(Rect parent)
        {
            if(HasOutRect())
            {
                OutRect.Rect = new Rect(parent.x + parent.width, parent.y + CalcOffset(), 10, 10);
                GUI.Button(OutRect.Rect, "",VisualGraphEditor.OutputNodeStyle);
            }
            if(HasInRect())
            {
                InRect.Rect = new Rect(parent.x - 10, parent.y + CalcOffset(), 10, 10);
                GUI.Button(InRect.Rect, "", VisualGraphEditor.InputNodeStyle);
            }
        }

        public static byte[] Write(FieldNode node)
        {
            ByteBuffer bfs = new ByteBuffer();
            bfs.WriteByte((byte)node.Access);
            bfs.WriteByte((byte)node.Domain);
            bfs.WriteBytes(AccessRect.Write(node.OutRect));
            bfs.WriteBytes(AccessRect.Write(node.InRect));
            bfs.WriteByte((byte)node.Type);
            bfs.WriteString(node.Name);
            bfs.WriteString(node.Value);
            bfs.WriteByte((byte)node.Index);
            bfs.WriteInt32(node.HashID);
            if(node.GetPrev()!=null)
            {
                bfs.WriteBool(true);
                bfs.WriteInt32(((FieldNode)node.GetPrev()).HashID);
            }
            else
            {
                bfs.WriteBool(false);
            }
            if (node.GetNext() != null)
            {
                bfs.WriteBool(true);
                bfs.WriteInt32(((FieldNode)node.GetNext()).HashID);
            }
            else
            {
                bfs.WriteBool(false);
            }
            return bfs.Getbuffer();
        }

        public static FieldNode Read(byte[] bytes)
        {
            FieldNode node = new FieldNode(0,null);
            ByteBuffer bfs = new ByteBuffer(bytes);
            node.Access = (AccessMode)bfs.ReadByte();
            node.Domain = (DomainMode)bfs.ReadByte();
            node.OutRect = AccessRect.Read(bfs.ReadBytes());
            node.OutRect.Target = node;
            node.InRect = AccessRect.Read(bfs.ReadBytes());
            node.InRect.Target = node;
            node.Type = bfs.ReadByte();
            node.Name = bfs.ReadString();
            node.Value = bfs.ReadString();
            node.Index = bfs.ReadByte();
            node.HashID = bfs.ReadInt32();
            if(bfs.ReadBool())
            {
                node.PrevHashID = bfs.ReadInt32();
            }
            if(bfs.ReadBool())
            {
                node.NextHashID = bfs.ReadInt32();
            }
            return node;
        }
    }
}
