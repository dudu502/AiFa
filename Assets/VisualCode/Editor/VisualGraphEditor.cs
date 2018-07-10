using System;
using System.Collections;
using System.Collections.Generic;
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
    }
    public static Vector2 WindowScrollPos = Vector2.zero;
    List<VisualNode> Nodes = new List<VisualNode>();

    

    private void OnGUI()
    {
        DrawMenuBar();
        ProcessEvent(Event.current);
        WindowScrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height),
        WindowScrollPos, new Rect(0, 0, 10000, 10000));             
        InitWindow();
        GUI.EndScrollView();
    }
    private void DrawMenuBar()
    {
        GUILayout.BeginArea(new Rect(0, 0, position.width, 20f), EditorStyles.toolbar);
        GUILayout.BeginHorizontal();
        GUILayout.Space(5);
        GUILayout.Button(new GUIContent("Save"), EditorStyles.toolbarButton, GUILayout.Width(35));
        GUILayout.EndHorizontal();
        GUILayout.EndArea();
    }
    private void Update()
    {
        
    }
    private void ProcessEvent(Event current)
    {
        foreach (var nod in Nodes)
            nod.ProcessEvent(current);
        if (current.type == EventType.MouseDown)
        {
            if (current.button == 1)//right mouse
            {
                var menu = new GenericMenu();
                menu.AddItem(new GUIContent("Create Var"), false, OnCreateNewVarNodeHandler, current.mousePosition + WindowScrollPos);
                menu.AddItem(new GUIContent("Get Var"), false, OnGetVarNodeHandler, current.mousePosition + WindowScrollPos);

                menu.ShowAsContext();
                Event.current.Use();
            }
            else//left mouse
            {
                Nodes.ForEach((nod) => nod.OnMouseDownInOutRect(current.mousePosition + WindowScrollPos));
            }
        }
        else if (current.type == EventType.MouseUp)
        {
            var outField = GetReadyOutStateField();
            if (outField != null)
            {
                foreach(var nod in Nodes)
                {
                    var rst = nod.GetAngInStateContainPos(current.mousePosition + WindowScrollPos);
                    if(rst != null&&outField.Target!=rst.Target)
                    {
                        outField.OutRect.State = rst.InRect.State = 2;
                        outField.AddNext(rst);
                        break;
                    }
                }
            }

            var inField = GetReadyInStateField();
            if (inField != null)
            {
                foreach (var nod in Nodes)
                {
                    var rst = nod.GetAngOutStateContainPos(current.mousePosition + WindowScrollPos);
                    if (rst != null&&inField.Target!=rst.Target)
                    {
                        inField.InRect.State = rst.OutRect.State = 2;
                        inField.AddPrev(rst);
                        break;
                    }
                }
            }

            var flowOut = GetReadyOutStateFlow();
            if (flowOut != null)
            {
                foreach(var nod in Nodes)
                {
                    var rst = nod.GetFlowAngInStateContainPos(current.mousePosition + WindowScrollPos);
                    if(rst != null && flowOut.Target != rst.Target)
                    {
                        flowOut.OutRect.State = rst.InRect.State = 2;
                        flowOut.AddNext(rst);
                    }
                }
            }

            var flowIn = GetReadyInStateFlow();
            if(flowIn!=null)
            {
                foreach(var nod in Nodes)
                {
                    var rst = nod.GetFlowAngOutStateContainPos(current.mousePosition + WindowScrollPos);
                    if(rst != null && flowIn.Target != rst.Target)
                    {
                        flowIn.InRect.State = rst.OutRect.State = 2;
                        flowIn.AddPrev(rst);
                    }
                }
            }

            Nodes.ForEach((nod) => nod.ResetAllReadyState2Zero());
        }
    }

    FieldNode GetReadyOutStateField()
    {
        foreach(var node in Nodes)
        {
            foreach(var field in node.fields)
            {
                if (field.OutRect.State == 1)
                    return field;
            }
        }
        return null;
    }
    
    FieldNode GetReadyInStateField()
    {
        foreach (var node in Nodes)
        {
            foreach (var field in node.fields)
            {
                if (field.InRect.State == 1)
                    return field;
            }
        }
        return null;
    }
    FlowNode GetReadyInStateFlow()
    {
        foreach(var node in Nodes)
        {
            if (node.currentFlow.InRect.State == 1)
                return node.currentFlow;
        }
        return null;
    }

    FlowNode GetReadyOutStateFlow()
    {
        foreach (var node in Nodes)
        {
            if (node.currentFlow.OutRect.State == 1)
                return node.currentFlow;
        }
        return null;
    }
    void OnGetVarNodeHandler(object pos)
    {
        Nodes.Add(new GetVarNode((Vector2)pos));
    }
    void OnCreateNewVarNodeHandler(object pos)
    {
        Nodes.Add(new SetVarNode((Vector2)pos));
    }
    void InitWindow()
    {
        Nodes.ForEach((node) => node.DrawConn(Event.current.mousePosition));
        Nodes.ForEach((node) => node.DrawPorts());      
        BeginWindows();
        Nodes.ForEach((node) => node.DrawWindow());
        EndWindows();
    }
}
