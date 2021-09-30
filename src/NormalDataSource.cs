﻿using Eleon.Modding;
using System.Collections.Generic;
using System.Linq;

namespace GalacticWaez
{
    /// <summary>
    /// Provides star positions by first checking for a save file and, failing
    /// that, scanning memory for them.
    /// </summary>
    class NormalDataSource : IGalaxyDataSource
    {
        private readonly IFileDataSource fileSource;
        private readonly IGalaxyDataSource scanSource;

        public NormalDataSource(IFileDataSource fileSource, IGalaxyDataSource scanSource)
        {
            this.fileSource = fileSource;
            this.scanSource = scanSource;
        }

        public IEnumerable<VectorInt3> GetGalaxyData()
        {
            var positions = fileSource.GetGalaxyData();
            if (positions == null || !positions.Any())
            {
                positions = scanSource.GetGalaxyData();
            }
            return positions;
        }
    }
}
