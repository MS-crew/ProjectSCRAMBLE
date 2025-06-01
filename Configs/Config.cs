﻿using UnityEngine;
using Exiled.API.Interfaces;
using System.ComponentModel;

namespace ProjectSCRAMBLE
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Whether to remove the main 1344 effect when using SCRAMBLES")]
        public bool RemoveOrginal1344Effect { get; set; } = true;

        [Description("If you remove the original effect, simulate the temporary darkness when wearing the glasses")]
        public bool SimulateTemporaryDarkness { get; set; } = true;

        [Description("Should there be a Random error in the artificial intelligence of the glasses?")]
        public bool RandomError { get; set; } = false;

        [Description("Random error chance")]
        public float RandomErrorChance { get; set; } = 0.001f;

        [Description("Whether the SCRAMBLES will use charge while blocking SCP-096 face")]
        public bool ScrambleCharge { get; set; } = true;

        [Description("How much power should the SCRAMBLEs use to obfuscate 96's face? (1 = default, >1 = faster, <1 = slower)")]
        public float ChargeUsageMultiplayer { get; set; } = 1;

        [Description("Attach to head or Directl attach to player")]
        public bool AttachCensorToHead { get; set; } = true;

        [Description("0.1 is okey, 0.01 better/good , 0.001 greater")]
        public float AttachToHeadsyncInterval { get; set; } = 0.01f;

        [Description("Censor rotate speed")]
        public float Censorspeed { get; set; } = 250f;

        [Description("Censor Movement Smooth value")]
        public byte CensorMovementSmooth { get; set; } = 30;

        [Description("Censor scale")]
        public Vector3 CensorScale { get; set; } = new Vector3(0.5f, 0.5f , 0.5f);

        [Description("Wearing time (default 5)")]
        public float ActivateTime { get; set; } = 1f;

        [Description("Removal time (default 5.1)")]
        public float DeactivateTime { get; set; } = 1f;

        [Description("Custom item settings")]
        public ProjectSCRAMBLE ProjectSCRAMBLE { get; set; } = new ProjectSCRAMBLE();

        [Description("Hint settings")]
        public Hints Hint { get; set; } = new Hints();
        public class Hints 
        {
            public float XCordinate { get; set; } = 370;
            public float YCordinate { get; set; } = 90;
            public int FontSize { get; set; } = 20;

            [Description("0 = left , 1 = right, 2 = center")]
            public int Alligment { get; set; } = 0;
        }
    }
}
