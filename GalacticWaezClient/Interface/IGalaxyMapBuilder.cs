namespace GalacticWaez
{
    public interface IGalaxyMapBuilder
    {
        IGalaxyMap BuildGalaxyMap(IGalaxyDataSource source, float maxWarpRange,
            System.Threading.CancellationToken token);
    }
}