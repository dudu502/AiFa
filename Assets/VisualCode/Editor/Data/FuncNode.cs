using UnityEngine;
using System.Collections;
namespace VisualCode
{


    public class FuncNode : VisualNode
    {
        public FuncNode(Vector2 pos) : base(pos)
        {

        }

        protected override void DrawWindowFunc(int id)
        {
            base.DrawWindowFunc(id);
        }
        protected override string GetTitle()
        {
            return "Func";
        }
    }
}