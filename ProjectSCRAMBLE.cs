using System.Collections.Generic;
using System.Linq;

using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.CustomItems.API.EventArgs;
using Exiled.CustomItems.API.Features;
using Exiled.Events.EventArgs.Scp096;
using Exiled.Events.EventArgs.Scp1344;

using InventorySystem.Items.Usables.Scp1344;

using MEC;

using PlayerRoles.FirstPersonControl.Thirdperson.Subcontrollers.Wearables;

using ProjectSCRAMBLE.Configs;
using ProjectSCRAMBLE.Extensions;

using UnityEngine;

using YamlDotNet.Serialization;

using static ProjectSCRAMBLE.Methods;

using Scp1344event = Exiled.Events.Handlers.Scp1344;
using Scp96Event = Exiled.Events.Handlers.Scp096;

namespace ProjectSCRAMBLE
{
    [CustomItem(ItemType.SCP1344)]
    public class ProjectSCRAMBLE : CustomItem
    {
        internal static event System.Action<ReferenceHub> OnProjectScrambleWearOff;
        internal static void WearOffProjectScramble(ReferenceHub hub) => OnProjectScrambleWearOff?.Invoke(hub);

        internal static ProjectSCRAMBLE SCRAMBLE { get; set; }

        [YamlIgnore]
        public Dictionary<ushort, float> ScrambleCharges { get; set; } = [];

        [YamlIgnore]
        public HashSet<Player> ActiveScramblePlayers { get; set; } = [];

        public override uint Id { get; set; } = 1730;

        public override float Weight { get; set; } = 1f;

        public override string Name { get; set; } = "Project SCRAMBLE";

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
            Scp96Event.AddingTarget += OnAddingTarget;
            Scp1344event.ChangedStatus += OnChangedStatus;
            OnProjectScrambleWearOff += DisableScramble;

            base.SubscribeEvents();
        }

        protected override void UnsubscribeEvents()
        {
            Scp96Event.AddingTarget -= OnAddingTarget;
            Scp1344event.ChangedStatus -= OnChangedStatus;
            OnProjectScrambleWearOff -= DisableScramble;

            base.UnsubscribeEvents();
        }

        protected override void OnWaitingForPlayers()
        {
            ScrambleCharges.Clear();
            ActiveScramblePlayers.Clear();

            base.OnWaitingForPlayers();
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

            if (ev.Player.IsHost)
                return;

            if (ev.Scp1344Status != Scp1344Status.Active)
                return;

            if (ActiveScramblePlayers.Contains(ev.Player))
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
                    ev.Player.DisableEffect(EffectType.Scp1344); 
                    ev.Player.AddSCRAMBLEHint(Plugin.Instance.Translation.OffCharge);
                    ev.Player.ReferenceHub.EnableWearables(WearableElements.Scp1344Goggles);
                    Log.Debug($"{ev.Player.Nickname}: Tried to wear SCRAMBLE with no charge.");
                    return;
                }

                string hint = Plugin.Instance.Translation.Charge.Replace("{charge}", charge.FormatCharge());
                ev.Player.AddSCRAMBLEHint(hint);

                Log.Debug($"{ev.Player.Nickname}: SCRAMBLEs charge {charge}.");
            }

            ActivateScramble(ev.Player);
        }

        public void OnAddingTarget(AddingTargetEventArgs ev)
        {
            if (!ev.IsLooking)
                return;

            if (!ActiveScramblePlayers.Contains(ev.Target))
                return;

            Config config = Plugin.Instance.Config;
            Translation translation = Plugin.Instance.Translation;

            bool shouldRandomError = config.RandomError && Random.Range(0f, 100f) <= config.RandomErrorChance;

            if (!config.ScrambleCharge)
            {
                if (shouldRandomError)
                {
                    ev.Target.AddSCRAMBLEHint(translation.Error);
                    return;
                }

                ev.IsAllowed = false;
                return;
            }

            ushort serial = 0;

            var items = ev.Target.Inventory.UserInventory.Items.Keys;
            foreach (ushort key in items)
            {
                if (TrackedSerials.Contains(key))
                {
                    serial = key;
                    break;
                }
            }

            if (serial == 0)
            {
                string playerSerials = string.Join(", ", items.Select(k => k.ToString()));
                string trackedSerials = string.Join(", ", TrackedSerials.Select(s => s.ToString()));
                Log.Debug
                    ($"""
                    [SCRAMBLE ERROR]
                    Player: {ev.Target.Nickname} ({ev.Target.UserId})
                    Reason: No matching SCRAMBLE serial found.
                    Player Serial Keys: [{playerSerials}]
                    Tracked Serial Keys: [{trackedSerials}]
                    """);
                ev.IsAllowed = false;
                return;
            }

            if (!ScrambleCharges.TryGetValue(serial, out float charge))
            {
                charge = 100f;
                ScrambleCharges[serial] = charge;
                ev.Target.AddSCRAMBLEHint(translation.Charge.Replace("{charge}", charge.FormatCharge()));
                ev.IsAllowed = false;
                return;
            }

            if (charge <= 0f)
            {
                ev.Target.AddSCRAMBLEHint(translation.OffCharge);
                DeObfuscateScp96s(ev.Target);
                return;
            }

            SCRAMBLE.ScrambleCharges[serial] -= Time.deltaTime * config.ChargeUsageMultiplayer;

            if (shouldRandomError)
            {
                ev.Target.AddSCRAMBLEHint(translation.Error);
                Timing.CallDelayed(0.5f, () => ev.Target.AddSCRAMBLEHint(translation.Charge.Replace("{charge}", charge.FormatCharge())));
                return;
            }

            ev.Target.AddSCRAMBLEHint(translation.Charge.Replace("{charge}", charge.FormatCharge()));
            ev.IsAllowed = false;
        }

        private void ActivateScramble(Player player)
        {
            Config config = Plugin.Instance.Config;

            player.DisableEffect(EffectType.Scp1344);
            player.ReferenceHub.EnableWearables(WearableElements.Scp1344Goggles);

            if (config.SimulateTemporaryDarkness)
                player.EnableEffect(EffectType.Blinded, 255, float.MaxValue, true);

            ObfuscateScp96s(player);
            ActiveScramblePlayers.Add(player);

            foreach (Player ply in player.CurrentSpectatingPlayers)
            {
                ObfuscateScp96s(ply);
                Plugin.Instance.EventHandlers.DirtyPlayers.Add(ply);
            }

            Log.Debug($"{player.Nickname}: Activated Project Scramble");
        }

        private void DisableScramble(ReferenceHub hub)
        {
            Player player = Player.Get(hub);

            if (!ActiveScramblePlayers.Contains(player))
                return;

            player.DisableEffect(EffectType.Blinded);
            player.ReferenceHub.DisableWearables(WearableElements.Scp1344Goggles);
            
            DeObfuscateScp96s(player); 
            player.RemoveSCRAMBLEHint();
            ActiveScramblePlayers.Remove(player);

            foreach (Player ply in player.CurrentSpectatingPlayers)
            {
                DeObfuscateScp96s(ply);
                Plugin.Instance.EventHandlers.DirtyPlayers.Remove(ply);
            }

            Log.Debug($"{player.Nickname} : Deactivated  Project Scramble");
        }

        public void ObfuscateScp96s(Player player)
        {
            foreach (GameObject censor in Scp96Censors.Values)
            {
                player.ShowHidedNetworkObject(censor);
            }
        }

        public void DeObfuscateScp96s(Player player)
        {
            foreach (GameObject censor in Scp96Censors.Values)
            {
                player.HideNetworkObject(censor);
            }
        }
    }
}
