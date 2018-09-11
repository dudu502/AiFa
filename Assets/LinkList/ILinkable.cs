using UnityEngine;
using System.Collections;

/// <summary>
/// 链表节点接口
/// </summary>
public interface ILinkable
{
    ILinkable Next { set; get; }
    ILinkable Prev { set; get; }
}
public static class LinkableExtends
{
    /// <summary>
    /// 是否是链表的头
    /// </summary>
    /// <param name="source">查询当前节点是否是链表头</param>
    /// <returns></returns>
    public static bool IsHead(this ILinkable source)
    {
        return source.Prev == null;
    }

    /// <summary>
    /// 是否是链表的尾
    /// </summary>
    /// <param name="source">查询当前节点是否是链表尾</param>
    /// <returns></returns>
    public static bool IsTail(this ILinkable source)
    {
        return source.Next == null;
    }

    /// <summary>
    /// 获得链表的头
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ILinkable GetHead(this ILinkable source)
    {      
        ILinkable result = source;
        while(result.Prev!=null)
            result = source.Prev;
        return result;
    }

    /// <summary>
    /// 获得链表的尾
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ILinkable GetTail(this ILinkable source)
    {
        ILinkable result = source;
        while (result.Next != null)
            result = source.Prev;
        return result;
    }

    /// <summary>
    /// 在当前节点下添加节点
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    public static void AddNext(this ILinkable source,ILinkable value)
    {
        source.Next = value;
        value.Prev = source;
    }


    /// <summary>
    /// 在当前节点前添加节点
    /// </summary>
    /// <param name="source"></param>
    /// <param name="value"></param>
    public static void AddPrev(this ILinkable source,ILinkable value)
    {
        value.Next = source;
        source.Prev = value;
    }
    
    /// <summary>
    /// 移除当前节点
    /// </summary>
    /// <param name="source"></param>
    public static void Remove(this ILinkable source)
    {
        if (source.Prev!= null)
            source.Prev.Next = source.Next;
        if (source.Next != null)
            source.Next.Prev = source.Prev;
        source = null;
    }
}