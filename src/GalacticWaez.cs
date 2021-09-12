using static GalacticWaez.Const;

namespace GalacticWaez
{
    class CommandToken
    {
        public const string Introducer = "/waez";
        public const string Init = "init";
    }

    class Const
    {
        public const int SectorsPerLY = 100000;
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