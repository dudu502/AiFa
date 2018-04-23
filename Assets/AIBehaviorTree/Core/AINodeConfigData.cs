using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using LitJson;
using UnityEngine;

namespace AIBehaviorTree
{
   public class AINodeConfigData
    {
        /// <summary>
        /// 1 顺序控制节点 （顺序执行）
        /// 2 随机选择控制节点（选择一个节点执行之后结束）
        /// 3 并发节点 （同时）
        /// 0 执行节点
        /// </summary>
        public int type = 0;

        /// <summary>
        /// 权重 type==2时 子节点的weight起作用
        /// weight 越大随机到的几率越大
        /// </summary>
        public int weight = 1;

        /// <summary>
        /// 名称
        /// </summary>
        public string name = "";

        /// <summary>
        /// 脚本名称
        /// </summary>
        public string scriptName = "";

        /// <summary>
        /// 子节点
        /// </summary>
        /// <returns></returns>
        [ListElement(typeof(AINodeConfigData))]
        public List<AINodeConfigData> listAINodeConfigData = new List<AINodeConfigData>();


        /// <summary>
        /// 脚本内容 
        /// </summary>
        public string script;

        public override string ToString()
        {
            return string.Format("[Type={0},Name={1},ScriptName={2}],ChildrenCount={3}]",type,name,scriptName,listAINodeConfigData.Count);
        }

        /// <summary>
        /// 通过AINodeConfigData根节点生成AINode
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="owner"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static AINode Create(AINodeConfigData data,AI ai)
        {
            AINode node = null;
            TextAsset txt = Resources.Load<TextAsset>(data.scriptName);
            data.script = txt.text;
            if (data.listAINodeConfigData.Count > 0)
            {
                node = new ControlNode(ai, data);
                foreach (var c in data.listAINodeConfigData)
                {
                    var child = Create(  c, ai);
                    node.Add(child);
                }
            }
            else
            {
                node = new ActionNode(ai, data);
            }
            return node;
        }


        public static AINode Create(byte[] bytes, AI ai)
        {
            AINode node = null;
            ByteBuffer buffer = new ByteBuffer(bytes);
            AINodeConfigData config = new AINodeConfigData();
            config.name = buffer.ReadString();
            config.type = buffer.ReadByte();
            config.scriptName = buffer.ReadString();
            config.weight = buffer.ReadInt32();
            buffer.ReadFloat();
            buffer.ReadFloat();
            if (config.scriptName != "")
            {
                TextAsset txt = Resources.Load<TextAsset>(config.scriptName);
                config.script = txt.text;
            }
            else
            {
                config.script = "function detect()return true end";
            }
            int count = buffer.ReadInt32();
            if (count > 0)
            {
                node = new ControlNode(ai, config);
                for (int i = 0; i < count; ++i)
                {
                    var child = Create(buffer.ReadBytes(), ai);
                    node.Config.listAINodeConfigData.Add(child.Config);
                    node.Add(child);
                }
            }
            else
            {
                node = new ActionNode(ai, config);
            }
            return node;
        }
        public static AINode Create(JsonData data, AI ai)
        {
            AINode node = null;
            AINodeConfigData config = new AINodeConfigData();
            config.name = data["name"].ToString();
            if (data.Keys.Contains("type"))
                config.type = int.Parse(data["type"].ToString());
            if (data.Keys.Contains("weight"))
                config.weight = int.Parse(data["weight"].ToString());
            if(data.Keys.Contains("scriptName"))
                config.scriptName = data["scriptName"].ToString();
            if(config.scriptName != "")
            {
                TextAsset txt = Resources.Load<TextAsset>(config.scriptName);
                config.script = txt.text;
            }
            else
            {
                config.script = "function detect()return true end";
            }
            if (data["children"].IsArray && data["children"].Count > 0)
            {
                node = new ControlNode(ai, config);
                foreach (JsonData childData in data["children"])
                {
                    var child = Create(childData,ai);
                    node.Config.listAINodeConfigData.Add(child.Config);
                    node.Add(child);
                }
            }
            else
            {
                node = new ActionNode(ai, config);
            }

            return node;
        }
    }
}