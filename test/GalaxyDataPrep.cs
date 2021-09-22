using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using Eleon.Modding;
using GalacticWaez;

namespace GalacticWaezTests
{
    [TestClass]
    [DeploymentItem("Dependencies\\stardata-test-large.csv")]
    public class GalaxyDataPrep
    {
        public static IEnumerable<VectorInt3> LoadPositions(string starDataFile)
        {
            var reader = new StreamReader(new FileStream(
                starDataFile, FileMode.Open, FileAccess.Read
                ));
            var data = new List<VectorInt3>();
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var coords = line.Split(',');
                data.Add(new VectorInt3(
                    int.Parse(coords[0]),
                    int.Parse(coords[1]),
                    int.Parse(coords[2])
                    ));
            }
            return data;
        }
    }
}
