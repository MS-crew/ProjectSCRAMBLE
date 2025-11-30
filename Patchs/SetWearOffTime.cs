using System.Reflection.Emit;
using System.Collections.Generic;
using InventorySystem.Items.Usables.Scp1344;

using Exiled.API.Features.Pools;

using HarmonyLib;
using static HarmonyLib.AccessTools;

namespace ProjectSCRAMBLE.Patchs
{
    [HarmonyPatch(typeof(Scp1344Item), nameof(Scp1344Item.ServerUpdateDeactivating))]
    public static class SetWearOffTime
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> NewCodes = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label Skip = generator.DefineLabel();
            Label Skip2 = generator.DefineLabel();

            int index = NewCodes.FindIndex(code => code.opcode == OpCodes.Ldc_R4 && (float)code.operand == Scp1344Item.DeactivationTime);

            NewCodes[index].labels.Add(Skip);
            NewCodes[index + 1].labels.Add(Skip2);

            NewCodes.InsertRange(index,
            [
                new(OpCodes.Call,      PropertyGetter(typeof(ProjectSCRAMBLE), nameof(ProjectSCRAMBLE.SCRAMBLE))),
                new(OpCodes.Callvirt,  PropertyGetter(typeof(ProjectSCRAMBLE), nameof(ProjectSCRAMBLE.TrackedSerials))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt,  PropertyGetter(typeof(Scp1344Item), nameof(Scp1344Item.ItemSerial))),
                new(OpCodes.Callvirt,  Method(typeof(HashSet<int>), nameof(HashSet<int>.Contains), [typeof(int)])),
                new(OpCodes.Brfalse_S, Skip),
                new(OpCodes.Ldc_R4,    Plugin.Instance.Config.WearingOffTime),
                new(OpCodes.Br_S ,     Skip2)
            ]);

            for (int i = 0; i < NewCodes.Count; i++)
                yield return NewCodes[i];

            ListPool<CodeInstruction>.Pool.Return(NewCodes);
        }
    }
}
