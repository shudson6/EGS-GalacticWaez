using System;
using System.Runtime.InteropServices;
using Eleon.Modding;
using static GalacticWaez.Const;

namespace GalacticWaez
{
    class StarFinder
    {
        const int SizeOfStarData = 24;
        const int StarCountThreshold = 1000;

        readonly VectorInt3 soughtVector;
        public int StarsFound { get => hits; }

        int hits;

        public StarFinder(VectorInt3 knownPosition)
        {
            soughtVector = knownPosition;
            hits = 0;
        }

        public void Search()
        {
            // need to get system info for page size and valid address range
            Kernel32.SYSTEM_INFO sysInfo;
            Kernel32.GetSystemInfo(out sysInfo);

            ulong baseAddress = (ulong)sysInfo.lpMinimumApplicationAddress.ToInt64();
            ulong maxAddress = (ulong)sysInfo.lpMaximumApplicationAddress.ToInt64();
            Kernel32.MEMORY_BASIC_INFORMATION memInfo = default;
            while (baseAddress < maxAddress)
            {
                if (Kernel32.VirtualQuery(baseAddress, out memInfo, Marshal.SizeOf(memInfo)) == 0)
                {
                    // ignore the error and move on to the next page
                    baseAddress += sysInfo.dwPageSize;
                    continue;
                }

                if (memInfo.Protect == Kernel32.DesiredPageProtection
                    && memInfo.State == Kernel32.DesiredPageState
                    && memInfo.Type == Kernel32.DesiredPageType
                ) {
                    ScanRegion(memInfo.BaseAddress.ToUInt64(), (ulong)memInfo.RegionSize.ToInt64());
                }

                baseAddress += (ulong)memInfo.RegionSize.ToInt64();
            }
        }

        void ScanRegion(ulong baseAddress, ulong size)
        {
            // we're looking for ints and assuming the values are qword-aligned
            const int Step = 4;

            // we're looking for 3 vector members plus an ordinal, 4 ints total
            const int StarDataPadding = 16;
            ulong limit = size - StarDataPadding;
            for (ulong i = 0; i < limit; i += Step)
            {
                if (IntValueAt(baseAddress + i) == soughtVector.x
                    && IntValueAt(baseAddress + i + 4) == soughtVector.y
                    && IntValueAt(baseAddress + i + 8) == soughtVector.z
                ) {
                    int foo = CountPossibleInstances(baseAddress, size, i);
                    if (foo > hits) hits = foo;
                }
            }
        }

        int CountPossibleInstances(ulong baseAddress, ulong size, ulong vectorIndex)
        {
            // in {x, y, z, id} vectorLocation points to x. we want id
            ulong idAddress = baseAddress + vectorIndex + 12;
            int id = IntValueAt(idAddress);
            // make sure the counting starts at 0
            if (id < 0)
            {
                return 0;
            }
            if (id > 0)
            {
                idAddress -= (uint)id * SizeOfStarData;
                if (idAddress < baseAddress)
                {
                    return 0;
                }
            }
            id = 0;
            while (id == IntValueAt(idAddress)
                && LooksLikeStarPosition(IntValueAt(idAddress - 12),
                    IntValueAt(idAddress - 8), IntValueAt(idAddress - 4))
            ) {
                id++;
                idAddress += SizeOfStarData;
            }

            return id > StarCountThreshold ? id : 0;
        }

        unsafe int IntValueAt(ulong address)
        {
            return *(int*)address;
        }

        bool LooksLikeStarPosition(int x, int y, int z)
        {
            return x % SectorsPerLY == 0
                && y % SectorsPerLY == 0
                && z % SectorsPerLY == 0;
        }

        /*
         * This struct represents the star data as found in the game's memory.
         * All fields are readonly and no constructor is provided; this struct
         * is intended for extracting position data via pointer casting.
         */
        [StructLayout(LayoutKind.Sequential)]
        struct StarData
        {
            // i don't know what the first 8 bytes are for
            readonly long reserved;
            public readonly int sectorX;
            public readonly int sectorY;
            public readonly int sectorZ;
            public readonly int id;
        }
    }

    static class Kernel32
    {
        /* 
         * More info about these definitions is at:
         * https://docs.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-memory_basic_information
         */

        // PAGE_READWRITE = 0x04
        public const int DesiredPageProtection = 0x04;

        // MEM_COMMIT = 0x1000
        public const int DesiredPageState = 0x1000;

        // MEM_PRIVATE = 0x20000
        public const int DesiredPageType = 0x20000;

        [StructLayout(LayoutKind.Sequential)]
        public struct MEMORY_BASIC_INFORMATION
        {
            public UIntPtr BaseAddress;
            public UIntPtr AllocationBase;
            public uint AllocationProtect;
            public IntPtr RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SYSTEM_INFO
        {
            public uint dwOemId;
            public uint dwPageSize;
            public IntPtr lpMinimumApplicationAddress;
            public IntPtr lpMaximumApplicationAddress;
            public uint dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern int 
        VirtualQuery(
            ulong lpAddress,
            out MEMORY_BASIC_INFORMATION lpBuffer,
            int dwLength
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern void 
        GetSystemInfo(
            [MarshalAs(UnmanagedType.Struct)] out SYSTEM_INFO lpSystemInfo
        );
    }
}
