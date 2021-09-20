using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Eleon.Modding;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez.src
{
    public class StarDataStorage
    {
        public const string SavedContentDir = "Content\\GalacticWaez";
        private const string FileName = "stardata.csv";

        private readonly IModApi modApi;
        private readonly string Dir;
        private readonly string FilePath;

        public StarDataStorage(IModApi modApi)
        {
            this.modApi = modApi;
            Dir = modApi.Application.GetPathFor(AppFolder.SaveGame)
                + $"\\{SavedContentDir}";
            FilePath = $"{Dir}\\{FileName}";
        }

        public bool Exists()
        {
            return File.Exists(FilePath);
        }

        public IEnumerable<SectorCoordinates> Load()
        {
            if (!Exists())
                return null;

            StreamReader reader = null;

            try
            {
                reader = new StreamReader(new FileStream(
                    FilePath, FileMode.Open, FileAccess.Read
                    ));

                string line = reader.ReadLine();
                int count = int.Parse(line);

                var starPositions = new List<SectorCoordinates>(count);
                while ((line = reader.ReadLine()) != null)
                {
                    var coords = line.Split(separator: ",".ToCharArray(), count: 3);
                    starPositions.Add(new SectorCoordinates(
                        int.Parse(coords[0]),
                        int.Parse(coords[1]),
                        int.Parse(coords[2])
                        ));
                }

                if (starPositions.Count != count)
                    throw new Exception($"Expected {count} stars but found {starPositions.Count}");

                return starPositions;
            }
            catch (Exception e)
            {
                modApi.Log($"Exception in StarDataStorage.Load: {e.Message}");
                return null;
            }
            finally
            {
                reader?.Close();
            }
        }

        public bool Store(IEnumerable<SectorCoordinates> positions)
        {
            StreamWriter writer = null;
            int count = positions.Count();

            try
            {
                writer = new StreamWriter(new FileStream(
                    FilePath, FileMode.Create, FileAccess.Write
                    ));

                writer.Write(count);
                foreach (var pos in positions)
                {
                    writer.WriteLine($"{pos.x},{pos.y},{pos.z}");
                }
                return true;
            }
            catch (Exception e)
            {
                modApi.Log($"Exception in StarDataStorage.Store: {e.Message}");
                return false;
            }
            finally
            {
                writer?.Close();
            }
        }
    }
}
