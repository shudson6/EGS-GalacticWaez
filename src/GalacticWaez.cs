using static GalacticWaez.Const; 

using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez
{
    public class Const
    {
        /// <summary> Base value of player warp range, to which bonuses are added </summary>
        public const float BaseWarpRange = BaseWarpRangeLY * SectorsPerLY;
        /// <summary> Default maximum distance between stars to be considered neighbors </summary>
        public const float DefaultMaxWarpRange = DefaultMaxWarpRangeLY * SectorsPerLY;
        /// <summary> Base value of player warp range, in LY, to which bonuses are added </summary>
        public const float BaseWarpRangeLY = 30;
        /// <summary> Default maximum distance between stars to be considered neighbors, in LY</summary>
        public const float DefaultMaxWarpRangeLY = 110;
        /// <summary> Constant for converting between sectors and light-years </summary>
        public const int SectorsPerLY = 100000;
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
}