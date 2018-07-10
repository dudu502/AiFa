using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

namespace VisualCode
{
    public class VisualNode
    {
        public Rect rect = new Rect();
        public List<FieldNode> fields = new List<FieldNode>();
        public FlowNode currentFlow;
        public static void DrawNodeCurve(Rect start, Rect end)
        {
            Vector3 startPos = start.center;
            Vector3 endPos = end.center;
            Vector3 startTan = startPos + Vector3.right *15;
            Vector3 endTan = endPos + Vector3.left * 15;
            Color shadowCol = new Color(0f, 0f, 0f, 0.06f);
            for (int i = 0; i < 1; i++) // Draw a shadow
                Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 2);
        }

        public VisualNode(Vector2 pos)
        {
            rect.position = pos;
        }

        protected void SetRectSize(Vector2 size)
        {
            rect.size = size;          
        }

        public void DrawPorts()
        {
            foreach(var field in fields)
            {
                field.DrawRect(rect);               
            }
            currentFlow.DrawRect(rect);
        }
        public void DrawWindow()
        {
            rect = GUILayout.Window(GetHashCode(),rect,DrawWindowFunc, GetTitle(), GUILayout.ExpandWidth(true),GUILayout.ExpandHeight(true));
        }
        protected virtual string GetTitle()
        {
            return "";
        }
        public void ResetAllReadyState2Zero()
        {
            foreach (var field in fields)
                field.ResetReadyState2Zero();
            if (currentFlow.InRect.State == 1)
                currentFlow.InRect.State = 0;
            if (currentFlow.OutRect.State == 1)
                currentFlow.OutRect.State = 0;
        }

        public FieldNode GetAngOutStateContainPos(Vector2 pos)
        {
            foreach(var field in fields)
            {
                if (field.OutRect.Contains(pos)&&field.OutRect.State == 0) return field;
            }
            return null;
        }

        public FlowNode GetFlowAngOutStateContainPos(Vector2 pos)
        {
            if (currentFlow.OutRect.Rect.Contains(pos) && currentFlow.OutRect.State == 0)
                return currentFlow;
            return null;
        }
        public FlowNode GetFlowAngInStateContainPos(Vector2 pos)
        {
            if (currentFlow.InRect.Rect.Contains(pos) && currentFlow.InRect.State == 0)
                return currentFlow;
            return null;
        }

        public FieldNode GetAngInStateContainPos(Vector2 pos)
        {
            foreach (var field in fields)
            {
                if (field.InRect.Contains(pos)&&field.InRect.State==0) return field;
            }
            return null;
        }
        protected virtual void DrawWindowFunc(int id)
        {
            
        }

        public void DrawConn(Vector2 vector2)
        {
            fields.ForEach((field) => field.DrawConn(vector2));
            currentFlow.DrawConn(vector2);
        }

        public void OnMouseDownInOutRect(Vector2 pos)
        {
            var outField = GetAngOutStateContainPos(pos);
            if (outField != null)
                outField.OutRect.State = 1;
            var inField = GetAngInStateContainPos(pos);
            if (inField != null)
                inField.InRect.State = 1;

            if (currentFlow.OutRect.Contains(pos)&&currentFlow.OutRect.State==0)
                currentFlow.OutRect.State = 1;
            if (currentFlow.InRect.Contains(pos)&&currentFlow.InRect.State==0)
                currentFlow.InRect.State = 1;
        }
        public void ProcessEvent(Event e)
        {
            
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

        public class AccessRect
        {
            public Rect Rect = new Rect();
            public int State = 0;
            public bool Enable = true;
            public AccessRect() { }
            public AccessRect(float x, float y, float w, float h)
            {
                Rect = new Rect(x,y,w,h);
            }
            public AccessRect(Vector2 pos,Vector2 size)
            {
                Rect.position = pos;
                Rect.size = size;
            }
            public Vector2 size {
                get { return Rect.size; }
            }
            public bool Contains(Vector2 pos)
            {
                return Rect.Contains(pos);
            }
            public Vector2 position
            {
                get { return Rect.position; }
            }
        }
        public AccessMode Access = AccessMode.Public;
        public DomainMode Domain = DomainMode.Global;
        public AccessRect InRect = new AccessRect(0, 0, 10, 10);
        public AccessRect OutRect = new AccessRect(0, 0, 10, 10);
        
        public ILinkNode Next;
        public ILinkNode Prev;
        public bool HasInRect()
        {
            return InRect.Enable;
        }

        
        public bool HasOutRect()
        {
            return OutRect.Enable;
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
            if(OutRect.Enable)
            {
                if (OutRect.State == 1)
                {
                    VisualNode.DrawNodeCurve(OutRect.Rect, new Rect(pos, new Vector2(10, 10)));
                }
                else if (OutRect.State == 0)
                {
                    VisualNode.DrawNodeCurve(OutRect.Rect, new Rect(OutRect.position + new Vector2(5, 0), new Vector2(10, 10)));
                }
                else if (GetNext() != null)
                {
                    VisualNode.DrawNodeCurve(OutRect.Rect, ((AccessNode)GetNext()).InRect.Rect);
                }

            }
            if(InRect.Enable)
            {
                if (InRect.State == 1)
                {
                    VisualNode.DrawNodeCurve(new Rect(pos, new Vector2(10, 10)), InRect.Rect);
                }
                else if (InRect.State == 0)
                {
                    VisualNode.DrawNodeCurve(new Rect(OutRect.position - new Vector2(5, 0), new Vector2(10, 10)), InRect.Rect);
                }
                else if (GetPrev() != null)
                {
                    VisualNode.DrawNodeCurve(((AccessNode)GetPrev()).OutRect.Rect, InRect.Rect);
                }
            }
            
        }


        public ILinkNode GetNext()
        {
            return Next;
        }

        public void SetNext(ILinkNode value)
        {
            Next = value;
        }

        public ILinkNode GetPrev()
        {
            return Prev;
        }

        public void SetPrev(ILinkNode value)
        {
            Prev = value;
        }
    }
    public class FlowNode : AccessNode
    {
        public VisualNode Target;
        public FlowNode(VisualNode target)
        {
            Target = target;          
        }
        public override void DrawRect(Rect parent)
        {
            if (HasOutRect())
            {
                OutRect.Rect = new Rect(parent.x + parent.width, parent.y , 16, 16);
                GUI.Button(OutRect.Rect, "");
            }
            if (HasInRect())
            {
                InRect.Rect = new Rect(parent.x - 16, parent.y , 16, 16);
                GUI.Button(InRect.Rect, "");
            }
        }
        public override void DrawConn(Vector2 pos)
        {
            base.DrawConn(pos);
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
        public VisualNode Target;

        public FieldNode(int index,VisualNode target)
        {
            Target = target;
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
                GUI.Button(OutRect.Rect, "");
            }
            if(HasInRect())
            {
                InRect.Rect = new Rect(parent.x - 10, parent.y + CalcOffset(), 10, 10);
                GUI.Button(InRect.Rect, "");
            }
        }
    }
}
