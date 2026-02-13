using System;

using Exiled.API.Features;
using Exiled.CustomItems.API;

using ProjectSCRAMBLE.Configs;

namespace ProjectSCRAMBLE
{
    public class Plugin : Plugin<Config, Translation>
    {
        public static Plugin Instance { get; private set; }

        public EventHandlers EventHandlers { get; private set; }

        public override string Author { get; } = "MS";

        public override string Name { get; } = "ProjectSCRAMBLE";

        public override string Prefix { get; } = "ProjectSCRAMBLE";

        public override Version Version { get; } = new Version(1, 5, 0);

        public override Version RequiredExiledVersion { get; } = new Version(9, 13, 0);

        public override void OnEnabled()
        {
            Instance = this;
            EventHandlers = new EventHandlers();

            Config.ProjectSCRAMBLE.Register();
            EventHandlers.Subscribe();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Config.ProjectSCRAMBLE.Unregister();
            EventHandlers.Unsubscribe();

            EventHandlers = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}
