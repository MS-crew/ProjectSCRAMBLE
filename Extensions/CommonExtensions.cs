using System;
using System.Collections.Generic;

using Exiled.API.Features;

using HarmonyLib;

using MEC;

using UnityEngine;

using Utils.NonAllocLINQ;

namespace ProjectSCRAMBLE.Extensions
{
    public static class CommonExtensions
    {
        public static string FormatCharge(this float Float) 
        { 
           return ((int)Float).ToString(); 
        }

        public static void PatchSingleType(this Harmony harmony, Type patchClass)
        {
            PatchClassProcessor processor = new(harmony, patchClass);
            processor.Patch();
        }

        public static void AttachToTransform(this Transform thisTransform, Transform targetTransform)
        {
            Timing.RunCoroutine(Methods.TrackHead(thisTransform, targetTransform, Plugin.Instance.Config.AttachToHeadsyncInterval));
        }

        public static void HideForUnGlassesPlayer(this GameObject gameObject, Player CensorOwner)
        {
            foreach (Player normalply in Player.List)
            {
                if (normalply.IsHost)
                    continue;

                if (ProjectSCRAMBLE.SCRAMBLE.ActiveScramblePlayers.TryGetValue(normalply, out List<Player> scp96s))
                {
                    scp96s.AddIfNotContains(CensorOwner);
                    continue;
                }

                normalply.HideNetworkObject(gameObject);
            }
        }
    }
}
