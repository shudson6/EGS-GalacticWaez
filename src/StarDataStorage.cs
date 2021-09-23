﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez
{
    public class StarDataStorage : IStarDataStorage
    {
        public class ChecksumException : Exception
        {
            public ChecksumException(int expected, int actual)
                : base($"Expected {expected} stars but loaded {actual}") { }
        }

        public const string DefaultContentDir = "Content\\Mods\\GalacticWaez";
        private const string DefaultFileName = "stardata.csv";

        public string DirectoryPath { get; protected set; }
        public string FileName { get; protected set; }

        public string FilePath => $"{DirectoryPath}\\{FileName}";

        public StarDataStorage(string directoryPath, string fileName = DefaultFileName)
        {
            DirectoryPath = directoryPath;
            FileName = fileName;
        }

        public bool Exists()
        {
            return File.Exists(FilePath);
        }

        /// <summary>
        /// Loads star positions from storage.
        /// </summary>
        /// <returns>
        /// A collection of star positions as sector coordinates, 
        /// or <c>null</c> if an error occurred.
        /// </returns>
        /// <exception cref="FileNotFoundException">
        /// if the file is not found
        /// </exception>
        /// <exception cref="ChecksumException">
        /// if the expected number of stars is not loaded
        /// </exception>
        public IEnumerable<SectorCoordinates> Load()
        {
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
                    throw new ChecksumException(count, starPositions.Count);

                return starPositions;
            }
            finally
            {
                reader?.Close();
            }
        }

        public bool Store(IEnumerable<SectorCoordinates> positions)
        {
            Directory.CreateDirectory(DirectoryPath);
            StreamWriter writer = null;
            int count = positions.Count();

            try
            {
                writer = new StreamWriter(new FileStream(
                    FilePath, FileMode.Create, FileAccess.Write
                    ));

                writer.WriteLine(count);
                foreach (var pos in positions)
                {
                    writer.WriteLine($"{pos.x},{pos.y},{pos.z}");
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                writer?.Close();
            }
        }
    }
}
