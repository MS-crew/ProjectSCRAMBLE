using Exiled.API.Features;
using Exiled.API.Features.Roles;

using Mirror;

using PlayerRoles.PlayableScps.Scp096;

using UnityEngine;

using Scp096Role = Exiled.API.Features.Roles.Scp096Role;

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
    public static class Extensions
    {
#if RUEI
        private static readonly Tag scrambleHintTag = new("ProjectScramble");
#elif HSM
        private static readonly string hsmID = "SCRAMBLE";
#endif

        internal static void AddSCRAMBLEHint(this Player player, string text)
        {
#if RUEI
            RueDisplay.Get(player).Show(scrambleHintTag, new BasicElement(Plugin.Instance.Config.Hint.YCordinate, text));
#elif HSM
            PlayerDisplay pd = player.GetPlayerDisplay();

            if (pd.GetHint(hsmID) != null)
            {
                pd.GetHint(hsmID).Text = text;
                return;
            }

            HintServiceMeow.Core.Models.Hints.Hint newHint = new()
            {
                Id = hsmID,
                YCoordinate = Plugin.Instance.Config.Hint.YCordinate,
                XCoordinate = Plugin.Instance.Config.Hint.XCordinate,
                FontSize = Plugin.Instance.Config.Hint.FontSize,
                Alignment = (HintServiceMeow.Core.Enum.HintAlignment)Plugin.Instance.Config.Hint.Alligment,
                Text = text
            };

            pd.AddHint(newHint);
#endif
        }

        internal static void RemoveSCRAMBLEHint(this Player player)
        {
#if RUEI
            RueDisplay.Get(player).Remove(scrambleHintTag);
#elif HSM
            PlayerDisplay pd = player.GetPlayerDisplay();
            if (pd.GetHint(hsmID) != null)
                pd.RemoveHint(hsmID);
#endif
        }

        internal static bool TryGetScp96Head(this Player player, out Transform headTransform)
        {
            headTransform = null;

            if (player.Role is not Scp096Role scp96Role)
            {
                Log.Debug("This is not 96 role.");
                return false;
            }

            headTransform = scp96Role.HeadTransform;
            return headTransform != null;
        }

        internal static void ShowHidedNetworkObject(this Player player, GameObject networkedObject)
        {
            if (!networkedObject.TryGetComponent(out NetworkIdentity identity))
            {
                Log.Warn($"{networkedObject} not have network identity");
                return;
            }

            NetworkServer.SendSpawnMessage(identity, player.Connection);
#if PMER
            foreach (NetworkIdentity netIdentity in networkedObject.GetComponentsInChildren<NetworkIdentity>(true))
            {
                NetworkServer.SendSpawnMessage(netIdentity, player.Connection);
            }
#endif
        }

        internal static void HideNetworkObject(this Player player, GameObject networkedObject)
        {
            if (!networkedObject.TryGetComponent(out NetworkIdentity identity))
            {
                Log.Warn($"{networkedObject} not have network identity");
                return;
            }

            player.Connection.Send(new ObjectHideMessage(){ netId = identity.netId });
        }

        internal static string FormatCharge(this float Float)
        {
            return ((int)Float).ToString();
        }
    }
}