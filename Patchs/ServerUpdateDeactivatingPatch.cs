using System;
using System.Collections.Generic;
using System.Reflection.Emit;

using Exiled.API.Features.Pools;

using HarmonyLib;

using InventorySystem;
using InventorySystem.Items.Usables.Scp1344;

using static HarmonyLib.AccessTools;

namespace ProjectSCRAMBLE.Patchs
{
    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.ServerUpdateDeactivating))]
    public static class ServerUpdateDeactivatingPatch
    {
        internal static event Action<ReferenceHub> OnProjectScrambleWearOff;
        internal static void WearOffProjectScramble(ReferenceHub hub) => OnProjectScrambleWearOff?.Invoke(hub);

        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> NewCodes = ListPool<CodeInstruction>.Pool.Get(instructions);

            if (Plugin.Instance.Config.CanWearOff)
            {
                List<Label> jumpLabels;
                Label Skip = generator.DefineLabel();

                const int offset = -1;
                int index = NewCodes.FindLastIndex(code => code.Calls(Method(typeof(Scp1344Item), nameof(Scp1344Item.ActivateFinalEffects)))) + offset;

                jumpLabels = NewCodes[index].ExtractLabels();
                NewCodes[index].labels.Add(Skip);

                NewCodes.InsertRange(index,
                [
                    new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(ProjectSCRAMBLE), nameof(ProjectSCRAMBLE.SCRAMBLE))).WithLabels(jumpLabels),
                    new(OpCodes.Callvirt,  PropertyGetter(typeof(ProjectSCRAMBLE), nameof(ProjectSCRAMBLE.TrackedSerials))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp1344Item), nameof(Scp1344Item.ItemSerial))),
                    new(OpCodes.Callvirt,  Method(typeof(HashSet<int>), nameof(HashSet<int>.Contains), [typeof(ushort)])),
                    new(OpCodes.Brfalse_S, Skip),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldc_I4, (int)Scp1344Status.Idle),
                    new(OpCodes.Callvirt , PropertySetter(typeof(Scp1344Item), nameof(Scp1344Item.Status))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldc_R4, 0f),
                    new(OpCodes.Stfld,  Field(typeof(Scp1344Item), nameof(Scp1344Item._useTime))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldc_R4, 0f),
                    new(OpCodes.Stfld,  Field(typeof(Scp1344Item), nameof(Scp1344Item._cancelationTime))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt,  PropertyGetter(typeof(Scp1344Item), nameof(Scp1344Item.Owner))),
                    new(OpCodes.Ldfld,  Field(typeof(ReferenceHub), nameof(ReferenceHub.inventory))),
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Callvirt,  Method(typeof(Inventory), nameof(Inventory.ServerSelectItem))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt,  PropertyGetter(typeof(Scp1344Item), nameof(Scp1344Item.Owner))),
                    new(OpCodes.Call, Method(typeof(ServerUpdateDeactivatingPatch), nameof(ServerUpdateDeactivatingPatch.WearOffProjectScramble))),
                    new(OpCodes.Ret),
                ]);
            }

            Label Skip2 = generator.DefineLabel();
            Label Skip3 = generator.DefineLabel();

            int index2 = NewCodes.FindIndex(code => code.opcode == OpCodes.Ldc_R4 && (float)code.operand == Scp1344Item.DeactivationTime);

            NewCodes[index2].labels.Add(Skip2);
            NewCodes[index2 + 1].labels.Add(Skip3);

            NewCodes.InsertRange(index2,
            [
                new(OpCodes.Call,      PropertyGetter(typeof(ProjectSCRAMBLE), nameof(ProjectSCRAMBLE.SCRAMBLE))),
                new(OpCodes.Callvirt,  PropertyGetter(typeof(ProjectSCRAMBLE), nameof(ProjectSCRAMBLE.TrackedSerials))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt,  PropertyGetter(typeof(Scp1344Item), nameof(Scp1344Item.ItemSerial))),
                new(OpCodes.Callvirt,  Method(typeof(HashSet<int>), nameof(HashSet<int>.Contains), [typeof(ushort)])),
                new(OpCodes.Brfalse_S, Skip2),
                new(OpCodes.Ldc_R4,    Plugin.Instance.Config.WearingOffTime),
                new(OpCodes.Br_S ,     Skip3)
            ]);

            for (int i = 0; i < NewCodes.Count; i++)
            {
                yield return NewCodes[i];
            }

            ListPool<CodeInstruction>.Pool.Return(NewCodes);
        }
    }
}
