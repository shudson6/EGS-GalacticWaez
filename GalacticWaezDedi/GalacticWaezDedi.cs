using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon.Modding;
using GalacticWaez;

namespace GalacticWaez.Dedi
{
    public class GalacticWaezDedi : IMod
    {
        private IModApi modApi;

        public ModState Status { get; private set; }

        public void Init(IModApi modAPI)
        {
            modApi = modAPI;
            Status = ModState.Uninitialized;
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }
    }
}
