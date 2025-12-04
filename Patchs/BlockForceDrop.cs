using Exiled.API.Features;
using Exiled.API.Features.Items;

using HarmonyLib;

using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Usables.Scp1344;

using MEC;

namespace ProjectSCRAMBLE.Patchs
{
    [HarmonyPatch(typeof(ItemBase), nameof(ItemBase.ServerDropItem), typeof(bool))]
    public static class BlockForceDrop
    {
        public static void Postfix(ItemBase __instance, ref ItemPickupBase __result)
        {
            if (__result == null) 
                return;

            if (!ProjectSCRAMBLE.SCRAMBLE.TrackedSerials.Contains(__result.Info.Serial))
                return;

            if (!Item.Get(__instance).Is(out Scp1344 scp1344Item))
                return;

            if (scp1344Item.Status != Scp1344Status.Deactivating)
                return;

            ItemPickupBase pickup = __result;
            Timing.CallDelayed(0.5f, () => CheckIsCuffed(pickup));
        }

        private static void CheckIsCuffed(ItemPickupBase pickup)
        {
            Player owner = Player.Get(pickup.PreviousOwner.Hub);
            if (owner.IsCuffed)
                return;

            pickup.PreviousOwner.Hub.inventory.ServerAddItem(pickup.ItemId.TypeId, ItemAddReason.Undefined, pickup.Info.Serial, pickup);
            pickup.DestroySelf();
        }
    }
}
