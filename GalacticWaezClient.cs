using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon.Modding;

namespace GalacticWaez
{
    public class GalacticWaezClient : IMod
    {
        IModApi modApi;

        public void Init(IModApi modApi)
        {
            this.modApi = modApi;
            modApi.Log("GalacticWaez attached.");
        }

        public void Shutdown()
        {
            modApi.Log("GalacticWaez detached.");
        }
    }
}
