using Eleon.Modding;

namespace GalacticWaez
{
    public interface IStarFinder
    {
        VectorInt3[] Search(VectorInt3 knownPosition);
    }
}