using System;

using Exiled.API.Features;
using Exiled.CustomItems.API;

using HarmonyLib;

using ProjectSCRAMBLE.Configs;

namespace ProjectSCRAMBLE
{
    public class Plugin : Plugin<Config, Translation>
    {
        private Harmony harmony;

        public static Plugin Instance { get; private set; }

        public EventHandlers EventHandlers { get; private set; }

        public override string Author { get; } = "ZurnaSever";

        public override string Name { get; } = "ProjectSCRAMBLE";

        public override string Prefix { get; } = "ProjectSCRAMBLE";

        public override Version Version { get; } = new Version(1, 4, 2);

        public override Version RequiredExiledVersion { get; } = new Version(9, 12, 0);

        public override void OnEnabled()
        {
            Instance = this;
            EventHandlers = new EventHandlers();

            Config.ProjectSCRAMBLE.Register();
            EventHandlers.Subscribe();

            harmony = new Harmony(Prefix + DateTime.Now.Ticks);
            harmony.PatchAll();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            harmony.UnpatchAll(harmony.Id);

            Config.ProjectSCRAMBLE.Unregister();
            EventHandlers.Unsubscribe();

            EventHandlers = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}
