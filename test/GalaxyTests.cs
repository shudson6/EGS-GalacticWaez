﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GalacticWaez;
using GalacticWaez.Navigation;
using Eleon.Modding;

namespace GalacticWaezTests
{
    [TestClass]
    public class GalaxyTests
    {
        private static IEnumerable<VectorInt3> positions;

        [ClassInitialize]
        public static void SetupClass(TestContext _tc)
        {
            positions = GalaxyDataPrep.LoadPositions(_tc.DeploymentDirectory + "\\stardata-test-large.csv");
        }

        [TestMethod]
        [DeploymentItem("Dependencies\\stardata-test-large.csv")]
        public void Has_Expected_Count_Stars_Warplines()
        {
            var buildGalaxy = Task<Galaxy>.Factory.StartNew(() =>
                Galaxy.CreateNew(positions, Const.BaseWarpRange)
                );
            // meanwhile, build our tedious test galaxy
            var testGalaxy = new List<TestNode>(positions.Count());
            foreach (var p in positions)
            {
                testGalaxy.Add(new TestNode(TestNode.LightYearify(p)));
            }
            long edgeCount = 0;
            foreach (var n in testGalaxy)
            {
                foreach (var p in testGalaxy)
                {
                    if (n == p || n.DistanceTo(p) > Const.BaseWarpRange) continue;
                    n.neighbors.Add(p);
                    edgeCount++;
                }
            }
            edgeCount /= 2;
            buildGalaxy.Wait();
            Assert.AreEqual(buildGalaxy.Result.StarCount, testGalaxy.Count);
            Assert.AreEqual(buildGalaxy.Result.WarpLines, edgeCount);
        }
    }

    class TestNode
    {
        public static VectorInt3 LightYearify(VectorInt3 sectorCoords)
        {
            return new VectorInt3(sectorCoords.x / Const.SectorsPerLY,
                sectorCoords.y / Const.SectorsPerLY,
                sectorCoords.z / Const.SectorsPerLY
                );
        }
        public VectorInt3 position;
        public List<TestNode> neighbors;
        public TestNode(VectorInt3 position)
        {
            this.position = position;
            neighbors = new List<TestNode>(100);
        }
        public float DistanceTo(TestNode other)
        {
            return (float)Math.Sqrt(Math.Pow(position.x - other.position.x, 2)
                + Math.Pow(position.y - other.position.y, 2)
                + Math.Pow(position.z - other.position.z, 2)
                );
        }
    }
}
