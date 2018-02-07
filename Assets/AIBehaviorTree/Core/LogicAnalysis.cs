using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
namespace AIBehaviorTree
{
    public class LogicAnalysis
    {
        /// <summary>
        /// 逻辑语句
        /// 识别的运算符有
        /// ==  !=  >   <   >=  <=
        /// </summary>
        public class LogicStatement
        {
            public class LogicVariable
            {
      
                public string m_Value;
                public LogicVariable(string value)
                {
                    m_Value = value;
                }
                public int GetVariableResultTypeValue()
                {
                    if (IsBoolean())
                        return 1;
                    if (IsNumberic())
                        return 2;
                    return 4;
                }
              
                public bool IsNumberic()
                {
                    return Regex.IsMatch(m_Value, @"^[+-]?\d*[.]?\d*$");
                }
                public float Parse2Number() 
                {
                    return float.Parse(m_Value);
                }
                public bool IsBoolean()
                {
                    return m_Value == "true" || m_Value == "false";
                }
                public bool Parse2Boolean()
                {
                    return bool.Parse(m_Value);
                }
                public bool IsHandler()
                {
                    return !IsNumberic() && !IsBoolean();
                }
                public string Parse2Handler()
                {
                    return m_Value;
                }
                
            }
            public LogicVariable m_Left;
            /// <summary>
            /// ==  !=  >   <   >=  <=
            /// </summary>
            public string m_Operation;
            public LogicVariable m_Right;

            public string m_Expression;
            public LogicStatement(string expression)
            {
                m_Expression = expression;
                Init();
            }
          
            public bool Execute(object obj)
            {
                int leftType = m_Left.GetVariableResultTypeValue();
                int rightType = m_Right.GetVariableResultTypeValue();

                object leftResult = null;
                object rightResult = null;
                if (leftType == 4)
                    leftResult = obj.ExecuteMember(m_Left.Parse2Handler());
                else if (leftType == 2)
                    leftResult = m_Left.Parse2Number();
                else if (leftType == 1)
                    leftResult = m_Left.Parse2Boolean();

                if (rightType == 4)
                    rightResult = obj.ExecuteMember(m_Right.Parse2Handler());
                else if (rightType == 2)
                    rightResult = m_Right.Parse2Number();
                else if (rightType == 1)
                    rightResult = m_Right.Parse2Boolean();

                bool result = false;
                int minType = Math.Min(leftType, rightType);
                if(minType == 4)
                {
                    if(m_Operation == "==")
                    {
                        result = leftResult == rightResult;
                    }
                    else if(m_Operation == "!=")
                    {
                        result = leftResult != rightResult;
                    }
                    else
                    {
                        throw new Exception("无法操作" + minType + m_Operation);
                    }
                }
                else if(minType == 2)
                {
                    if (m_Operation == "==")
                    {
                        result = float.Parse(leftResult.ToString()) == float.Parse(rightResult.ToString());
                    }
                    else if (m_Operation == "!=")
                    {
                        result = float.Parse(leftResult.ToString()) != float.Parse(rightResult.ToString());
                    }
                    else if(m_Operation == ">")
                    {
                        result = float.Parse(leftResult.ToString()) > float.Parse(rightResult.ToString());
                    }
                    else if(m_Operation == ">=")
                    {
                        result = float.Parse(leftResult.ToString()) >= float.Parse(rightResult.ToString());
                    }
                    else if(m_Operation == "<")
                    {
                        result = float.Parse(leftResult.ToString()) < float.Parse(rightResult.ToString());
                    }
                    else if(m_Operation == "<=")
                    {
                        result = float.Parse(leftResult.ToString()) <= float.Parse(rightResult.ToString());
                    }
                    else
                    {
                        throw new Exception("无法操作" + minType + m_Operation);
                    } 
                }
                else
                {
                    if (m_Operation == "==")
                    {
                        result = (bool)leftResult == (bool)rightResult;
                    }
                    else if (m_Operation == "!=")
                    {
                        result = (bool)leftResult != (bool)rightResult;
                    }
                    else
                    {
                        throw new Exception("无法操作" + minType + m_Operation);
                    }
                }
                return result;
            }
            void Init()
            {
                int index = 0;
                string oper = "";
                while(index < m_Expression.Length)
                {
                    char c = m_Expression[index];
                    ++index;
                    if(c == '=')
                    {
                        oper += c;
                        if (oper == "==" || oper == "!=" || oper == ">=" || oper == "<=")
                        {
                            m_Operation = oper;
                        }                    
                    }
                    else if (c == '!' || c == '>' || c == '<')
                    {
                        oper += c;
                        m_Operation = oper;
                    }
                    else
                    {
                        oper = "";
                    }                  
                }
                if (string.IsNullOrEmpty(m_Operation))
                    throw new Exception("m_Operation error");
                string[] e = m_Expression.Split(new string[] { m_Operation }, StringSplitOptions.RemoveEmptyEntries);
                if(e.Length != 2)
                    throw new Exception("vars error");
                m_Left = new LogicVariable(e[0]);
                m_Right = new LogicVariable(e[1]);
            }
        }

        List<LogicStatement> m_QExpression = new List<LogicStatement>();
        List<string> m_QLogic = new List<string>();
        string m_Code;
        public LogicAnalysis(string code)
        {
            m_Code = code;
            Init();
        }
        public bool Execute(object obj)
        {
            if (m_QExpression.Count == 0) throw new Exception("表达式空");
            LogicStatement state = m_QExpression[0];
            bool result = state.Execute(obj);
            int index = 0;
            while (index < m_QLogic.Count)
            {
                string logic = m_QLogic[index];
                if(logic == "||")
                {
                    result = result || m_QExpression[index+1].Execute(obj);
                }
                else//&&
                {
                    result = result && m_QExpression[index+1].Execute(obj);
                }
                ++index;
            }
            return result;
        }
        public void Init()
        {
            int index = 0;
            string or = "";
            string and = "";
            string expression = "";
            while (index < m_Code.Length)
            {
                char c = m_Code[index];
                ++index;
                if (c == 32 || c == 10)
                {
                    continue;
                }
                if (c == '|')
                {
                    or += c;
                    if (or == "||")
                    {
                        m_QLogic.Add(or);
                        or = "";
                        m_QExpression.Add(new LogicStatement(expression));
                        expression = "";
                    }
                }
                else if (c == '&')
                {
                    and += c;
                    if (and == "&&")
                    {
                        m_QLogic.Add(and);
                        and = "";
                        m_QExpression.Add(new LogicStatement(expression));
                        expression = "";
                    }
                }
                else if (c == '#')
                {
                    m_QExpression.Add(new LogicStatement(expression));
                    expression = "";
                }
                else
                {
                    expression += c;
                }
            }
        }
    }
}