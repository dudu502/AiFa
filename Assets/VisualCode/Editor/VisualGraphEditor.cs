using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using VisualCode;

public class VisualGraphEditor : EditorWindow
{
    [MenuItem("Window/Node/VisualGraphEditor")]
    static void ShowEditor()
    {
        VisualGraphEditor editor = GetWindow<VisualGraphEditor>();
        editor.Show();
        editor.Init();
    }
    public static GUIStyle InputNodeStyle;
    public static GUIStyle OutputNodeStyle;
    private void Init()
    {
        InputNodeStyle = new GUIStyle();
        InputNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node0.png") as Texture2D;
    
        OutputNodeStyle = new GUIStyle();
        OutputNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/lightskin/images/node5.png") as Texture2D;
    }

    VisualGraphData GraphData = new VisualGraphData();
    private Vector2 WindowOffset;
    private Vector2 WindowDrag;

    public List<VisualNode> Nodes { get { return GraphData.Nodes; } }
    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);
        ProcessEvent(Event.current);
        InitWindow();
        DrawMenuBar();
    }
    private void DrawMenuBar()
    {
        GUILayout.BeginArea(new Rect(0, 0, position.width, 20f), EditorStyles.toolbar);
        GUILayout.BeginHorizontal();
        GUILayout.Space(5);
        if (GUILayout.Button(new GUIContent("Save"), EditorStyles.toolbarButton, GUILayout.Width(35)))
        {
            SaveNodes();
        }
        if (GUILayout.Button(new GUIContent("⊙"), EditorStyles.toolbarButton, GUILayout.Width(20)))
        {
            Nodes.ForEach((n) =>n.rect.position-= WindowOffset);
            WindowOffset -= WindowOffset;
        }
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }

    private void SaveNodes()
    {
        string path = Application.dataPath + "/VisualCode/Out/" + "nodes.bytes";
        File.WriteAllBytes(path, VisualGraphData.Write(GraphData));
        AssetDatabase.Refresh();
    }
    void OnSelectionChange()
    {
        if (Selection.objects.Length > 0)
        {
            SetSelectTextAsset(Selection.objects[0] as TextAsset);
        }
    }

    private void SetSelectTextAsset(TextAsset textAsset)
    {
        byte[] bytes = textAsset.bytes;
        if(bytes != null && bytes.Length>0)
        {
            GraphData = VisualGraphData.Read(bytes);
            GraphData.CalcConnectionInfos();
            Debug.Log(GraphData);
            WindowOffset = Vector2.zero;
        }
    }
    VisualNode OnFocusOnNode(Vector2 pos)
    {
        foreach(var n in Nodes)
        {
            if (n.ContainPos(pos))
                return n;
        }
        return null;
    }
    private void ProcessEvent(Event current)
    {
        WindowDrag = Vector2.zero;
        Vector2 vectorPos = current.mousePosition;
        if (current.type == EventType.MouseDown)
        {
            if (current.button == 1)//right mouse
            {
                var menu = new GenericMenu();
                var n = OnFocusOnNode(vectorPos);
                if (n != null)
                {
                    menu.AddItem(new GUIContent("Delete"), false, OnDeleteNodeHandler, n);
                    menu.ShowAsContext();
                    Event.current.Use();
                }                                    
                else
                {
                    menu.AddItem(new GUIContent("Variable/Set"), false, OnCreateNewVarNodeHandler, vectorPos);
                    menu.AddItem(new GUIContent("Variable/Get"), false, OnCreateGetVarNodeHandler, vectorPos);
                    menu.AddItem(new GUIContent("Function/[0]"), false, OnCreateFunctionNodeHandler, new object[] { 0, vectorPos });
                    menu.AddItem(new GUIContent("Function/[1]"), false, OnCreateFunctionNodeHandler, new object[] { 1, vectorPos });
                    menu.AddItem(new GUIContent("Function/[2]"), false, OnCreateFunctionNodeHandler, new object[] { 2, vectorPos });
                    menu.AddItem(new GUIContent("Function/[3]"), false, OnCreateFunctionNodeHandler, new object[] { 3, vectorPos });
                    menu.AddItem(new GUIContent("Function/[4]"), false, OnCreateFunctionNodeHandler, new object[] { 4, vectorPos });
                    menu.AddItem(new GUIContent("Calculation/Add"), false, OnCreateAddHandler, vectorPos);
                    menu.AddItem(new GUIContent("Calculation/Minus"), false, OnCreateMinusHandler, vectorPos);
                    menu.AddItem(new GUIContent("Calculation/Multiply"), false, OnCreateMultiplyHandler, vectorPos);
                    menu.AddItem(new GUIContent("Calculation/Division"), false, OnCreateDivisionHandler, vectorPos);
                    menu.AddItem(new GUIContent("Process/[0]"), false, OnCreateProcessNodeHandler, new object[] { 0, vectorPos });
                    menu.AddItem(new GUIContent("Process/[1]"), false, OnCreateProcessNodeHandler, new object[] { 1, vectorPos });
                    menu.AddItem(new GUIContent("Process/[2]"), false, OnCreateProcessNodeHandler, new object[] { 2, vectorPos });
                    menu.AddItem(new GUIContent("Process/[3]"), false, OnCreateProcessNodeHandler, new object[] { 3, vectorPos });
                    menu.AddItem(new GUIContent("Process/[4]"), false, OnCreateProcessNodeHandler, new object[] { 4, vectorPos });
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }
            else//left mouse
            {
                Nodes.ForEach((nod) => nod.UpdateAccessRectReadyState(vectorPos));
            }
        }
        else if(current.type == EventType.MouseDrag)
        {
            if (current.button == 2)
            {
                var n = OnFocusOnNode(vectorPos);
                if(n == null)
                {
                    WindowDrag = current.delta;
                    Nodes.ForEach((item) => {
                        item.rect.position += current.delta;
                    });
                    Repaint();
                }
            }
           

        }
        else if (current.type == EventType.MouseUp)
        {
            AccessNode.AccessRect rect = null;
            Nodes.ForEach((node)=> 
            {
                var rst = node.FindReadyStateAccessRect();
                if (rst != null)
                    rect = rst;
            });
            if (rect != null)
            {
                foreach(var node in Nodes)
                {
                    var mouseUpRect = node.GetMouseUpAccessRect(vectorPos);
                    if (mouseUpRect != null && mouseUpRect.Target.Target != rect.Target.Target)
                    {                       
                        if (rect.Type == AccessNode.AccessRect.RectType.FieldOut
                            && mouseUpRect.Type == AccessNode.AccessRect.RectType.FieldIn ||
                            rect.Type == AccessNode.AccessRect.RectType.FlowOut
                            && mouseUpRect.Type == AccessNode.AccessRect.RectType.FlowIn)
                        {
                            rect.Target.AddNext(mouseUpRect.Target);
                        }                           
                        else if (rect.Type == AccessNode.AccessRect.RectType.FieldIn
                            && mouseUpRect.Type == AccessNode.AccessRect.RectType.FieldOut ||
                            rect.Type == AccessNode.AccessRect.RectType.FlowIn
                            && mouseUpRect.Type == AccessNode.AccessRect.RectType.FlowOut)
                        {
                            rect.Target.AddPrev(mouseUpRect.Target);
                        }                                            
                        break;
                    }
                }
            }            
            Nodes.ForEach((nod) => nod.ResetAllReadyState2Zero());
        }
    }

    void OnCreateGetVarNodeHandler(object pos)
    {
        GraphData.AddNode(new GetVarNode((Vector2)pos));
    }
    void OnCreateNewVarNodeHandler(object pos)
    {
        GraphData.AddNode(new SetVarNode((Vector2)pos));
    }
    void OnCreateFunctionNodeHandler(object obj)
    {
        int count = (int)((object[])obj)[0]; ;
        Vector2 pos =(Vector2)((object[])obj)[1];
        GraphData.AddNode(new FuncNode(count,pos));
    }
    void OnCreateProcessNodeHandler(object obj)
    {
        int count = (int)((object[])obj)[0]; ;
        Vector2 pos = (Vector2)((object[])obj)[1];
        GraphData.AddNode(new ProcNode(count, pos));
    }
    void OnDeleteNodeHandler(object obj)
    {
        VisualNode node = obj as VisualNode;
        Nodes.Remove(node);
        node.Remove();
    }
    void OnCreateAddHandler(object obj)
    {
        GraphData.AddNode(new AddOpNode((Vector2)obj));
    }
    void OnCreateMinusHandler(object obj)
    {
        GraphData.AddNode(new MinusOpNode((Vector2)obj));
    }
    void OnCreateMultiplyHandler(object obj)
    {
        GraphData.AddNode(new MultiplyOpNode((Vector2)obj));
    }
    void OnCreateDivisionHandler(object obj)
    {
        GraphData.AddNode(new DivisionOpNode((Vector2)obj));
    }
    void InitWindow()
    {
        Nodes.ForEach((node) => node.DrawConn(Event.current.mousePosition));
        Nodes.ForEach((node) => node.DrawPorts());      
        BeginWindows();
        Nodes.ForEach((node) => node.DrawWindow());
        EndWindows();
        Repaint();
    }


    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        WindowOffset += WindowDrag * 0.5f;
        Vector3 newOffset = new Vector3(WindowOffset.x % gridSpacing, WindowOffset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);

        for (int j = 0; j < heightDivs; j++)
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);

        Handles.color = Color.white;
        Handles.EndGUI();
    }
}
