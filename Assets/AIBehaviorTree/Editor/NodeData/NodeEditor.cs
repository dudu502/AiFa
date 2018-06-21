using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;
using UnityEditor;
using UnityEngine;
using AIBehaviorTree;

public class NodeEditor : EditorWindow {

    [MenuItem("Window/NodeEditor")]
    static void ShowEditor()
    {
        NodeEditor editor = GetWindow<NodeEditor>();
        editor.Show();
        editor.Init();
    }
    static public void ShowEditor(TextAsset asset)
    {
        NodeEditor editor = GetWindow<NodeEditor>();
        editor.Show();
        editor.Init();
        editor.SetSelectTextAsset(asset);
    }


    private TextAsset m_CurrentTextAsset = null;
    private Vector2 WindowScrollPos;
    private NodeGraph m_RootNode;
    private void Init()
    {       
        m_RootNode = new NodeGraph();
        m_RootNode.ToRect();
        m_RootNode.Type = NodeGraph.NODETYPE.SEQUENCE;
    }

    #region 菜单功能
    void OnExportAllHandler(object data)
    {
        var node = data as NodeGraph;
        var bytes = NodeGraph.CreateInBinary(node);
        var path = Application.dataPath + AI.TREE_OUTPUTPATH + node.OutPutPath + ".bytes";
        File.WriteAllBytes(path, bytes);
        EditorUtility.DisplayDialog("提示", "导出成功"+ path, "ok");
        AssetDatabase.Refresh();
    }
    void OnNodeDeleteHandler(object data)
    {
        var node = data as NodeGraph;
        if (node.Parent != null)
            node.Parent.RemoveNode(node);                    
        else
            EditorUtility.DisplayDialog("警告", "不可删除根节点", "ok");
    }

    void OnNodeMenuClickCreateChildHandler(object data)
    {
        var node = data as NodeGraph;
        var child = CreateANewNode( new Vector2(node.NodeRect.x+300, node.NodeRect.y));
        node.AddNode(child);
    }
    void OnNodeMoveUpInParentHandler(object data)
    {
        var node = data as NodeGraph;
        if (node.Parent != null)
        {
            int index = node.Parent.Nodes.IndexOf(node);
            node.Parent.ExchangeChild(index, index - 1);
        }
    }
    void OnNodeMoveDownInParentHandler(object data)
    {
        var node = data as NodeGraph;
        if (node.Parent != null)
        {
            int index = node.Parent.Nodes.IndexOf(node);
            node.Parent.ExchangeChild(index, index + 1);
        }
    }
    #endregion
    NodeGraph CreateANewNode(Vector2 mpos)
    {
        var node = new NodeGraph();
        node.ClickPos = mpos;
        node.ToRect();
        return node;
    }

