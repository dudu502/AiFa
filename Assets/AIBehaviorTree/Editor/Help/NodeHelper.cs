using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using AIBehaviorTree;
using System.Reflection.Emit;

public class NodeHelper : EditorWindow
{
    [MenuItem("Window/NodeHelper")]
    static void ShowEditor()
    {
        NodeHelper editor = GetWindow<NodeHelper>();
        editor.Show();
        editor.Init();
    }

    List<NodeHelpData> Nodes;

    private void Init()
    {
        Nodes =  AIBehaviorTree.AI.CreateHelpDataFromAssembly();
    }

    private void OnGUI()
    {
        if(Nodes!=null)
            InitWindow(Nodes);
    }
    string[] GetAllClassNames()
    {
        string[] rst = new string[Nodes.Count];
        for (int i = 0; i < Nodes.Count; ++i)
            rst[i] = Nodes[i].ClassName + string.Format(" [{0}]", Nodes[i].ClassDescribe);
        return rst;
    }
    void InitWindow(List<NodeHelpData> datas)
    {
        DrawNodeWindow();
    }
    private Vector2 WindowScrollPos;
    int selectIndex = 0;
    void DrawNodeWindow()
    {
        WindowScrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height),
        WindowScrollPos, new Rect(0, 0, 200, 5000));

        selectIndex = EditorGUILayout.Popup("All", selectIndex, GetAllClassNames());
        var data = Nodes[selectIndex];
        GUI.color = Color.gray;
        EditorGUILayout.BeginVertical("box");
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Class Name");
        EditorGUILayout.LabelField(data.ClassName);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Class Description");
        EditorGUILayout.LabelField(data.ClassDescribe);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.EndVertical();
        GUI.color = Color.white;
        foreach (var field in data.Fields)
        {
            GUI.color = Color.cyan;
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Field Name");
            EditorGUILayout.LabelField(field.Field.Name);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Field Type");
            EditorGUILayout.LabelField(field.Field.FieldType.Name);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Field Description");
            EditorGUILayout.LabelField(field.FieldDescribe);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUI.color = Color.white;
        }
       
        foreach(var prop in data.Propertys)
        {
            GUI.color = Color.green;
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Property Name");
            EditorGUILayout.LabelField(prop.Property.Name);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Property Type");
            EditorGUILayout.LabelField(prop.Property.PropertyType.Name);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Property Description");
            EditorGUILayout.LabelField(prop.PropertyDescribe);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUI.color = Color.white;
        }

        foreach(var func in data.Funcs)
        {
            GUI.color = Color.yellow;
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Function Name");
            EditorGUILayout.LabelField(func.Func.ToString());
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Function Return Type");
            EditorGUILayout.LabelField(func.Func.ReturnType.ToString());
            EditorGUILayout.EndHorizontal();
            if(func.Func.GetParameters().Length>0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Function Parameters");
                string paraStr = "";
                foreach(var para in func.Func.GetParameters())
                {
                    paraStr += string.Format("[{0}] ", para.ToString());
                }
                
                EditorGUILayout.LabelField(paraStr);
                EditorGUILayout.EndHorizontal();
            }
            

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Function Describe");
            EditorGUILayout.LabelField(func.FuncDescribe);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
            GUI.color = Color.white;
        }

        GUI.EndScrollView();
    }
}
