using System;

using HarmonyLib;

using InventorySystem.Items.Usables.Scp1344;

using static ProjectSCRAMBLE.ProjectSCRAMBLE;

namespace ProjectSCRAMBLE.Patchs
{
    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.ActivateFinalEffects))]
    public class BlockBadEffect
    {
        internal static event Action<ReferenceHub> OnProjectScrambleWearOff;
        internal static void WearOffProjectScramble(ReferenceHub hub) => OnProjectScrambleWearOff?.Invoke(hub);

        public static bool Prefix(Scp1344Item __instance)
        {
            if (!SCRAMBLE.TrackedSerials.Contains(__instance.ItemSerial))
                return true;

            WearOffProjectScramble(__instance.Owner);
            return false;
        }
    }
}
