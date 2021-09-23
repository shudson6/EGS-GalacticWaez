using Eleon.Modding;
using System.Collections.Generic;

namespace GalacticWaez
{
    public interface IStarDataStorage
    {
        bool Exists();
        IEnumerable<VectorInt3> Load();
        bool Store(IEnumerable<VectorInt3> positions);
    }
}