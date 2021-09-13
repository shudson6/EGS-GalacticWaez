using Eleon.Modding;

namespace GalacticWaez
{
    class Const
    {
        public const float BaseWarpRange = 30;
        public const int SectorsPerLY = 100000;
    }

    class PlayerData
    {
        public IPlayer Entity { get; }
        public float WarpRange { get; }

        public PlayerData(IPlayer player, float warpRange)
        {
            Entity = player;
            WarpRange = warpRange;
        }
    }

    struct StarPosition
    {
        public readonly int sectorX;
        public readonly int sectorY;
        public readonly int sectorZ;

        public StarPosition(int sectorX, int sectorY, int sectorZ)
        {
            this.sectorX = sectorX;
            this.sectorY = sectorY;
            this.sectorZ = sectorZ;
        }
    }
}