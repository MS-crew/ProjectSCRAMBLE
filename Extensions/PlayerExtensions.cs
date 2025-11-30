using System.Collections.Generic;

using AdminToys;

using Exiled.API.Features;
using Exiled.API.Features.Roles;
using Exiled.API.Features.Toys;

using MEC;

using Mirror;

using PlayerRoles;
using PlayerRoles.PlayableScps.Scp096;

using UnityEngine;

using Utils.NonAllocLINQ;
using System.Runtime.CompilerServices;


#if RUEI
using RueI.API;
using RueI.API.Elements;
#endif

#if HSM
using HintServiceMeow.Core.Extension;
using HintServiceMeow.Core.Utilities;
#endif

#if PMER
using ProjectMER.Features;
using ProjectMER.Features.Objects;
#endif

namespace ProjectSCRAMBLE.Extensions
{
    public static class PlayerExtensions
    {
#if RUEI
        private static readonly Tag scrambleHintTag = new("ProjectScramble");
#elif HSM
        private static readonly string hsmID = "SCRAMBLE";
#endif

        public static Dictionary<Player, GameObject> Scp96Censors = [];

        public static void AddSCRAMBLEHint(this Player player, string text)
        {
#if RUEI
            RueDisplay.Get(player).Show(scrambleHintTag, new BasicElement(Plugin.Instance.Config.Hint.YCordinate, text));
#elif HSM
            player.RemoveSCRAMBLEHint();
            HintServiceMeow.Core.Models.Hints.Hint newHint = new()
            {
                Id = hsmID,
                YCoordinate = Plugin.Instance.Config.Hint.YCordinate,
                XCoordinate = Plugin.Instance.Config.Hint.XCordinate,
                FontSize = Plugin.Instance.Config.Hint.FontSize,
                Alignment = (HintServiceMeow.Core.Enum.HintAlignment)Plugin.Instance.Config.Hint.Alligment,
                Text = text
            };

            player.AddHint(newHint);
#endif
        }

        public static void RemoveSCRAMBLEHint(this Player player)
        {
#if RUEI
            RueDisplay.Get(player).Remove(scrambleHintTag);
#elif HSM
            PlayerDisplay pd = player.GetPlayerDisplay();
            if (pd.GetHint(hsmID) != null)
                pd.RemoveHint(hsmID);
#endif
        }

        public static void AddCensor(this Player player)
        {
            if (player.Role.Type != RoleTypeId.Scp096)
                return;
            
            if (!player.TryGetScp96Head(out Transform head))
            {
                Log.Error("Scp096 head not found.");
                return;
            }

            if (Scp96Censors.ContainsKey(player))
                player.RemoveCensor();

            Config config = Plugin.Instance.Config;
#if PMER
            if (!ObjectSpawner.TrySpawnSchematic(config.CensorSchematic, head.position, head.rotation, 
                config.CensorScale , out SchematicObject Censor))
            {
                Log.Error("Censor Schematic failed to spawn");
                return;
            }

            Censor.transform.SetParent(player.Transform, false);

            if (config.AttachCensorToHead)
                Censor.transform.AttachToTransform(head);

            Scp96Censors.Add(player, Censor.gameObject);
            Censor.gameObject.HideForUnGlassesPlayer(player);
#else
            Primitive Censor = Primitive.Create(primitiveType: PrimitiveType.Cube, flags: PrimitiveFlags.Visible, position: head.position,
                rotation: head.rotation.eulerAngles, scale: config.CensorScale, spawn: true, color: config.CensorColor);

            Censor.MovementSmoothing = 0;
            Censor.Transform.SetParent(player.Transform, false);

            if (config.AttachCensorToHead)
                Censor.Transform.AttachToTransform(head);

            if (config.CensorRotate)
                Timing.RunCoroutine(Methods.RotateRandom(Censor.Transform));

            Scp96Censors.Add(player, Censor.GameObject);
            Censor.GameObject.HideForUnGlassesPlayer(player);
#endif
        }

        public static void RemoveCensor(this Player player)
        {
            if (!Scp96Censors.ContainsKey(player))
                return;

            GameObject Censor = Scp96Censors[player];

            Scp96Censors.Remove(player);
            NetworkServer.Destroy(Censor);

            foreach (List<Player> ply in ProjectSCRAMBLE.SCRAMBLE.ActiveScramblePlayers.Values)
            {
                ply.Remove(player);
            }
        }

        public static void ObfuscateScp96s(this Player player)
        {
            Dictionary<Player, List<Player>> activeScramblePlayers = ProjectSCRAMBLE.SCRAMBLE.ActiveScramblePlayers;

            foreach (KeyValuePair<Player, GameObject> censor in Scp96Censors)
            {
                player.ShowHidedNetworkObject(censor.Value);
                activeScramblePlayers[player].AddIfNotContains(censor.Key);
            }
        }

        public static void DeObfuscateScp96s(this Player player)
        {
            foreach (GameObject censor in Scp96Censors.Values)
            {
                player.HideNetworkObject(censor);
            }

            ProjectSCRAMBLE.SCRAMBLE.ActiveScramblePlayers.Remove(player);
        }

        private static bool TryGetScp96Head(this Player player, out Transform headTransform)
        {
            headTransform = null;

            if (player.Role is not FpcRole fpc)
            {
                Log.Debug("This 96 role is not have first person control.");
                return false;
            }

            if (fpc.Model is not Scp096CharacterModel scp96AnimatedCharacterModel)
            {
                Log.Debug("This 96 role doesnt have Scp096CharacterModel.");
                return false;
            }

            headTransform = scp96AnimatedCharacterModel.Head;
            return scp96AnimatedCharacterModel.Head != null;
        }

        public static void ShowHidedNetworkObject(this Player player, GameObject networkedObject)
        {
            if (!networkedObject.TryGetComponent(out NetworkIdentity identity))
            {
                Log.Warn($"{networkedObject} not have network identity");
                return;
            }

            Server.SendSpawnMessage.Invoke(null, [identity, player.Connection]);
#if PMER
            foreach (NetworkIdentity netIdentity in networkedObject.GetComponentsInChildren<NetworkIdentity>(true))
            {
                Server.SendSpawnMessage.Invoke(null, [netIdentity, player.Connection]);
            }
#endif
        }

        public static void HideNetworkObject(this Player player, GameObject networkedObject)
        {
            if (!networkedObject.TryGetComponent(out NetworkIdentity identity))
            {
                Log.Warn($"{networkedObject} not have network identity");
                return;
            }

            player.Connection.Send(new ObjectHideMessage(){ netId = identity.netId });
        }
    }
}