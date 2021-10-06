using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GalacticWaez
{
    public interface IPlayerProvider
    {
        IPlayerInfo GetPlayerInfo(int playerId);
    }
}
