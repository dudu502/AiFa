using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class DataCreator : EditorWindow
{

    [MenuItem("Window/DataCreator")]
    static void ShowEditor()
    {
        DataCreator editor = GetWindow<DataCreator>();
        editor.Show();
        editor.Init();
    }

    private void Init()
    {
        
    }
    private void OnGUI()
    {
        DrawNodeWindow();
    }
    private Vector2 WindowScrollPos;
    string dataPath = "/AIBehaviorTree/Resources/DataScriptObjectBase.asset";
    ScriptableObject currentData;
    void DrawNodeWindow()
    {
        WindowScrollPos = GUI.BeginScrollView(new Rect(0, 0, position.width, position.height),
        WindowScrollPos, new Rect(0, 0, 200, 5000));
        EditorGUILayout.BeginHorizontal();
        
        dataPath = EditorGUILayout.TextField("DataPath", dataPath);
                
        if (currentData != null&&GUILayout.Button("Save Data")  )
        {
            AssetDatabase.CreateAsset(currentData, "Assets" + dataPath);
        }
        else if (GUILayout.Button("Create Data"))
        {
            currentData = ScriptableObject.CreateInstance(Path.GetFileNameWithoutExtension(dataPath));
        }
        EditorGUILayout.EndHorizontal();
        GUI.EndScrollView();
    }
    void OnSelectionChange()
    {
        if (Selection.objects.Length > 0)
        {
            
        }
    }
    void DrawDataGrid (DataItemBase item)
    {
        EditorGUILayout.BeginVertical("box");
        Type type = currentData.GetType();

        foreach(FieldInfo field in type.GetFields())
        {
            if(field.FieldType == typeof(int))
            {
                item.SetFieldValue( field.Name, EditorGUILayout.IntField(field.Name, item.GetFieldValue<int>(field.Name)));
            }
            else if(field.FieldType == typeof(float))
            {
                
            }
            else if(field.FieldType == typeof(string))
            {

            }
        }

        EditorGUILayout.EndVertical();
    }
}
