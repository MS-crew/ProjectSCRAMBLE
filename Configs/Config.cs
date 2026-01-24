using System.ComponentModel;

using Exiled.API.Interfaces;

using UnityEngine;

namespace ProjectSCRAMBLE.Configs
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Should there be a Random error in the artificial intelligence of the glasses?")]
        public bool RandomError { get; set; } = false;

        [Description("Random error chance")]
        public float RandomErrorChance { get; set; } = 0.001f;

        [Description("Whether the SCRAMBLES will use charge while blocking SCP-096 face")]
        public bool ScrambleCharge { get; set; } = true;

        [Description("How much power should the SCRAMBLEs use to obfuscate 96's face? (1 = default, >1 = faster, <1 = slower)")]
        public float ChargeUsageMultiplayer { get; set; } = 1;

        [Description("Attach to head or Directly attach to player")]
        public bool AttachCensorToHead { get; set; } = true;

        [Description("0.1 is okey, 0.01 better/good , 0.001 greater")]
        public float AttachToHeadsyncInterval { get; set; } = 0.01f;

#if PMER
        [Description("Censor schematic name")]
        public string CensorSchematic { get; set; } = "Censormain";
#else
        [Description("Censor type as primitive")]
        public PrimitiveType CensorType { get; set; } = PrimitiveType.Cube;

        [Description("Rotate censor randomly")]
        public bool CensorRotate { get; set; } = true;

        [Description("Censor Color")]
        public Color CensorColor { get; set; } = new Color(0, 0, 0, 1);
#endif

        [Description("Censor scale")]
        public Vector3 CensorScale { get; set; } = Vector3.one * 0.5f;

        [Description("Custom item settings")]
        public ProjectSCRAMBLE ProjectSCRAMBLE { get; set; } = new ProjectSCRAMBLE();


        [Description("Hint settings")]
        public Hints Hint { get; set; } = new Hints();
        public class Hints 
        {
            public float YCordinate { get; set; } = 90;
#if HSM
            public float XCordinate { get; set; } = 370;
            public int FontSize { get; set; } = 20;

            [Description("0 = left , 1 = right, 2 = center")]
            public int Alligment { get; set; } = 0;
#endif
        }
    }
}
