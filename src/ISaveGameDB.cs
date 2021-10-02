using Eleon.Modding;
using System.Collections.Generic;

namespace GalacticWaez
{
    public interface ISaveGameDB
    {
        int ClearPathMarkers(int playerId);
        bool GetSolarSystemCoordinates(string starName, out VectorInt3 coordinates);
    }
}