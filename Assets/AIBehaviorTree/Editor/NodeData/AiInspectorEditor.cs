﻿using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using AIBehaviorTree;

[CustomEditor(typeof(AI))]
public class AiInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var aiScript = target as AI; 
        EditorGUILayout.BeginVertical("box");
        base.OnInspectorGUI();
        if (aiScript.m_TextDataAiTree != null && GUILayout.Button("Open NodeEditor"))
            NodeEditor.ShowEditor(aiScript.m_TextDataAiTree);
        EditorGUILayout.EndVertical();
        
        var nodes = aiScript.GetExecutingNodes();
        if (nodes.Count > 0)
        {
            EditorGUILayout.BeginVertical("box");
            foreach (var node in nodes)
            {
                foreach (var genNode in node.GetGenerations())
                {
                    GUI.color = NodeGraph.GetColorByType(genNode.Config.type);
                    EditorGUILayout.BeginVertical("box");
                    EditorGUILayout.LabelField("Name", genNode.Config.name);
                    EditorGUILayout.LabelField("Script", genNode.Config.scriptName);
                    if (!string.IsNullOrEmpty(genNode.Config.scriptName))
                    {
                        if (GUILayout.Button("Edit Script"))
                            Process.Start(Application.dataPath + AI.SCRIPT_OUTPUTPATH + genNode.Config.scriptName + ".txt");
                    }
                    EditorGUILayout.EndVertical();
                    GUI.color = Color.white;
                }
            }
            EditorGUILayout.EndVertical();
        }
    }
}