using UnityEngine;
using UnityEditor;
using System.IO;

public class NodeScriptTemplate 
{
    public static void DeleteScript(string path)
    {
        if(EditorUtility.DisplayDialog("提示", "确定删除" + path, "OK"))
        {
            File.Delete(path);
            AssetDatabase.Refresh();
        }      
    }
    public static void NewEmptyScript(string path,NodeGraph node)
    {
        string result = string.Format(@"
--[[
    INFO--{0}
 --]]
",node.ToString());
        if (node.Type == NodeGraph.NODETYPE.ACTION)
        {
            result += @"
function detect()
    return true
end

function enter()

end

function update(dt)
    
end

function trigger(type,obj)

end

function exit()

end
";
        }
        else
        {
            result += @"
function detect()
    return true
end
";
        }

        File.WriteAllText(path, result);
        AssetDatabase.Refresh();
    }
}