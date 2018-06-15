using UnityEngine;
using System.Collections;
using System;
using System.Threading;

namespace ECS
{

    public class MutipleThreadResetEvent : IDisposable
    {
        private readonly ManualResetEvent done;
        private readonly int total;
        private long current;
        public MutipleThreadResetEvent(int total)
        {
            this.total = total;
            current = total;
            done = new ManualResetEvent(false);
        }
        public void Reset()
        {
            current = total;
            done.Reset();
        }
        /// <summary>
        /// 唤醒一个等待的线程
        /// </summary>
        public void SetOne()
        {
            // Interlocked 原子操作类 ,此处将计数器减1
            if (Interlocked.Decrement(ref current) == 0)
            {
                //当所以等待线程执行完毕时，唤醒等待的线程
                done.Set();
            }
        }

        /// <summary>
        /// 等待所以线程执行完毕
        /// </summary>
        public void WaitAll()
        {
            done.WaitOne();
        }

        /// <summary>
        /// 释放对象占用的空间
        /// </summary>
        public void Dispose()
        {
            ((IDisposable)done).Dispose();
        }
    }
}