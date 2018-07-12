using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace VisualCode
{
    public class VisualGraphData 
    {
        public List<VisualNode> Nodes = new List<VisualNode>();
        public static byte[] Write(VisualGraphData data)
        {
            ByteBuffer bfs = new ByteBuffer();
            bfs.WriteInt32(data.Nodes.Count);
            for (int i = 0; i < data.Nodes.Count; ++i)
            {
                var d = data.Nodes[i];
                bfs.WriteShort((short)d.GetNodeType());
                if (d.GetNodeType() == VisualNode.NodeType.SetVar)
                    bfs.WriteBytes(SetVarNode.Write(d as SetVarNode));
                else if (d.GetNodeType() == VisualNode.NodeType.GetVar)
                    bfs.WriteBytes(GetVarNode.Write(d as GetVarNode));
                else if (d.GetNodeType() == VisualNode.NodeType.Func)
                    bfs.WriteBytes(FuncNode.Write(d as FuncNode));
                else if (d.GetNodeType() == VisualNode.NodeType.AddOp)
                    bfs.WriteBytes(AddOpNode.Write(d as AddOpNode));
            }
            return bfs.Getbuffer();
        }
        public static VisualGraphData Read(byte[] bytes)
        {
            VisualGraphData data = new VisualGraphData();
            ByteBuffer bfs = new ByteBuffer(bytes);
            int count = bfs.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                VisualNode.NodeType type = (VisualNode.NodeType)bfs.ReadShort();
                if (type == VisualNode.NodeType.SetVar)
                    data.Nodes.Add(SetVarNode.Read(bfs.ReadBytes()));
                else if (type == VisualNode.NodeType.GetVar)
                    data.Nodes.Add(GetVarNode.Read(bfs.ReadBytes()));
                else if (type == VisualNode.NodeType.Func)
                    data.Nodes.Add(FuncNode.Read(bfs.ReadBytes()));
                else if (type == VisualNode.NodeType.AddOp)
                    data.Nodes.Add(AddOpNode.Read(bfs.ReadBytes()));
            }
            return data;
        }

        public void CalcConnectionInfos()
        {
            foreach (var nod in Nodes)
            {
                if (nod.currentFlow.OutRect.State == 2)
                {
                    var item = GetAccessNodeByHashID(nod.currentFlow.NextHashID);
                    if (item != null) nod.currentFlow.AddNext(item);
                }
                if (nod.currentFlow.InRect.State == 2)
                {
                    var item = GetAccessNodeByHashID(nod.currentFlow.PrevHashID);
                    if (item != null) nod.currentFlow.AddPrev(item);
                }

                foreach(var field in nod.fields)
                {
                    if(field.OutRect.State == 2)
                    {
                        var item = GetAccessNodeByHashID(field.NextHashID);
                        if (item != null) field.AddNext(item);
                    }
                    if(field.InRect.State == 2)
                    {
                        var item = GetAccessNodeByHashID(field.PrevHashID);
                        if (item != null) field.AddPrev(item);
                    }
                }
                if(nod.resultField!=null&&nod.resultField.OutRect.State==2)
                {
                    var item = GetAccessNodeByHashID(nod.resultField.NextHashID);
                    if (item != null) nod.resultField.AddNext(item);
                }
            }
        }

        public AccessNode GetAccessNodeByHashID(int hashID)
        {
            foreach(var nod in Nodes)
            {
                if (hashID == nod.currentFlow.HashID)
                    return nod.currentFlow;             
                foreach(var field in nod.fields)
                {
                    if (hashID == field.HashID)
                        return field;
                }
                if (nod.resultField != null && nod.resultField.HashID == hashID)
                    return nod.resultField;

            }
            return null;
        }
    }
}