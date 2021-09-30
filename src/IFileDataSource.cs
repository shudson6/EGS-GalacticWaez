namespace GalacticWaez
{
    public interface IFileDataSource : IGalaxyDataSource
    {
        string PathToFile { get; }
    }
}
