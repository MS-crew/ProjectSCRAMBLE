using HarmonyLib;

using InventorySystem.Items;
using InventorySystem.Items.Usables.Scp1344;

namespace ProjectSCRAMBLE.Patchs
{
    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.CanStartUsing), MethodType.Getter)]
    public class CanStartUsingPatch
    {
        public static void Postfix(Scp1344Item __instance, ref bool __result)
        {
            if (!__result)
                return;

            foreach (ItemBase item in __instance.OwnerInventory.UserInventory.Items.Values)
            {
                if (item.ItemTypeId != ItemType.SCP1344)
                    continue;

                Scp1344Item scp1344 = item as Scp1344Item;
                if (!scp1344.IsWorn)
                    continue;

                __result = false;
                break;
            }
        }
    }
}
