using Eleon.Modding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GalacticWaez
{
    public class FileDataSource : IFileDataSource, IGalaxyStorage
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

        /// <summary>
        /// Attempts to load galaxy data from the underlying file 
        /// (available via <see cref="PathToFile"/>).
        /// </summary>
        /// <returns>
        /// star positions, or <c>null</c> if the file does not exist or any
        /// error occurs.
        /// </returns>
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
                if (count == starPositions.Count)
                {
                    Log($"Loaded {count} star positions from file.");
                    return starPositions;
                }
                Log($"Bad file: expected {count} star positions but found {starPositions.Count}.");
                return null;
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

        /// <summary>
        /// Writes <c>positions</c> to the underlying file
        /// (available via <see cref="PathToFile"/>).
        /// </summary>
        /// <param name="positions"></param>
        /// <returns>
        /// <c>true</c> on success, <c>false</c> on failure
        /// </returns>
        public bool StoreGalaxyData(IEnumerable<VectorInt3> positions)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(PathToFile));
            StreamWriter writer = null;
            int count = positions.Count();

            try
            {
                writer = new StreamWriter(new FileStream(
                    PathToFile, FileMode.Create, FileAccess.Write
                    ));

                writer.WriteLine(count);
                foreach (var pos in positions)
                {
                    writer.WriteLine($"{pos.x},{pos.y},{pos.z}");
                }
                Log($"Successfully wrote {count} positions to {PathToFile}");
                return true;
            }
            catch (Exception)
            {
                Log($"Failed to write star positions to {PathToFile}");
                return false;
            }
            finally
            {
                writer?.Close();
            }
        }
    }
}
