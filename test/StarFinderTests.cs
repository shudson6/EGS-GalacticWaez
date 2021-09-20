using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using GalacticWaez;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaezTests
{
    [TestClass]
    public class StarFinderTests
    {
        [TestMethod]
        public void DoesItWork()
        {
            var loaded = new List<StarFinder.StarData>(GalaxyDataPrep.Locations.Count);
            int i = 0;
            foreach (var p in GalaxyDataPrep.Locations)
            {
                loaded.Add(new StarFinder.StarData(0, -1, p.x, p.y, p.z, i));
                i++;
            }
            var found = new StarFinder().Search(new SectorCoordinates(
                loaded[0].x, loaded[0].y, loaded[0].z));
            i = 0;
            foreach (var p in GalaxyDataPrep.Locations)
            {
                Assert.AreEqual(p, found[i]);
                i++;
            }
        }
    }
}
