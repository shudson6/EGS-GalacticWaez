using System;
using System.Collections.Generic;
using Eleon.Modding;

namespace GalacticWaezTests
{
    public class FakePlayer : IPlayer
    {
        public FakePlayer(int id)
        {
            Id = id;
        }

        public string SteamId => throw new NotImplementedException();

        public string StartPlayfield => throw new NotImplementedException();

        public byte Origin => throw new NotImplementedException();

        public FactionData FactionData => throw new NotImplementedException();

        public FactionRole FactionRole => throw new NotImplementedException();

        public float Health => throw new NotImplementedException();

        public float HealthMax => throw new NotImplementedException();

        public float Oxygen => throw new NotImplementedException();

        public float OxygenMax => throw new NotImplementedException();

        public float Stamina => throw new NotImplementedException();

        public float StaminaMax => throw new NotImplementedException();

        public float Food => throw new NotImplementedException();

        public float FoodMax => throw new NotImplementedException();

        public float Radiation => throw new NotImplementedException();

        public float RadiationMax => throw new NotImplementedException();

        public float BodyTemp => throw new NotImplementedException();

        public float BodyTempMax => throw new NotImplementedException();

        public int Kills => throw new NotImplementedException();

        public int Died => throw new NotImplementedException();

        public double Credits => throw new NotImplementedException();

        public int ExperiencePoints => throw new NotImplementedException();

        public int UpgradePoints => throw new NotImplementedException();

        public int Ping => throw new NotImplementedException();

        public IStructure CurrentStructure => throw new NotImplementedException();

        public IEntity DrivingEntity => throw new NotImplementedException();

        public bool IsPilot => throw new NotImplementedException();

        public int HomeBaseId => throw new NotImplementedException();

        public string SteamOwnerId => throw new NotImplementedException();

        public int Permission => throw new NotImplementedException();

        public List<ItemStack> Toolbar => throw new NotImplementedException();

        public List<ItemStack> Bag => throw new NotImplementedException();

        public int Id { get; private set; }

        public string Name => throw new NotImplementedException();

        public FactionData Faction => default;

        public global::UnityEngine.Vector3 Position { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public global::UnityEngine.Vector3 Forward => throw new NotImplementedException();

        public global::UnityEngine.Quaternion Rotation { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool IsLocal => throw new NotImplementedException();

        public bool IsProxy => throw new NotImplementedException();

        public bool IsPoi => throw new NotImplementedException();

        public int BelongsTo => throw new NotImplementedException();

        public int DockedTo => throw new NotImplementedException();

        public EntityType Type => throw new NotImplementedException();

        public IStructure Structure => throw new NotImplementedException();

        public void DamageEntity(int damageAmount, int damageType)
        {
            throw new NotImplementedException();
        }

        public bool LoadFromDSL()
        {
            throw new NotImplementedException();
        }

        public void Move(global::UnityEngine.Vector3 direction)
        {
            throw new NotImplementedException();
        }

        public void MoveForward(float speed)
        {
            throw new NotImplementedException();
        }

        public void MoveStop()
        {
            throw new NotImplementedException();
        }

        public bool Teleport(string playfield, global::UnityEngine.Vector3 pos, global::UnityEngine.Vector3 rot)
        {
            throw new NotImplementedException();
        }

        public bool Teleport(global::UnityEngine.Vector3 pos)
        {
            throw new NotImplementedException();
        }
    }
}
