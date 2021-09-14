using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using GalacticWaez;

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
    }
}
