using Eleon.Modding;

namespace GalacticWaez
{
    public interface IKnownStarProvider
    {
        bool GetFirstKnownStarPosition(out VectorInt3 pos);
    }
}
