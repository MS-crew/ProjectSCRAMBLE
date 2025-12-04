using HarmonyLib;

using InventorySystem.Items;
using InventorySystem.Items.Usables.Scp1344;

using static ProjectSCRAMBLE.ProjectSCRAMBLE;

namespace ProjectSCRAMBLE.Patchs
{
    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.OnPlayerDisarmed))]
    public class OnPlayerDisarmedPatch
    {
        public static bool Prefix(Scp1344Item __instance, ReferenceHub disarmerHub, ReferenceHub targetHub)
        {
            if (!SCRAMBLE.TrackedSerials.Contains(__instance.ItemSerial))
                return true;

            if (__instance is not ItemBase itemBase)
                return true;

            itemBase.ServerDropItem(true);
            ServerUpdateDeactivatingPatch.WearOffProjectScramble(targetHub);
            return false;
        }
    }
}