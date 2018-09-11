using UnityEngine;
using System.Collections;
namespace Game.Core
{


    public class TestActivity : Activity
    {

        public override UIRoot.Layer GetLayer()
        {
            return UIRoot.Layer.Game;
        }

        protected override string GetActivityFullName()
        {
            return "TestPanel";
        }
    }
}
