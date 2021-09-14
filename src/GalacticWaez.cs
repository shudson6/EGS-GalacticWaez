using Eleon.Modding;

namespace GalacticWaez
{
    public class Const
    {
        public const float BaseWarpRange = 30;
        public const int SectorsPerLY = 100000;
    }

    public class PlayerData
    {
        public IPlayer Entity { get; }
        public float WarpRange { get; }

        public PlayerData(IPlayer player, float warpRange)
        {
            Entity = player;
            WarpRange = warpRange;
        }
    }
}