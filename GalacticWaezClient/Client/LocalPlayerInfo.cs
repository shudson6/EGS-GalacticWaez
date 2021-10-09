using Mono.Data.Sqlite;
using System;
using System.Data;
using static GalacticWaez.GalacticWaez;
using Eleon.Modding;

namespace GalacticWaez.Client
{
    public sealed class LocalPlayerInfo : PlayerProviderBase, IPlayerInfo
    {
        private readonly IModApi modApi;

        public int Id { get => modApi.Application.LocalPlayer.Id; }
        public int FactionId { get => modApi.Application.LocalPlayer.Faction.Id; }
        public float WarpRange => GetWarpRange(Id);

        public LocalPlayerInfo(IModApi modApi)
            : base(modApi.Application.GetPathFor(AppFolder.SaveGame), modApi.Log)
            => this.modApi = modApi;

        public override IPlayerInfo GetPlayerInfo(int _) => this;

        public VectorInt3 StarCoordinates => modApi.ClientPlayfield.SolarSystemCoordinates;

        public string PlayfieldName => modApi.ClientPlayfield.Name;

        public string Name => modApi.Application.LocalPlayer.Name;
    }
}