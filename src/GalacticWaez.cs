using Eleon.Modding;
using static GalacticWaez.Const; 

using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez
{
    public class Const
    {
        public const float BaseWarpRange = 30;
        public const int SectorsPerLY = 100000;
        // used as error response when return value is VectorInt3, since the default value
        // 0, 0, 0 is perfectly valid.
        // this value would never appear as star coordinates
        public static readonly VectorInt3 ErrorVector = new VectorInt3(0x52004500, 0x4f005200, 0x21005200);
    }

    public struct LYCoordinates
    {
        public readonly int x;
        public readonly int y;
        public readonly int z;

        public LYCoordinates(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public LYCoordinates(SectorCoordinates sc)
            : this(sc.x / SectorsPerLY, sc.y / SectorsPerLY, sc.z / SectorsPerLY)
        { }

        public SectorCoordinates ToSectorCoordinates() =>
            new SectorCoordinates(x * SectorsPerLY, y * SectorsPerLY, z * SectorsPerLY);

        public override string ToString()
        {
            return $"({x}, {y}, {z})";
        }
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