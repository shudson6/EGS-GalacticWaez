using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using GalacticWaez.Navigation;

namespace GalacticWaezTests
{
    [TestClass]
    public class MinheapTests
    {
        [TestMethod]
        public void Empty_Heap_RemoveMin_Returns_Default()
        {
            var heap = new Minheap<string>();
            var result = heap.RemoveMin();
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Sort_100_Ints()
        {
            var rand = new Random();
            var heap = new Minheap<int>();
            var list = new List<int>();
            for (int i = 0; i < 100; i++)
            {
                int r = rand.Next();
                heap.Insert(r, r);
                list.Add(r);
            }
            list.Sort();

            foreach (int i in list)
            {
                Assert.AreEqual(i, heap.RemoveMin());
            }
        }
    }
}
