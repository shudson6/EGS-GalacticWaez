using Eleon.Modding;

namespace GalacticWaez
{
    public interface IKnownStarProvider
    {
        bool GetFirstKnownStarPosition(out VectorInt3 pos);

        bool GetPosition(string name, out VectorInt3 pos);
    }
}
