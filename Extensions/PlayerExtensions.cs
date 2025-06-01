using Mirror;
using UnityEngine;
using PlayerRoles;
using Exiled.API.Enums;
using Utils.NonAllocLINQ;
using CustomPlayerEffects;
using Exiled.API.Features;
using Exiled.API.Extensions;
using Exiled.API.Features.Toys;
using ProjectSCRAMBLE.Components;
using System.Collections.Generic;
using PlayerRoles.FirstPersonControl;
using HintServiceMeow.Core.Utilities;
using HintServiceMeow.Core.Extension;
using PlayerRoles.PlayableScps.Scp096;
using PlayerRoles.FirstPersonControl.Thirdperson;

namespace ProjectSCRAMBLE.Extensions
{
    public static class PlayerExtensions
    {
        public static Dictionary<Player, Primitive> Scp96sCencors = [];

        public static void AddSCRAMBLEHint(this Player player, string text)
        {
            player.RemoveSCRAMBLEHint();
            HintServiceMeow.Core.Models.Hints.Hint newHint = new()
            {
                Id = "SCRAMBLE",
                YCoordinate = Plugin.Instance.Config.Hint.YCordinate,
                XCoordinate = Plugin.Instance.Config.Hint.XCordinate,
                FontSize = Plugin.Instance.Config.Hint.FontSize,
                Alignment = (HintServiceMeow.Core.Enum.HintAlignment)Plugin.Instance.Config.Hint.Alligment,
                Text = text
            };

            player.AddHint(newHint);
        }

        public static void RemoveSCRAMBLEHint(this Player player)
        {
            PlayerDisplay pd = player.GetPlayerDisplay();
            if (pd.GetHint("SCRAMBLE") != null)
                pd.RemoveHint("SCRAMBLE");
        }

        public static void AddCensor(this Player player)
        {
            if (player.Role.Type != RoleTypeId.Scp096)
                return;

            Transform Head = player.GetScp96Head();
            if (Head == null)
            {
                Log.Debug("Scp096 head not found.");
                return;
            }

            if (Scp96sCencors.ContainsKey(player))
                player.RemoveCensor();

            Primitive Censormmain = Primitive.Create(Head.position, Head.rotation.eulerAngles, spawn: true );

            Censormmain.Visible = Censormmain.Collidable = false;

            for (int i = 0; i < 4; i++)
            {
                Primitive cube = Primitive.Create(
                    primitiveType: PrimitiveType.Cube,
                    position: Head.position,
                    rotation: Head.rotation.eulerAngles,
                    scale: Plugin.Instance.Config.CensorScale,
                    spawn: true,
                    color: Color.black);

                cube.Transform.parent = Censormmain.Transform;

                cube.Visible = true;
                cube.Collidable = false;
                
                cube.MovementSmoothing = Plugin.Instance.Config.CensorMovementSmooth;
                cube.GameObject.AddComponent<Rotator>().Initialize((Axis)i);
            }

            Censormmain.Transform.parent = player.Transform;

            if (Plugin.Instance.Config.AttachCensorToHead)
            {
                Censormmain.AttachToTransform(Head);
            }

            Scp96sCencors.Add(player, Censormmain);
            Censormmain.RemoveForUnGlassesPlayer(player);
        }

        public static void RemoveCensor(this Player player)
        {
            if (!Scp96sCencors.ContainsKey(player))
                return;

            Primitive Censor = Scp96sCencors[player];
            NetworkServer.Destroy(Censor.GameObject);

            Scp96sCencors.Remove(player);

            foreach (List<Player> ply in ProjectSCRAMBLE.ActiveScramblePlayers.Values)
            {
                if (!ply.Contains(player))
                    continue;

                ply.Remove(player);
            }
        }

        public static void ObfuscateScp96s(this Player player)
        {
            foreach (Player ply in Scp96sCencors.Keys)
            {
                player.SpawnCensor(Scp96sCencors[ply]);
                ProjectSCRAMBLE.ActiveScramblePlayers[player].AddIfNotContains(ply);
            }
        }

        public static void DeObfuscateScp96s(this Player player)
        {
            if (!ProjectSCRAMBLE.ActiveScramblePlayers.ContainsKey(player))
            {
                Log.Debug("This Playerin not wearing Project SCRAMBLE");
                return;
            }

            foreach (Player ply in ProjectSCRAMBLE.ActiveScramblePlayers[player])
            {
                if (!Scp96sCencors.ContainsKey(ply))
                    continue;

                Log.Debug($"{ply.Nickname} Obfuscate destroying for {player.Nickname} ");
                player.DestroyCensor(Scp96sCencors[ply]);
            }

            ProjectSCRAMBLE.ActiveScramblePlayers.Remove(player);
        }

        public static Transform GetScp96Head(this Player player)
        {
            if (player.Role.Base is not IFpcRole fpc)
            {
                Log.Debug("This 96 role is not have first person control.");
                return null;
            }

            CharacterModel model = fpc.FpcModule.CharacterModelInstance;
            if (model is not Scp096CharacterModel anima)
            {
                Log.Debug("This 96 role doesnt have Scp096CharacterModel.");
                return null;
            }

            return anima.Head;
        }

        public static void SpawnCensor(this Player player, Primitive Censormain)
        {
            foreach (NetworkIdentity networkIdentity in Censormain.GameObject.GetComponentsInChildren<NetworkIdentity>())
            {
                Server.SendSpawnMessage.Invoke(null, [networkIdentity, player.Connection]);
            }
        }

        public static void DestroyCensor(this Player player, Primitive Censormain)
        {
            foreach (NetworkIdentity networkIdentity in Censormain.GameObject.GetComponentsInChildren<NetworkIdentity>())
            {
                player.Connection.Send(new ObjectDestroyMessage
                {
                    netId = networkIdentity.netId
                });
            }
        }

        public static void SendFakeEffect(this Player effectOwner, EffectType effect, byte intensity)
        {
            MirrorExtensions.SendFakeSyncObject(effectOwner, effectOwner.NetworkIdentity, typeof(PlayerEffectsController), (writer) =>
            {
                const ulong InitSyncObjectDirtyBit = 0b0001;
                const uint ChangesCount = 1;
                const byte OperationId = (byte)SyncList<byte>.Operation.OP_SET;

                StatusEffectBase foundEffect = effectOwner.GetEffect(effect);
                uint index = (uint)effectOwner.GetEffectIndex(foundEffect);
                if (index < 0)
                    return;

                byte newIntensity = intensity;

                writer.WriteULong(InitSyncObjectDirtyBit);
                writer.WriteUInt(ChangesCount);
                writer.WriteByte(OperationId);
                writer.WriteUInt(index);
                writer.WriteByte(newIntensity);
            });
        }

        private static int GetEffectIndex(this Player player, StatusEffectBase effect)
        {
            PlayerEffectsController controller = player.ReferenceHub.playerEffectsController;
            for (int i = 0; i < controller.EffectsLength; i++)
            {
                if (ReferenceEquals(controller.AllEffects[i], effect))
                    return i;
            }

            return -1;
        }
    }
}
