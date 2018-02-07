using System.Collections;

/// <summary>
/// List Iterator 
/// list 迭代器
/// 2015年6月29日 17:07:01 修改
/// 2015年12月25日 17:38:41 修改 （MoveNext,MovePrev）
/// 2016年1月6日 15:57:37 修改 ForEach
/// </summary>
public class ListIterator
{
    /// <summary>
    /// 默认索引0
    /// </summary>
    private int currentIndex = 0;
    /// <summary>
    /// IList 类型的源
    /// </summary>
    private IList source = null;
    /// <summary>
    /// 构造
    /// </summary>
    /// <param name="source"></param>
    public ListIterator(IList source)
    {
        SetSource(source);
    }

    /// <summary>
    /// 设置源
    /// </summary>
    /// <param name="source"></param>
    public void SetSource(IList source)
    {
        this.Reset();
        this.source = source;
    }
    /// <summary>
    /// 获取源
    /// </summary>
    /// <returns></returns>
    public IList GetSource()
    {
        return source;
    }
    /// <summary>
    /// 获取头
    /// </summary>
    /// <returns></returns>
    public object GetHead()
    {
        Reset();
        if (source.Count > 0)
        {
            return source[0];
        }
        return null;
    }
    /// <summary>
    /// 获取尾
    /// </summary>
    /// <returns></returns>
    public object GetTail()
    {
        return source[GetSize() - 1];
    }
    /// <summary>
    /// 长度
    /// </summary>
    /// <returns></returns>
    public int GetSize()
    {
        return source.Count;
    }
    /// <summary>
    /// 获取下一个 并移动索引
    /// </summary>
    /// <returns></returns>
    public object GetNext()
    {
        object result = null;
        if (HasNext())
        {
            currentIndex++;
            result = source[currentIndex];
        }
        return result;
    }

    /// <summary>
    /// 再有下一个的情况下 往后移动索引
    /// </summary>
    public void MoveNext()
    {
        if (HasNext())
            currentIndex++;
    }

    /// <summary>
    /// 获取前一个 并移动索引
    /// </summary>
    /// <returns></returns>
    public object GetPrev()
    {
        object result = null;
        if (HasPrev())
        {
            currentIndex--;
            result = source[currentIndex];
        }
        return result;
    }

    /// <summary>
    /// 再有前一个的情况下 往前移动索引
    /// </summary>
    public void MovePrev()
    {
        if (HasPrev())
            currentIndex--;
    }
    /// <summary>
    /// 是否有下一个
    /// </summary>
    /// <returns></returns>
    public bool HasNext()
    {
        int nextIndex = currentIndex + 1;
        return nextIndex <= GetSize() - 1;
    }
    /// <summary>
    /// 是否有前一个
    /// </summary>
    /// <returns></returns>
    public bool HasPrev()
    {
        return currentIndex > 0;
    }
    /// <summary>
    /// 是否结束
    /// </summary>
    /// <returns></returns>
    public bool IsEnd()
    {
        return GetCurrent() == GetTail();
    }
    /// <summary>
    /// 是否开始
    /// </summary>
    /// <returns></returns>
    public bool IsBegin()
    {
        return GetCurrent() == GetHead();
    }
    /// <summary>
    /// 获得当前
    /// </summary>
    /// <returns></returns>
    public object GetCurrent()
    {
        return source[currentIndex];
    }
    /// <summary>
    /// 重置索引
    /// </summary>
    public void Reset()
    {
        currentIndex = 0;
    }
    /// <summary>
    /// 设置索引
    /// </summary>
    /// <param name="value"></param>
    public void SetCurrentIndex(int value)
    {
        currentIndex = value;
    }
    /// <summary>
    /// 获得索引
    /// </summary>
    /// <returns></returns>
    public int GetCurrentIndex()
    {
        return currentIndex;
    }

    /// <summary>
    /// 遍历 下标移至最后
    /// </summary>
    /// <param name="forAction"></param>
    public void ForEach(System.Action<int,object> forAction)
    {
        for (int i = 0; i < GetSize(); ++i)
            forAction(i,source[i]);
    }
}
