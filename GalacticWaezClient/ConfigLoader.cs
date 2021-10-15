using EcfParser;
using System;
using System.Collections.Generic;
using System.IO;
using static GalacticWaez.GalacticWaez;

namespace GalacticWaez
{
    public class ConfigLoader
    {
        private class Config : IConfiguration
        {
            public int BaseWarpRange => baseWarpRangeLY * SectorsPerLY;
            public int MaxWarpRange => maxWarpRangeLY * SectorsPerLY;
            public int NavTimeoutMillis => navTimeoutMillis;

            public int baseWarpRangeLY;
            public int maxWarpRangeLY;
            public int navTimeoutMillis;

            public static Config Default = new Config
            {
                baseWarpRangeLY = (int)DefaultBaseWarpRangeLY,
                maxWarpRangeLY = (int)DefaultMaxWarpRangeLY,
                navTimeoutMillis = DefaultNavTimeoutMillis
            };
        }

        private readonly LoggingDelegate Log;

        public ConfigLoader(LoggingDelegate log)
        {
            Log = log ?? delegate { };
        }

        public IConfiguration LoadConfig(string filename)
        {
            try
            {
                var lines = new List<string>();
                using (var reader = new StreamReader(filename))
                {
                    string l;
                    while ((l = reader.ReadLine()) != null)
                        lines.Add(l);
                    var ecf = Parse.Deserialize(lines.ToArray()).Blocks[0].EcfValues;
                    return new Config
                    {
                        baseWarpRangeLY = (int)ecf.GetValueOrDefault("BaseWarpRangeLY",
                            new EcfAttribute { Value = (int)DefaultBaseWarpRangeLY }).Value,
                        maxWarpRangeLY = (int)ecf.GetValueOrDefault("MaxWarpRangeLY",
                            new EcfAttribute { Value = (int)DefaultMaxWarpRangeLY }).Value,
                        navTimeoutMillis = 1000 * (int)ecf.GetValueOrDefault("NavTimeoutSeconds",
                            new EcfAttribute { Value = DefaultNavTimeoutSeconds }).Value
                    };
                }
            }
            catch (Exception ex)
            {
                Log("LoadConfig: " + ex.Message);
                Log("Using default configuration.");
                return Config.Default;
            }
        }
    }
}
