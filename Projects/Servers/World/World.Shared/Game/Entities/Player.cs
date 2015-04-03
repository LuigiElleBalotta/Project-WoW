﻿/*
 * Copyright (C) 2012-2015 Arctium Emulation <http://arctium.org>
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Linq;
using System.Numerics;
using Framework.Database.Character.Entities;
using Framework.Datastore;
using World.Shared.Game.Entities.Object;
using World.Shared.Game.Entities.Object.Descriptors;
using World.Shared.Game.Entities.Object.Guid;
using World.Shared.Game.Objects.Entities;

namespace World.Shared.Game.Entities
{
    public sealed class Player : WorldUnitBase, IWorldObject
    {
        public PlayerData PlayerData;

        Character data;

        public Player(Character player) : base(PlayerData.End)
        {
            data = player;

            Guid = new PlayerGuid
            {
                CreationBits = player.Guid,
                RealmId = (ushort)player.RealmId
            };

            PlayerData = new PlayerData();

            Position = new Vector3(player.X, player.Y, player.Z);
            Facing   = player.O;
            Map      = (short)player.Map;

            InitializeDescriptors();
        }

        public void InitializeDescriptors()
        {
            Set(ObjectData.Guid, Guid.Low);
            Set(ObjectData.Guid + 2, Guid.High);
            Set(ObjectData.Type, 0x19);
            Set(ObjectData.Scale, 1f);

            Set(UnitData.Health, 1337);
            Set(UnitData.MaxHealth, 1337);

            var gtLevelExperience = ClientDB.GtOCTLevelExperience.First(gt => gt.Data > data.Experience);

            // Current experience level.
            Set(UnitData.Level, gtLevelExperience.Index + 1);

            // Current experience points & needed experience points for next level.
            Set(PlayerData.XP, data.Experience);
            Set(PlayerData.NextLevelXP, (int)gtLevelExperience.Data);

            var race = ClientDB.ChrRaces.Single(r => r.Id == data.Race);
            var chrClass = ClientDB.ChrClasses.Single(r => r.Id == data.Class);

            Set(UnitData.DisplayPower, chrClass.DisplayPower);
            Set(UnitData.FactionTemplate, race.FactionId);

            Set(UnitData.Sex, (byte)data.Race, 0);
            Set(UnitData.Sex, (byte)data.Class, 1);
            Set(UnitData.Sex, (byte)0, 2);
            Set(UnitData.Sex, data.Sex, 3);

            var displayId = data.Sex == 1 ? race.FemaleDisplayId : race.MaleDisplayId;

            Set(UnitData.DisplayID, displayId);
            Set(UnitData.NativeDisplayID, displayId);

            Set(UnitData.Flags, 0x8u);

            Set(UnitData.BoundingRadius, 0.389f);
            Set(UnitData.CombatReach, 1.5f);
            Set(UnitData.ModCastingSpeed, 1f);
            Set(UnitData.MaxHealthModifier, 1f);

            Set(PlayerData.HairColorID, data.Skin, 0);
            Set(PlayerData.HairColorID, data.Face, 1);
            Set(PlayerData.HairColorID, data.HairStyle, 2);
            Set(PlayerData.HairColorID, data.HairColor, 3);

            Set(PlayerData.RestState, data.FacialHairStyle, 0);
            Set(PlayerData.RestState, 0, 1);
            Set(PlayerData.RestState, 0, 2);
            Set(PlayerData.RestState, 2, 3);

            Set(PlayerData.ArenaFaction, data.Sex, 0);
            Set(PlayerData.ArenaFaction, 0, 1);
            Set(PlayerData.ArenaFaction, 0, 2);
            Set(PlayerData.ArenaFaction, 0, 3);

            Set(PlayerData.WatchedFactionIndex, -1);
            Set(PlayerData.VirtualPlayerRealm, data.RealmId);
        }

        public void InitializeDynamicDescriptors()
        {
            throw new NotImplementedException();
        }
    }
}
