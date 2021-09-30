using System;
using GalacticWaez.Command;

namespace GalacticWaez
{
    public delegate void InitializerCallback(ICommandHandler commandHandler, AggregateException ex);

    /// <summary>
    /// Enum that tells Initializer where it should look for star map data.
    /// </summary>
    public enum InitializerType
    {
        /// <summary>Look for stored data first; scan memory if not found.</summary>
        Normal,
        /// <summary>Look for file only; don't fall back to memory scan.</summary>
        FileOnly,
        /// <summary>Scan memory only.</summary>
        ScanOnly
    }


    public interface IInitializer
    {
        void Initialize(InitializerCallback doneCallback);
    }
}