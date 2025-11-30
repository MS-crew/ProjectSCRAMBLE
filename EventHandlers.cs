using System.Linq;

using MEC;
using UnityEngine;
using PlayerRoles;
using ProjectSCRAMBLE.Extensions;

using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.API.Extensions;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Scp096;

using Scp96Event = Exiled.Events.Handlers.Scp096;
using PlayerEvent = Exiled.Events.Handlers.Player;

#if PMER
using ProjectMER.Features.Objects;
#endif

namespace ProjectSCRAMBLE
{
    public class EventHandlers
    {
        public void Subscribe()
        {
            PlayerEvent.Verified += OnVerified;
            PlayerEvent.Spawned += OnChangedRole;
            PlayerEvent.ReceivingEffect += OnChangeEffect;

            Scp96Event.AddingTarget += OnAddingTarget;
        }

        public void Unsubscribe()
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
                Timing.CallDelayed(0.5f, ev.Player.AddCensor);
                Log.Debug($"Scp96:{ev.Player.Nickname} added censor");
            }
        }

        public void OnAddingTarget(AddingTargetEventArgs ev)
        {
            if (!ev.IsLooking)
                return;

            if (!ProjectSCRAMBLE.SCRAMBLE.ActiveScramblePlayers.ContainsKey(ev.Target))
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

            if (!ProjectSCRAMBLE.SCRAMBLE.ScrambleCharges.TryGetValue(serial, out float charge))
            {
                charge = 100f;
                ProjectSCRAMBLE.SCRAMBLE.ScrambleCharges[serial] = charge;
                ev.Target.AddSCRAMBLEHint(translation.Charge.Replace("{charge}", charge.FormatCharge()));
                ev.IsAllowed = false;
                return;
            }

            if (charge <= 0f)
            {
                ev.Target.AddSCRAMBLEHint(translation.OffCharge);
                ev.Target.DeObfuscateScp96s();
                return;
            }

            ProjectSCRAMBLE.SCRAMBLE.ScrambleCharges[serial] -= Time.deltaTime * config.ChargeUsageMultiplayer;

            if (shouldRandomError)
            {
                ev.Target.AddSCRAMBLEHint(translation.Error);
                Timing.CallDelayed(0.5f , () => ev.Target.AddSCRAMBLEHint(translation.Charge.Replace("{charge}", charge.FormatCharge())));
                return;
            }
            
            ev.Target.AddSCRAMBLEHint(translation.Charge.Replace("{charge}", charge.FormatCharge()));
            ev.IsAllowed = false;
        }

        private void OnChangeEffect(ReceivingEffectEventArgs ev)
        {
            if (ev.Effect.GetEffectType() != EffectType.Scp1344 || !ev.Effect.IsEnabled)
                return;

            ev.Player.RemoveSCRAMBLEHint();

            if (!ProjectSCRAMBLE.SCRAMBLE.ActiveScramblePlayers.ContainsKey(ev.Player))
                return;

            ev.Player.DeObfuscateScp96s();
            Log.Debug("Player wear-off Project SCRAMBLE");
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            foreach (GameObject censor in PlayerExtensions.Scp96Censors.Values)
            {
                ev.Player.HideNetworkObject(censor);
            }
        }
    }
}
