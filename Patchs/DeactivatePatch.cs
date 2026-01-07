using HarmonyLib;

using InventorySystem.Items.Pickups;
using InventorySystem.Items.Usables.Scp1344;

using MEC;

using static ProjectSCRAMBLE.ProjectSCRAMBLE;

namespace ProjectSCRAMBLE.Patchs
{
    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.ActivateFinalEffects))]
    public class OnPlayerDisarmedPatch
    {
        public static bool Prefix(Scp1344Item __instance)
        {
            if (!SCRAMBLE.TrackedSerials.Contains(__instance.ItemSerial))
                return true;

            WearOffProjectScramble(__instance.Owner);
            return false;
        }
    }

    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.ServerDropItem))]
    public static class Prevent3114DropPatch
    {
        public static bool Prefix(Scp1344Item __instance, bool spawn, ref ItemPickupBase __result)
        {
            if (!spawn)
                return true;

            if (!SCRAMBLE.TrackedSerials.Contains(__instance.ItemSerial))
                return true;

            if (!__instance.IsWorn)
                return true;

            __instance.ServerSetStatus(Scp1344Status.Idle);
            WearOffProjectScramble(__instance.Owner);
            Timing.CallDelayed(Timing.WaitForOneFrame, () => __instance.OwnerInventory.ServerSelectItem(__instance.ItemSerial));
            return false;
        }
    }
}
