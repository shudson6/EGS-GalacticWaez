namespace GalacticWaez
{
    public interface IFileDataSource : IGalaxyDataSource
    {
        /// <summary> Gets the path to the file used by this instance. </summary>
        string PathToFile { get; }
    }
}
