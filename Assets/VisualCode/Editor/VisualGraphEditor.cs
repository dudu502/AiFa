using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using VisualCoding;

public class VisualGraphEditor : EditorWindow
{
    [MenuItem("VisualCode/Editor")]
    static void ShowEditor()
    {
        VisualGraphEditor editor = GetWindow<VisualGraphEditor>();
        editor.Show();
        editor.Init();
    }

    private void Init()
    {
        node = new VisualGraphNode();
        node.m_DataBase = new VariableData() { m_DataName="value",m_DataType = typeof(int).ToString()};
    }
    VisualGraphNode node;

    private void OnGUI()
    {
        BeginWindows();
        InitWindow(node);
        EndWindows();
    }

    void InitWindow(VisualGraphNode node)
    {
        string title = node.m_DataBase.ToTitle();
        node.m_DataBase.m_Rect = new Rect(node.m_DataBase.m_Rect.x,node.m_DataBase.m_Rect.y,280,10);
        node.m_DataBase.m_Rect = GUILayout.Window(node.GetHashCode(),node.m_DataBase.m_Rect,node.m_DataBase.Draw,new GUIContent(title),GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true));



    }
}
