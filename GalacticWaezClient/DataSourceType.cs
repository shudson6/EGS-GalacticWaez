using System;

namespace GalacticWaez
{
    /// <summary>
    /// Enum of sources for star map data.
    /// </summary>
    public enum DataSourceType
    {
        /// <summary>Look for stored data first; scan memory if not found.</summary>
        Normal,
        /// <summary>Look for file only; don't fall back to memory scan.</summary>
        FileOnly,
        /// <summary>Scan memory only.</summary>
        ScanOnly
    }
}