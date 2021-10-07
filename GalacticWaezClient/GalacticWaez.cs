using Eleon.Modding;

namespace GalacticWaez
{
    public delegate void LoggingDelegate(string text);

    public static class GalacticWaez
    {
        /// <summary> Base value of player warp range, in sectors </summary>
        public const float BaseWarpRange = BaseWarpRangeLY * SectorsPerLY;
        /// <summary> Default maximum distance between stars to be considered neighbors, in sectors </summary>
        public const float DefaultMaxWarpRange = DefaultMaxWarpRangeLY * SectorsPerLY;
        /// <summary> Base value of player warp range, in LY, to which bonuses are added </summary>
        public const float BaseWarpRangeLY = 30;
        /// <summary> Default maximum distance between stars to be considered neighbors, in LY</summary>
        public const float DefaultMaxWarpRangeLY = 110;
        /// <summary> Constant for converting between sectors and light-years </summary>
        public const int SectorsPerLY = 100000;

        public static VectorInt3 Divide(this VectorInt3 vector, int divisor)
            => new VectorInt3(
                vector.x / divisor,
                vector.y / divisor,
                vector.z / divisor);

        public static VectorInt3 Multiply(this VectorInt3 vector, int multiplier)
            => new VectorInt3(
                vector.x * multiplier,
                vector.y * multiplier,
                vector.z * multiplier);
    }
}