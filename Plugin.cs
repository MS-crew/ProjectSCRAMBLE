using System;

using ProjectSCRAMBLE.Extensions;
using ProjectSCRAMBLE.Patchs;

using Exiled.API.Features;
using Exiled.CustomItems.API;

using HarmonyLib;

namespace ProjectSCRAMBLE
{
    public class Plugin : Plugin<Config, Translation>
    {
        private Harmony harmony;

        private EventHandlers eventHandlers;

        public static Plugin Instance { get; private set; }

        public override string Author { get; } = "ZurnaSever";

        public override string Name { get; } = "ProjectSCRAMBLE";

        public override string Prefix { get; } = "ProjectSCRAMBLE";

        public override Version Version { get; } = new Version(1, 3, 0);

        public override Version RequiredExiledVersion { get; } = new Version(9, 10, 0);

        public override void OnEnabled()
        {
            Instance = this;
            eventHandlers = new EventHandlers();

            Config.ProjectSCRAMBLE.Register();
            eventHandlers.Subscribe();

            harmony = new Harmony(Prefix + DateTime.Now.Ticks);
            DoDynamicPatchs();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            harmony.UnpatchAll(harmony.Id);

            Config.ProjectSCRAMBLE.Unregister();
            eventHandlers.Unsubscribe();

            eventHandlers = null;
            Instance = null;
            base.OnDisabled();
        }

        private void DoDynamicPatchs()
        {
            if (Config.OverrideWearingTime)
            {
                harmony.PatchSingleType(typeof(SetWearingTime));
            }

            if (Config.OverrideWearingOffTime) 
            {
                harmony.PatchSingleType(typeof(SetWearOffTime));
            }

            if (!Config.ProjectSCRAMBLE.CanWearOff)
                return;

            harmony.PatchSingleType(typeof(BlockBadEffect));
            harmony.PatchSingleType(typeof(BlockForceDrop));
        }
    }
}
