using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Eleon;
using Eleon.Modding;
using GalacticWaez;

namespace GalacticWaez.Dedi
{
    public class GalacticWaezDedi : GalacticWaez
    {
        public override void Init(IModApi modApi)
        {
            base.Init(modApi);
            ChatHandler = CreateChatHandler(CreatePlayerProvider());
            Setup(DataSourceType.Normal);
        }

        public override void Shutdown()
        {
        }

        protected override IPlayerProvider CreatePlayerProvider()
            => new DediPlayerProvider(ModApi.Application.GetPathFor(AppFolder.SaveGame), ModApi.Log);
    }
}
