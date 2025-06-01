using MEC;
using UnityEngine;
using PlayerRoles;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Extensions;
using ProjectSCRAMBLE.Extensions;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;
using Scp96Event = Exiled.Events.Handlers.Scp096;
using PlayerEvent = Exiled.Events.Handlers.Player;
using Exiled.API.Features.Toys;

namespace ProjectSCRAMBLE
{
    public class EventHandlers
    {
        public void Sucsribe()
        {
            PlayerEvent.Verified += OnVerified;
            PlayerEvent.Spawned += OnChangedRole;
            PlayerEvent.ReceivingEffect += OnChangeEffect;
            
            Scp96Event.AddingTarget += OnAddingTarget;
        }

        public void UnSucsribe()
        {
            PlayerEvent.Verified -= OnVerified;
            PlayerEvent.Spawned -= OnChangedRole;
            PlayerEvent.ReceivingEffect -= OnChangeEffect;

            Scp96Event.AddingTarget -= OnAddingTarget;
        }

        private void OnChangedRole(SpawnedEventArgs ev)
        {
            if (ProjectSCRAMBLE.SCRAMBLE == null)
                return;

            if (ev.OldRole == RoleTypeId.Scp096 && ev.Player.Role != RoleTypeId.Scp096)
            {
                ev.Player.RemoveCensor();
                Log.Debug($"Scp96:{ev.Player.Nickname} removed censor");
            }
            else if (ev.Player.Role == RoleTypeId.Scp096)
            {
                Timing.CallDelayed(0.5f, () => ev.Player.AddCensor());
                Log.Debug($"Scp96:{ev.Player.Nickname} added censor");
            }
        }

        public void OnAddingTarget(AddingTargetEventArgs ev)
        {
            if (!ev.IsLooking || !ProjectSCRAMBLE.ActiveScramblePlayers.ContainsKey(ev.Target))
                return;

            bool shouldRandomError = Plugin.Instance.Config.RandomError && Random.Range(0f, 100f) <= Plugin.Instance.Config.RandomErrorChance;

            if (!Plugin.Instance.Config.ScrambleCharge)
            {
                if (shouldRandomError)
                {
                    ev.Target.AddSCRAMBLEHint(Plugin.Instance.Translation.Error);
                    return;
                }

                ev.IsAllowed = false;
                return;
            }

            ushort serial = 0;
            foreach (var key in ev.Target.Inventory.UserInventory.Items.Keys)
            {
                if (ProjectSCRAMBLE.SCRAMBLE.TrackedSerials.Contains(key))
                {
                    serial = key;
                    break;
                }
            }

            if (serial == 0)
            {
                string playerSerials = string.Join(", ", ev.Target.Inventory.UserInventory.Items.Keys.Select(k => k.ToString()));
                string trackedSerials = string.Join(", ", ProjectSCRAMBLE.SCRAMBLE.TrackedSerials.Select(s => s.ToString()));
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

            if (!ProjectSCRAMBLE.ScrambleCharges.TryGetValue(serial, out float charge))
            {
                ProjectSCRAMBLE.ScrambleCharges[serial] = 100f;
                ev.Target.AddSCRAMBLEHint(Plugin.Instance.Translation.Charge.Replace("{charge}", charge.FormatCharge()));
                ev.IsAllowed = false;
                return;
            }

            if (charge <= 0f)
            {
                ev.Target.AddSCRAMBLEHint(Plugin.Instance.Translation.OffCharge);
                ev.Target.DeObfuscateScp96s();
                return;
            }

            ProjectSCRAMBLE.ScrambleCharges[serial] -= Time.deltaTime * Plugin.Instance.Config.ChargeUsageMultiplayer;

            if (shouldRandomError)
            {
                ev.Target.AddSCRAMBLEHint(Plugin.Instance.Translation.Error);
                Timing.CallDelayed(0.5f , () => ev.Target.AddSCRAMBLEHint(Plugin.Instance.Translation.Charge.Replace("{charge}", charge.FormatCharge())));
                return;
            }
            
            ev.Target.AddSCRAMBLEHint(Plugin.Instance.Translation.Charge.Replace("{charge}", charge.FormatCharge()));
            ev.IsAllowed = false;
        }

        private void OnChangeEffect(ReceivingEffectEventArgs ev)
        {
            if (ev.Effect.GetEffectType() != EffectType.Scp1344 || !ev.Effect.IsEnabled)
                return;

            ev.Player.RemoveSCRAMBLEHint();

            if (!ProjectSCRAMBLE.ActiveScramblePlayers.ContainsKey(ev.Player))
                return;
            
            ev.Player.DeObfuscateScp96s();
            Log.Debug("Player wear-off Project SCRAMBLE");
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            foreach (Primitive Censormain in PlayerExtensions.Scp96sCencors.Values)
            {
                ev.Player.DestroyCensor(Censormain);
            }
        }
    }
}
