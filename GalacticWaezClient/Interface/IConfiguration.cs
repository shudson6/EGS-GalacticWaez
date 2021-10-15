namespace GalacticWaez
{
    public interface IConfiguration
    {
        int BaseWarpRange { get; }
        int MaxWarpRange { get; }
        int NavTimeoutMillis { get; }
    }
}