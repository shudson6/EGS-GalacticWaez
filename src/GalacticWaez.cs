namespace GalacticWaez
{
    public delegate void LoggingDelegate(string text);

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
}