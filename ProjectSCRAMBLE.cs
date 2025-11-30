using System.Collections.Generic;

using Exiled.API.Enums;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.EventArgs;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Scp1344;

using InventorySystem.Items.Usables.Scp1344;

using MEC;

using ProjectSCRAMBLE.Extensions;

using UnityEngine;

using YamlDotNet.Serialization;

using Scp1344event = Exiled.Events.Handlers.Scp1344;

namespace ProjectSCRAMBLE
{
    [CustomItem(ItemType.SCP1344)]
    public class ProjectSCRAMBLE : CustomItem
    {
        public Dictionary<ushort, float> ScrambleCharges { get; set; } = [];

        public Dictionary<Player, List<Player>> ActiveScramblePlayers { get; set; } = [];

        internal static ProjectSCRAMBLE SCRAMBLE { get; set; }

        public bool CanWearOff { get; set; } = true;

        public override uint Id { get; set; } = 1730;

        public override float Weight { get; set; } = 1f;

        public override string Name { get; set; } = "Project SCRAMBLE";

        [YamlIgnore]
        public override ItemType Type { get; set; } = ItemType.SCP1344;

        public override SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();

        public override string Description { get; set; } = "An artificial intelligence Visor that censors SCP-096's face";

        public override void Init()
        {
            base.Init();
            SCRAMBLE = this;
        }

        public override void Destroy()
        {
            SCRAMBLE = null;
            base.Destroy();
        }

        protected override void SubscribeEvents()
        {
            Scp1344event.ChangedStatus += OnChangedStatus;
            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Scp1344event.ChangedStatus -= OnChangedStatus;
            base.UnsubscribeEvents();
        }

        protected override void OnUpgrading(UpgradingEventArgs ev)
        {
            switch(ev.KnobSetting)
            {
                case Scp914.Scp914KnobSetting.Rough:
                    ScrambleCharges[ev.Pickup.Serial] = 0f;
                    break;

                case Scp914.Scp914KnobSetting.Coarse:
                    float charge = Random.Range(0, 50f);
                    ScrambleCharges[ev.Pickup.Serial] = charge;
                    break;

                case Scp914.Scp914KnobSetting.Fine:
                case Scp914.Scp914KnobSetting.VeryFine:
                    ScrambleCharges[ev.Pickup.Serial] = 100f;
                    break;
            }
        }

        protected override void OnUpgrading(UpgradingItemEventArgs ev)
        {
            switch (ev.KnobSetting)
            {
                case Scp914.Scp914KnobSetting.Rough:
                    ScrambleCharges[ev.Item.Serial] = 0f;
                    break;

                case Scp914.Scp914KnobSetting.Coarse:
                    float charge = Random.Range(0, 50f);
                    ScrambleCharges[ev.Item.Serial] = charge;
                    break;

                case Scp914.Scp914KnobSetting.Fine:
                case Scp914.Scp914KnobSetting.VeryFine:
                    ScrambleCharges[ev.Item.Serial] = 100f;
                    break;
            }
        }

        private void OnChangedStatus(ChangedStatusEventArgs ev)
        {
            if (!Check(ev.Item))
                return;

            if (ev.Scp1344Status != Scp1344Status.Active)
                return;

            if (Plugin.Instance.Config.ScrambleCharge)
            {
                ushort serial = ev.Item.Serial;

                if (!ScrambleCharges.TryGetValue(serial, out float charge))
                {
                    charge = 100f;
                    ScrambleCharges[serial] = charge;
                    Log.Debug($"Initialized SCRAMBLE charge for serial {serial}.");
                }

                else if (charge <= 0f)
                {
                    if (Plugin.Instance.Config.RemoveOrginal1344Effect) 
                        RemoveOrginalEffect(ev.Player);

                    ev.Player.DeObfuscateScp96s();
                    ev.Player.AddSCRAMBLEHint(Plugin.Instance.Translation.OffCharge);
                    Log.Debug($"{ev.Player.Nickname}: Tried to wear SCRAMBLE with no charge.");
                    return;
                }

                string hint = Plugin.Instance.Translation.Charge.Replace("{charge}", charge.FormatCharge());
                ev.Player.AddSCRAMBLEHint(hint);
                Log.Debug($"{ev.Player.Nickname}: SCRAMBLEs charge {charge}.");
            }

            if (Plugin.Instance.Config.RemoveOrginal1344Effect)
                RemoveOrginalEffect(ev.Player);  

            ActiveScramblePlayers[ev.Player] = [];
            ev.Player.ObfuscateScp96s();
            Log.Debug($"{ev.Player.Nickname}: Activated Project SCRAMBLE");
        }

        private void RemoveOrginalEffect(Player player)
        {
            Timing.CallDelayed(1f, () =>
            {
                if (player == null || player.IsDead)
                    return;

                player.SendFakeEffectTo(player, EffectType.Scp1344, 0);

                if (Plugin.Instance.Config.SimulateTemporaryDarkness)
                    player.EnableEffect(EffectType.Blinded, 255, float.MaxValue, true);
            });
        }
    }
}
