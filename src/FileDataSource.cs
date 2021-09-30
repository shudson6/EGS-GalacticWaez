using Eleon.Modding;
using System;
using System.Collections.Generic;
using System.IO;

namespace GalacticWaez
{
    public class FileDataSource : IFileDataSource
    {
        public const string ModContentDir = "Content\\Mods\\GalacticWaez";
        private const string FileName = "stardata.csv";

        public string PathToFile { get; }
        private readonly LoggingDelegate Log;

        public FileDataSource(string saveGameDir, LoggingDelegate logger)
        {
            Log = logger ?? delegate { };
            PathToFile = $"{saveGameDir}\\{ModContentDir}\\{FileName}";
        }

        public IEnumerable<VectorInt3> GetGalaxyData()
        {
            if (!File.Exists(PathToFile))
            {
                Log("Star data not present in save file.");
                return null;
            }

            StreamReader reader = null;
            try
            {
                reader = new StreamReader(new FileStream(
                    PathToFile, FileMode.Open, FileAccess.Read
                    ));

                int count = int.Parse(reader.ReadLine());
                var starPositions = new List<VectorInt3>(count);
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    var coords = line.Split(separator: ",".ToCharArray(), count: 3);
                    starPositions.Add(new VectorInt3(
                        int.Parse(coords[0]),
                        int.Parse(coords[1]),
                        int.Parse(coords[2])
                        ));
                }
                return (count == starPositions.Count) ? starPositions : null;
            }
            catch (Exception ex)
            {
                Log("Exception while loading star data: ");
                Log(ex.Message);
                return null;
            }
            finally
            {
                reader?.Close();
            }
        }
    }
}