    NodeGraph GetNodeByID(int id)
    {
        return NodeGraph.FindByID(m_RootNode,id);
    }
    NodeGraph GetContainMousePosNode(Vector2 mpos)
    {
       return NodeGraph.FindByMousePos(m_RootNode,mpos);
    }
    void OnGUI()
    {
        #region EventHandler
        //right click event
        if (Event.current.type == EventType.ContextClick)
        {
            var menu = new GenericMenu();
            var rightClickNode = GetContainMousePosNode(Event.current.mousePosition+ WindowScrollPos);
            if (rightClickNode != null)
            {
                menu.AddItem(new GUIContent("Create Child"),false, OnNodeMenuClickCreateChildHandler, rightClickNode);                
                if(rightClickNode.Parent!=null)
                {
                    menu.AddItem(new GUIContent("Delete Current"), false, OnNodeDeleteHandler, rightClickNode);
                    if (rightClickNode.Parent.HasPrevChild(rightClickNode))
                    {
                        menu.AddItem(new GUIContent("Move Up"), false, OnNodeMoveUpInParentHandler, rightClickNode);
                    }
                    if (rightClickNode.Parent.HasNextChild(rightClickNode))
                    {
                        menu.AddItem(new GUIContent("Move Down"), false, OnNodeMoveDownInParentHandler, rightClickNode);
                    }
                }
            }
            menu.ShowAsContext();
            Event.current.Use();
        }
        
        #endregion
        WindowScrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height),
        WindowScrollPos, new Rect(0, 0, 10000, 10000));      
        if (m_RootNode != null)
        {
            DrawCurvesImpl(m_RootNode);
            BeginWindows();           
            InitWindow(m_RootNode);
            EndWindows();
        }        
        GUI.EndScrollView(); 
    }

    void InitWindow(NodeGraph parent)
    {
        string title = parent.Parent == null ? "Root" : string.Format("No.{0}", parent.Parent.Nodes.IndexOf(parent));
        GUI.color = NodeGraph.GetColorByType((int)parent.Type);
        parent.NodeRect = new Rect(parent.NodeRect.x, parent.NodeRect.y, parent.NodeRect.width, 200);
        parent.NodeRect = GUILayout.Window(parent.ID,parent.NodeRect, DrawNodeWindow, new GUIContent(title),GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));
        GUI.color = Color.black;
        for (int i = 0; i < parent.Nodes.Count; ++i)
        {
            InitWindow(parent.Nodes[i]);
        }
    }

    void OnSelectionChange()
    {
        if (Selection.objects.Length > 0)
        {
            SetSelectTextAsset(Selection.objects[0] as TextAsset);
        }       
    }

    public void SetSelectTextAsset(TextAsset txtAsset)
    {
        m_CurrentTextAsset = txtAsset;
        if (txtAsset == null) return;
        NodeGraph node = null;
        try
        {
            node = NodeGraph.CreateFromBinary(m_CurrentTextAsset.bytes);
            m_RootNode = node;
            m_RootNode.OutPutPath = m_CurrentTextAsset.name;
        }
        catch(Exception e)
        {
            Debug.Log("Not Bytes File");
            return;
        }
    }


    void DrawNodeWindow(int id)
    {
        var node = GetNodeByID(id);
        if (node == null)
            return;
        EditorGUILayout.BeginVertical("box");

        EditorGUILayout.BeginVertical("box");
        node.FoldOutDes = EditorGUILayout.Foldout(node.FoldOutDes, "Name (Node Description)");
        if(node.FoldOutDes)
            node.Name = EditorGUILayout.TextField("", node.Name);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        node.Type = (NodeGraph.NODETYPE)EditorGUILayout.EnumPopup("Type", node.Type);
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        node.FoldOutScriptName = EditorGUILayout.Foldout(node.FoldOutScriptName,"ScriptPath&Name");
        if(node.FoldOutScriptName)
            node.ScriptName = EditorGUILayout.TextField("", node.ScriptName);


        if (node.ScriptName != "")
        {
            string fullPath = Application.dataPath + AI.SCRIPT_OUTPUTPATH + node.ScriptName + ".txt";
            if (!File.Exists(fullPath))
            {
                if (GUILayout.Button("Create Script"))
                    NodeScriptTemplate.NewEmptyScript(fullPath, node);
            }
            else 
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Delete Script"))
                    NodeScriptTemplate.DeleteScript(fullPath);
                if (GUILayout.Button("Edit Script"))
                    System.Diagnostics.Process.Start(fullPath);
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndVertical();
        if (node.Parent != null)
        {
            if (node.Parent.Type == NodeGraph.NODETYPE.RANDOWSELECT)
                node.Weight = EditorGUILayout.IntField("Weight *(Weight>0)", Mathf.Max(1, node.Weight));
        }
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginVertical("box");
        node.ToolBarSelectIndex = GUILayout.Toolbar(node.ToolBarSelectIndex, NodeGraph.ToolBarNames);
        if(node.ToolBarSelectIndex == 0)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("Export Tree (Input Export Tree File Name)", MessageType.Info);
            node.OutPutPath = EditorGUILayout.TextField("", node.OutPutPath);
            if (node.OutPutPath != "")
            {
                if(!File.Exists(Application.dataPath + AI.TREE_OUTPUTPATH + node.OutPutPath + ".json"))
                {
                    if(GUILayout.Button("Export Tree"))
                        OnExportAllHandler(node);
                }                  
                else
                {
                    if (GUILayout.Button("Save Changes"))
                        OnExportAllHandler(node);
                }
            }
            EditorGUILayout.EndVertical();
        }
        else if(node.ToolBarSelectIndex == 1)
        {
            EditorGUILayout.BeginVertical("box");         
            EditorGUILayout.HelpBox("Set SubTree (Drag In Tree Data)", MessageType.Info);            
            node.SubTreeAsset = EditorGUILayout.ObjectField("SubTree TextAsset", node.SubTreeAsset, typeof(TextAsset)) as TextAsset;
            if (node.SubTreeAsset != null && GUILayout.Button("Add SubTree"))
            {
                node.AddNode(NodeGraph.CreateNodeGraph(JsonMapper.ToObject<JsonData>(node.SubTreeAsset.text)), new Vector2(200, 0));
            }
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndVertical();
        GUI.DragWindow(new Rect(0, 0, 1000,20));   
       
    }


    void DrawCurvesImpl(NodeGraph parent)
    {
        for (int i = 0; i < parent.Nodes.Count; ++i)
        {
            DrawNodeCurve(parent.NodeRect,parent.Nodes[i].NodeRect);
            DrawCurvesImpl(parent.Nodes[i]);
        }
    }
    void DrawNodeCurve(Rect start, Rect end)
    {       
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 40;
        Vector3 endTan = endPos + Vector3.left * 40;
        Color shadowCol = new Color(0f, 0f, 0f, 0.06f);
        for (int i = 0; i < 3; i++) // Draw a shadow
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 2);
    }
}
