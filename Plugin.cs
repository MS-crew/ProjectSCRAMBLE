using System;
using HarmonyLib;
using Exiled.API.Features;
using Exiled.CustomItems.API;

namespace ProjectSCRAMBLE
{
    public class Plugin : Plugin<Config, Translation>
    {
        private Harmony harmony;

        public static EventHandlers eventHandlers;

        public override string Author => "ZurnaSever";

        public override string Name => "ProjectSCRAMBLE";

        public override string Prefix => "ProjectSCRAMBLE";

        public static Plugin Instance { get; private set; }

        public override Version Version { get; } = new Version(1, 2, 0);

        public override Version RequiredExiledVersion { get; } = new Version(9, 6, 0);

        public override void OnEnabled()
        {
            Instance = this;
            eventHandlers = new EventHandlers();

            Config.ProjectSCRAMBLE.Register();
            eventHandlers.Sucsribe();

            harmony = new Harmony("ProjectSCRAMBLE" + DateTime.Now.Ticks);
            harmony.PatchAll();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            harmony.UnpatchAll(harmonyID: "ProjectSCRAMBLE");

            Config.ProjectSCRAMBLE.Unregister();
            eventHandlers.UnSucsribe();

            eventHandlers = null;
            Instance = null;
            base.OnDisabled();
        } 
    }
}
