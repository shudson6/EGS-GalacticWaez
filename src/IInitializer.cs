using System;
using GalacticWaez.Navigation;

namespace GalacticWaez
{
    public delegate void InitializerCallback(Galaxy galaxy, AggregateException ex);

    public interface IInitializer
    {
        void Initialize(Initializer.Source source, InitializerCallback doneCallback);
    }
}