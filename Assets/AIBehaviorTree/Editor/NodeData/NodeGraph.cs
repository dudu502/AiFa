using System.Collections.Generic;
using LitJson;
using UnityEngine;
using UnityEditor;
using UnityEngine.EventSystems;

public class NodeGraph
{
    static Color S_ALPHA_COLOR = new Color(0, 0, 0, 0.3f);
    public static string[] ToolBarNames = new[] { "Export Tree", "Add SubTree"};
    public enum NODETYPE
    {
        ACTION = 0,
        SEQUENCE = 1,
        RANDOWSELECT = 2,
        PAIALLEL = 3,
    }
    public int ID;
    public string Name="";
    public NODETYPE Type = NODETYPE.ACTION;
    public int Weight=1;
    public string ScriptName="";
    public List<NodeGraph> Nodes = new List<NodeGraph>();

    public Vector2 ClickPos;

    public Rect NodeRect;

    public NodeGraph Parent = null;

    public string OutPutPath = "";
    public TextAsset SubTreeAsset = null;

    public int ToolBarSelectIndex = 0;

    public bool FoldOut = true;
    public NodeGraph()
    {       
        NodeRect = new Rect(0,0,60,50);
        ID = GetHashCode();
    }
    

    public void AddNode(NodeGraph node)
    {
        node.Parent = this;
        Nodes.Add(node);
        if (Type == NODETYPE.ACTION)
            Type = NODETYPE.SEQUENCE;
    }

    public void AddNode(NodeGraph node, Vector2 offset)
    {
        AddNode(node);
        //
        List<NodeGraph> result = new List<NodeGraph>();
        GetAllNodes(node, result);
        foreach (var nItem in result)
        {
            nItem.NodeRect.position += NodeRect.position + offset ;
        }
    }

    public void RemoveNode(NodeGraph node)
    {
        Nodes.Remove(node);
        if (Nodes.Count == 0)
            Type = NODETYPE.ACTION;
    }

    public Rect ToRect()
    {
        NodeRect.x = ClickPos.x;
        NodeRect.y = ClickPos.y;
        NodeRect.width = 240;
        NodeRect.height = 280;
        return NodeRect;
    }

    public void ExchangeChild(int index0,int index1)
    {
        if (index0 > -1 && index0 < Nodes.Count
            && index1 > -1 && index1 < Nodes.Count)
        {
            var temp = Nodes[index0];
            Nodes[index0] = Nodes[index1];
            Nodes[index1] = temp;
        }
    }

    public bool HasPrevChild(NodeGraph child)
    {
        var index = Nodes.IndexOf(child);
        return index > 0;
    }
    public bool HasNextChild(NodeGraph child)
    {
        var index = Nodes.IndexOf(child);
        return index < Nodes.Count - 1;
    }

    public static Color GetColorByType(int type)
    {
        if (type == (int)NODETYPE.ACTION)
            return Color.green - S_ALPHA_COLOR;
        if (type == (int)NODETYPE.PAIALLEL)
            return Color.red - S_ALPHA_COLOR;
        if (type == (int)NODETYPE.RANDOWSELECT)
            return Color.yellow - S_ALPHA_COLOR;
        return Color.gray - S_ALPHA_COLOR;
    }


    public override string ToString()
    {
        return string.Format("Name:{0} Type:{1} Weight:{2}",Name,Type.ToString(),Weight);
    }
    public static void GetAllNodes(NodeGraph node, List<NodeGraph> result)
    {
        result.Add(node);
        foreach (var child in node.Nodes)
        {
            GetAllNodes(child, result);
        }
    }
    public static NodeGraph FindByID(NodeGraph node, int id)
    {
        List<NodeGraph> all = new List<NodeGraph>();
        GetAllNodes(node, all);
        foreach (var item in all)
        {
            if (item.ID == id)
                return item;
        }
        return null;
    }

    public static NodeGraph FindByMousePos(NodeGraph node, Vector3 mpos)
    {
        List<NodeGraph> all = new List<NodeGraph>();
        GetAllNodes(node, all);
        foreach (var item in all)
        {
            if (item.NodeRect.Contains(mpos))
                return item;
        }
        return null;
    }

    public static JsonData CreateNodeJsonData(NodeGraph data)
    {
        var jd = new JsonData();
        jd["name"] = data.Name;
        jd["type"] = (int)data.Type;
        jd["scriptName"] = data.ScriptName;       
        jd["weight"] = data.Weight;
        jd["x"] = data.NodeRect.x;
        jd["y"] = data.NodeRect.y;
        var arr = new JsonData();
        arr.SetJsonType(JsonType.Array);
        jd["children"] = arr;
        for (int i = 0; i < data.Nodes.Count; ++i)
        {
            arr.Add(CreateNodeJsonData(data.Nodes[i]));
        }
        return jd;
    }
    public static NodeGraph CreateNodeGraph(JsonData data)
    {
        var node = new NodeGraph();
        node.ToRect();
        node.Name = data["name"].ToString();
        if(data.Keys.Contains("type"))
            node.Type = (NODETYPE) (int.Parse(data["type"].ToString()));
        if(data.Keys.Contains("weight"))
            node.Weight = (int.Parse(data["weight"].ToString()));
        if(data.Keys.Contains("x"))
            node.NodeRect.x = int.Parse(data["x"].ToString());
        if(data.Keys.Contains("y"))
            node.NodeRect.y = int.Parse(data["y"].ToString());
        if(data.Keys.Contains("scriptName"))
            node.ScriptName = data["scriptName"].ToString();
        if (data["children"].IsArray && data["children"].Count > 0)
        {
            foreach (JsonData childData in data["children"])
            {
                node.AddNode(CreateNodeGraph(childData));
            }
        }
        return node;
    }
}

