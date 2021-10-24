using Eleon.Modding;

namespace GalacticWaez.Dedi
{
    public class GalacticWaezDedi : GalacticWaez
    {
        public override void Init(IModApi modApi)
        {
            base.Init(modApi);
            PlayerProvider = CreatePlayerProvider();
            ChatHandler = CreateChatHandler(PlayerProvider);
            Setup(DataSourceType.Normal);
        }

        public override void Shutdown()
        {
        }

        protected override IPlayerProvider CreatePlayerProvider()
            => new DediPlayerProvider(
                ModApi.Application.GetPathFor(AppFolder.SaveGame), 
                Config.BaseWarpRange,
                ModApi.Application.GetPlayerDataFor,
                ModApi.Log);
    }
}
