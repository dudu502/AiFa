using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Diagnostics;




public class CreateXMLDataTool : EditorWindow
{
    [MenuItem("Tools/CreateXMLDataTool")]
    static void DoIt()
    {
        CreateXMLDataTool wnd = (CreateXMLDataTool)EditorWindow.GetWindow(typeof(CreateXMLDataTool));
        wnd.minSize = new Vector2(300, 300);
        wnd.Show();
    }
    
    void OnGUI()
    {      
        if (GUILayout.Button("CreateXML_Data"))
        {
            string bytesFolderPath = Environment.CurrentDirectory + "\\Assets\\AutoConfig\\OutPut\\binary_files";
            DirectoryInfo folder = new DirectoryInfo(bytesFolderPath);
            FileInfo[] files = folder.GetFiles();
            ReadBytes(files);
        }
    }

 
    void ReadBytes(FileInfo[] files)
    {
        foreach (FileInfo f in files)
        {
            if (f.Extension == ".bytes")
            {
                FileStream fs = new FileStream(f.FullName, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                byte[] result = br.ReadBytes((int)fs.Length);
                br.Close();
                fs.Close();
                PtOutBinary outBinary = PtOutBinary.Read(result);
                ReadBinaryAndCreateXmlData(outBinary);
            }
        }
        UnityEngine.Debug.Log(string.Format("[{0}]", "Finish"));
        
    }

    void ReadBinaryAndCreateXmlData(PtOutBinary outBinary)
    {
        LoadConfigDataUtil.TypePair tp = LoadConfigDataUtil.GetTypeByName(outBinary.class_name + "BinaryConfig");



        object main = Activator.CreateInstance(tp.m_TypeMain);

        FieldInfo[] fies =  tp.m_TypeMain.GetFields();

        IList list = Activator.CreateInstance(fies[0].FieldType) as IList;

        fies[0].SetValue(main, list);

        for (int i = 2; i < outBinary.rows.Count; ++i)
        {
            object child = Activator.CreateInstance(tp.m_TypeChild);
            list.Add(child);
            for (int j = 0; j < outBinary.field_count; ++j)
            {
                string type = outBinary.rows[0].cols[j];
                string name = outBinary.rows[1].cols[j];
                string value = outBinary.rows[i].cols[j];

                FieldInfo field = tp.m_TypeChild.GetField(name);
                if (field != null)
                {
                    if (type == "int")
                    {
                        field.SetValue(child, value == "" ? 0 : int.Parse(value));
                    }
                    else if(type == "string")
                    {
                        field.SetValue(child, value);
                    }
                    else if(type == "float")
                    {
                        field.SetValue(child, value == "" ? 0 : float.Parse(value));
                    }
                    else
                    {
                        throw new Exception("Field Type Error");
                    }
                }
            }
        }


        MethodInfo mt_Write = tp.m_TypeMain.GetMethod("Write");

        FileStream outFs = new FileStream(Environment.CurrentDirectory + string.Format("\\Assets\\AutoConfig\\Resources\\StaticConfigs\\{0}_Final.bytes", outBinary.class_name), FileMode.Create);
        BinaryWriter bw = new BinaryWriter(outFs);
        bw.Write(mt_Write.Invoke(null, new object[] { main }) as byte[]);
        bw.Flush();
        bw.Close();
        outFs.Close();

        UnityEngine.Debug.Log(string.Format("[{0}]", outBinary.class_name + " OK"));
        AssetDatabase.Refresh();
    }



}






