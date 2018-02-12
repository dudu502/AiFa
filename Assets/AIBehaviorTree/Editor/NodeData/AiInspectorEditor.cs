using UnityEngine;
using UnityEditor;
using System.Diagnostics;
using AIBehaviorTree;

[CustomEditor(typeof(AI))]
public class AiInspectorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUILayout.BeginVertical("box");
        base.OnInspectorGUI();
        EditorGUILayout.EndVertical();
        var aiScript = target as AIBehaviorTree.AI;
        var nodes = aiScript.GetExecutingNodes();
        if (nodes.Count>0)
        {
            EditorGUILayout.BeginVertical("box");
            foreach (var node in nodes)
            {
                EditorGUILayout.BeginVertical("box");
                EditorGUILayout.LabelField("Name", node.Config.name);
                EditorGUILayout.LabelField("Script", node.Config.scriptName);
                if (!string.IsNullOrEmpty(node.Config.scriptName))
                {
                    if (GUILayout.Button("Edit Script"))
                    {
                        Process.Start(Application.dataPath + AI.SCRIPT_OUTPUTPATH + node.Config.scriptName + ".txt");
                    }
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }
        if (aiScript.m_JsonAiTree != null && GUILayout.Button("Open NodeEditor"))
        {        
            NodeEditor.ShowEditor(aiScript.m_JsonAiTree);          
        }
    }
}