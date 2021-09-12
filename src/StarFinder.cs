using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace GalacticWaez
{
    class StarFinder
    {
        // https://docs.microsoft.com/en-us/windows/win32/api/winnt/ns-winnt-memory_basic_information

        // PAGE_READWRITE = 0x04
        const int DesiredPageProtection = 0x04;

        // MEM_COMMIT = 0x1000
        const int DesiredPageState = 0x1000;

        // MEM_PRIVATE = 0x20000
        const int DesiredPageType = 0x20000;

        Thread starFinder = null;

        public bool IsBusy { get => starFinder?.IsAlive ?? false; }

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
        [StructLayout(LayoutKind.Sequential)]
        struct MEMORY_BASIC_INFORMATION
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
        struct SYSTEM_INFO
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
        static extern int VirtualQuery(
            ulong lpAddress,
            out MEMORY_BASIC_INFORMATION lpBuffer,
            int dwLength
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern void GetSystemInfo(
            [MarshalAs(UnmanagedType.Struct)] out SYSTEM_INFO lpSystemInfo
        );
    }
}
