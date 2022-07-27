// Licensed to the AiCodo.com under one or more agreements.
// The AiCodo.com licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.
// 本程序文件开源，遵循MIT开源协议，如有疑问请联系作者（singba@163.com）
// 您可以私用、商用部分或全部代码，修改源码时，请保持原代码的完整性，以免因为版本升级导致问题。
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AiCodo.Tests
{
    internal class ParallelSortQueueTest
    {
        ParallelSortQueue<SortItem> _Queue = null;
        int _DoneCount = 0;
        int _QueueCount = 2000;

        [Test]
        public void TestAddRemove()
        {
            var queue = new ParallelSortQueue<SortItem>(DoItem, 2, new SortItemComparer(), CanDoItem);
            _Queue = queue;
            CreateItems(queue)
                .ContinueWith(t =>
                {
                    queue.Sealed();
                });

            var count = queue.Start().Result;
            Assert.AreEqual(_QueueCount, count);
        }

        private Task CreateItems(ParallelSortQueue<SortItem> queue)
        {
            var id = 0;
            var random = new Random((int)DateTime.Now.Ticks);
            return Task.Run(() =>
            {
                while (id < _QueueCount)
                {
                    id++;
                    queue.AddItem(new SortItem
                    {
                        ID = random.Next(0, 200),
                        Name = $"index:{id + 1}"
                    });
                    Thread.Sleep(100);
                }
            });
        }

        private void DoItem(SortItem item)
        {
            Interlocked.Increment(ref _DoneCount);
            Debug.WriteLine(item.ToString());
        }

        private bool CanDoItem(SortItem item)
        {
            if (_DoneCount > 50)
            {
                return true;
            }
            return item.ID > 100;
        }
    }
    class SortItemComparer : IComparer<SortItem>
    {
        public int Compare(SortItem x, SortItem y)
        {
            return x.ID > y.ID ? 1 : (x.ID == y.ID && x == y) ? 0 : -1;
        }
    }


    public class SortItem : EntityBase
    {
        public int ID { get; set; }

        public string Name { get; set; }

        public override string ToString()
        {
            return $"{ID},{Name}";
        }
    }
}
