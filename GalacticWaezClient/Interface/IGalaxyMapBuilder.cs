﻿namespace GalacticWaez
{
    public interface IGalaxyMapBuilder
    {
        GalaxyMap BuildGalaxyMap(IGalaxyDataSource source, float maxWarpRange,
            System.Threading.CancellationToken token);
    }
}