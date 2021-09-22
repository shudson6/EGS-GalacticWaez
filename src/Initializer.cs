﻿using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon.Modding;
using GalacticWaez.Navigation;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez
{
    /// <summary>
    /// Used by CommandHandler to build Galaxy
    /// </summary>
    class Initializer
    {
        /// <summary>
        /// Enum that tells Initializer where it should look for star map data.
        /// </summary>
        public enum Source
        {
            /// <summary>
            /// Look for stored data first; scan memory if not found.
            /// </summary>
            Normal,
            /// <summary>
            /// Look for file only; don't fall back to memory scan.
            /// </summary>
            File,
            /// <summary>
            /// Scan memory only.
            /// </summary>
            Scanner
        }

        public delegate void DoneCallback(Galaxy galaxy, string message);

        private readonly IModApi modApi;
        private DoneCallback doneCallback;
        private Galaxy galaxy;
        private Task<string> init;

        public Initializer(IModApi modApi)
        {
            this.modApi = modApi;
        }

        public void Initialize(Source source, DoneCallback doneCallback)
        {
            this.doneCallback = doneCallback;
            init = Task<string>.Factory.StartNew(function: BuildGalaxyMap, state: source);
            modApi.Application.Update += OnUpdateDuringInit;
        }

        private string BuildGalaxyMap(object obj)
        {
            IEnumerable<SectorCoordinates> stars = null;
            var source = (Source)obj;
            switch (source)
            {
                case Source.Normal:
                    stars = LoadStarData() ?? ScanForStarData(true);
                    break;

                case Source.File:
                    stars = LoadStarData();
                    break;

                case Source.Scanner:
                    stars = ScanForStarData();
                    break;
            }
            var message = new StringBuilder();
            galaxy = CreateGalaxy(stars, Const.BaseWarpRange, message);
            return message.ToString();
        }

        private Galaxy CreateGalaxy(IEnumerable<SectorCoordinates> locations, float range, StringBuilder msg)
        {
            var stopwatch = Stopwatch.StartNew();
            var g = Galaxy.CreateNew(locations, range);
            stopwatch.Stop();
            float time = (float)stopwatch.ElapsedMilliseconds / 1000;
            msg.AppendLine("Constructed galactic highway map: "
                + $"{g.StarCount} stars, {g.WarpLines} warp lines. "
                + $"Took {time}s.");
            return g;
        }

        private IEnumerable<SectorCoordinates> LoadStarData()
        {
            var stored = new StarDataStorage(modApi);
            if (!stored.Exists())
            {
                modApi.Log("Stored star data not found.");
                return null;
            }
            var loaded = stored.Load();
            if (loaded == null)
            {
                modApi.Log("Failed to load star data from file: " + stored.FilePath);
            }
            return loaded;
        }

        /// <summary>
        /// Uses StarFinder to scan memory for star position data.
        /// If save == true, saves the found data.
        /// </summary>
        /// <param name="save">
        /// Set to true to save the found star data.
        /// Defaults to false.
        /// </param>
        /// <returns>
        /// A collection containing the positions of the stars.
        /// </returns>
        private IEnumerable<SectorCoordinates> ScanForStarData(bool save = false)
        {
            var known = new SaveGameDB(modApi).GetFirstKnownStarPosition();
            var stopwatch = Stopwatch.StartNew();
            var stars = new StarFinder().Search(known);
            stopwatch.Stop();
            if (stars == null)
            {
                modApi.LogWarning("Failed to locate star position data. "
                    + $"Scan took {stopwatch.ElapsedMilliseconds}ms.");
                return null;
            }
            modApi.Log($"Located {stars.Length} stars in {stopwatch.ElapsedMilliseconds}ms.");
            if (save)
            {
                var storage = new StarDataStorage(modApi);
                if (storage.Store(stars))
                {
                    modApi.Log("Saved star positions to " + storage.FilePath);
                }
            }
            return stars;
        }

        private void OnUpdateDuringInit()
        {
            if (!init.IsCompleted) return;

            modApi.Application.Update -= OnUpdateDuringInit;
            doneCallback(galaxy, init.Result);
        }
    }
}
