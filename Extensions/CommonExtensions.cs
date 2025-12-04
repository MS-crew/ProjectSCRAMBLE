using System;

using HarmonyLib;

namespace ProjectSCRAMBLE.Extensions
{
    public static class CommonExtensions
    {
        internal static string FormatCharge(this float Float) 
        { 
           return ((int)Float).ToString(); 
        }

        internal static void PatchSingleType(this Harmony harmony, Type patchClass)
        {
            PatchClassProcessor processor = new(harmony, patchClass);
            processor.Patch();
        }
    }
}
