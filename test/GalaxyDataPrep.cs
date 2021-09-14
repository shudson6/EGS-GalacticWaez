using System;
using System.IO;
using System.Collections.Generic;
using Eleon.Modding;
using GalacticWaez;

namespace GalacticWaezTests
{
    public class GalaxyDataPrep
    {
        public static IReadOnlyCollection<VectorInt3> Locations { get; }

        static GalaxyDataPrep()
        {
            var reader = new StreamReader(new FileStream(
                "stardata.csv", FileMode.Open, FileAccess.Read));
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
            Locations = data;
        }
    }
}
