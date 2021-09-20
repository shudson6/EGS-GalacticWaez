﻿using System;
using System.Runtime.InteropServices;
using static GalacticWaez.Const;
using SectorCoordinates = Eleon.Modding.VectorInt3;

namespace GalacticWaez
{
    public class StarFinder
    {
        // for the call to TryStartNoGCRegion. 2MB should be plenty of room for this op
        private const long NoGCSize = 2097152;

        private const int SizeOfStarData = 24;
        private const int StarCountThreshold = 1000;

        private SectorCoordinates soughtVector;
        private bool noResumeGC = false;

        unsafe public SectorCoordinates[] Search(SectorCoordinates knownPosition)
        {
            soughtVector = knownPosition;
            PauseGC();
            // need to get system info for page size and valid address range
            Kernel32.SYSTEM_INFO sysInfo;
            Kernel32.GetSystemInfo(out sysInfo);

            byte* baseAddress = sysInfo.lpMinimumApplicationAddress;
            byte* maxAddress = sysInfo.lpMaximumApplicationAddress;
            Kernel32.MEMORY_BASIC_INFORMATION memInfo;
            while (baseAddress < maxAddress)
            {
                if (Kernel32.VirtualQuery(baseAddress, out memInfo, 
                    Marshal.SizeOf<Kernel32.MEMORY_BASIC_INFORMATION>()) == 0)
                {
                    // ignore the error and move on to the next page
                    baseAddress += sysInfo.dwPageSize;
                    continue;
                }

                if (memInfo.Protect == Kernel32.DesiredPageProtection
                    && memInfo.State == Kernel32.DesiredPageState
                    && memInfo.Type == Kernel32.DesiredPageType
                ) {
                    var starDataArray = ScanRegion(memInfo.BaseAddress, memInfo.RegionSize);
                    if (starDataArray.baseAddress != null)
                    {
                        ResumeGC();
                        return ExtractStarPositions(starDataArray);
                    }
                }

                baseAddress += memInfo.RegionSize;
            }
            ResumeGC();
            return null;
        }

        private void PauseGC()
        {
            try
            {
                GC.TryStartNoGCRegion(NoGCSize);
            }
            catch (InvalidOperationException)
            {
                // TODO: provide a way to log this
                // if this happens, someone else already put us in NoGC mode
                // so we must NOT be the one to change it back
                noResumeGC = true;
            }
            catch
            {
                // StarFinder must continue whether or not GC is suspended
            }
        }

        private void ResumeGC()
        {
            try
            {
                if (!noResumeGC)
                {
                    // never guaranteed not to throw
                    GC.EndNoGCRegion();
                }
            }
            catch 
            {
                // TODO: provide a way to log this?
                // nothing to do; just move on
            }
        }

        unsafe private StarDataArray ScanRegion(byte* baseAddress, ulong size)
        {
            int* region = (int*)baseAddress;
            size /= sizeof(int);
            // we're looking for 3 vector members plus an ordinal, 4 ints total
            ulong limit = size - 4 * sizeof(int);
            for (ulong i = 0; i < limit; i++)
            {
                if (region[i] == soughtVector.x
                    && region[i + 1] == soughtVector.y
                    && region[i + 2] == soughtVector.z
                ) {
                    var starDataArray = LocateStarDataArray(region, size, i);
                    if (starDataArray.baseAddress != null)
                    {
                        return starDataArray;
                    }
                }
            }
            return default;
        }

        unsafe private StarDataArray LocateStarDataArray(int* baseAddress, ulong size, ulong vectorIndex)
        {
            // we have found our vector. might we have found the star data array?
            // vectorIndex locates StarData.x so back it up to the start of the struct
            StarData* instance = (StarData*)(baseAddress + vectorIndex - 2);
            if (instance->id < 0)
            {
                return default;
            }
            instance -= instance->id;
            if (instance < baseAddress)
            {
                return default;
            }
            // now instanceAddress points to the first element of the StarData array,
            // IF we have found it. let's find out
            int count = CountStarDataElements(instance, baseAddress + size);
            if (count > StarCountThreshold)
            {
                return new StarDataArray(instance, count);
            }
            return default;
        }

        unsafe private int CountStarDataElements(StarData* starData, void* pastEnd)
        {
            int id = 0;
            while (starData < pastEnd
                && starData->id == id
                && LooksLikeStarPosition(starData->x, starData->y, starData->z))
            {
                id++;
                starData++;
            }

            return id;
        }

        unsafe private SectorCoordinates[] ExtractStarPositions(StarDataArray starDataArray)
        {
            var starPosition = new SectorCoordinates[starDataArray.count];
            for (int i = 0; i < starPosition.Length; i++)
            {
                starPosition[i] = new SectorCoordinates(
                    starDataArray[i].x,
                    starDataArray[i].y,
                    starDataArray[i].z
                );
            }
            return starPosition;
        }

        private bool LooksLikeStarPosition(int x, int y, int z)
        {
            return x % SectorsPerLY == 0
                && y % SectorsPerLY == 0
                && z % SectorsPerLY == 0;
        }

        /*
         * This struct represents the star data as found in the game's memory.
         * intended for extracting position data via pointer casting.
         */
        [StructLayout(LayoutKind.Sequential)]
        public struct StarData
        {
            // i don't know what the first 8 bytes are for
            readonly int a;
            readonly int b;
            public readonly int x;
            public readonly int y;
            public readonly int z;
            public readonly int id;

            public StarData(int a, int b, int x, int y, int z, int id)
            {
                this.a = a;
                this.b = b;
                this.x = x;
                this.y = y;
                this.z = z;
                this.id = id;
            }
        }

        unsafe private struct StarDataArray
        {
            public readonly StarData* baseAddress;
            public readonly int count;

            public StarDataArray(StarData* baseAddress, int count)
            {
                this.baseAddress = baseAddress;
                this.count = count;
            }

            public StarData this[int i] { get => baseAddress[i]; }
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
        unsafe public struct MEMORY_BASIC_INFORMATION
        {
            public byte* BaseAddress;
            public byte* AllocationBase;
            public uint AllocationProtect;
            public ulong RegionSize;
            public uint State;
            public uint Protect;
            public uint Type;
        }

        [StructLayout(LayoutKind.Sequential)]
        unsafe public struct SYSTEM_INFO
        {
            public uint dwOemId;
            public uint dwPageSize;
            public byte* lpMinimumApplicationAddress;
            public byte* lpMaximumApplicationAddress;
            public uint dwActiveProcessorMask;
            public uint dwNumberOfProcessors;
            public uint dwProcessorType;
            public uint dwAllocationGranularity;
            public ushort wProcessorLevel;
            public ushort wProcessorRevision;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        unsafe public static extern int 
        VirtualQuery(
            byte* lpAddress,
            out MEMORY_BASIC_INFORMATION lpBuffer,
            int dwLength
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        unsafe public static extern void 
        GetSystemInfo(
            [MarshalAs(UnmanagedType.Struct)] out SYSTEM_INFO lpSystemInfo
        );
    }
}
