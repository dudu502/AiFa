using UnityEngine;
using System.Collections;

namespace VisualCode
{
    public interface ILinkNode
    {
        ILinkNode GetNext();
        void SetNext(ILinkNode value);

        ILinkNode GetPrev();
        void SetPrev(ILinkNode value);
    }
    public static class Extends
    {
        public static void AddNext(this ILinkNode source, ILinkNode value)
        {
            source.SetNext(value);
            value.SetPrev(source);
        }

        public static void AddPrev(this ILinkNode source, ILinkNode value)
        {
            value.SetNext(source);
            source.SetPrev(value);
        }

        public static void Remove(this ILinkNode source)
        {
            if (source.GetPrev() != null)
                source.GetPrev().SetNext(source.GetNext());
            if (source.GetNext() != null)
                source.GetNext().SetPrev(source.GetPrev());
            source = null;
        }
    }
}
