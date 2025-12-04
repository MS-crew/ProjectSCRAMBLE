using System.Collections.Generic;

using MEC;

using UnityEngine;

using PlayerRoles;

using ProjectSCRAMBLE.Extensions;

using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

using ProjectSCRAMBLE.Patchs;

#if PMER
using ProjectMER.Features.Objects;
#endif

using MapEvent = Exiled.Events.Handlers.Map;
using PlayerEvent = Exiled.Events.Handlers.Player;
using ServerEvent = Exiled.Events.Handlers.Server;

using static ProjectSCRAMBLE.Methods;
using static ProjectSCRAMBLE.ProjectSCRAMBLE;
using Exiled.Events.EventArgs.Map;

namespace ProjectSCRAMBLE
{
    public class EventHandlers
    {
        public HashSet<Player> DirtyPlayers { get; set; } = [];
        public HashSet<ushort> DirtyPickupSerials { get; set; } = [];

        public void Subscribe()
        {
            ServerEvent.WaitingForPlayers += OnWaitingforPlayers;

            PlayerEvent.Verified += OnVerified;
            PlayerEvent.Spawned += OnChangedRole; 
            PlayerEvent.ChangingSpectatedPlayer += OnChangingSpectatedPlayer;

            MapEvent.PickupAdded += OnPickupAdded;
            MapEvent.PickupDestroyed += OnPickupDestroyed;
        }

        public void Unsubscribe()
        {
            ServerEvent.WaitingForPlayers -= OnWaitingforPlayers;

            PlayerEvent.Verified -= OnVerified;
            PlayerEvent.Spawned -= OnChangedRole; 
            PlayerEvent.ChangingSpectatedPlayer -= OnChangingSpectatedPlayer;

            MapEvent.PickupAdded -= OnPickupAdded;
            MapEvent.PickupDestroyed -= OnPickupDestroyed;
        }

        private void OnWaitingforPlayers()
        {
            DirtyPlayers.Clear();
            Scp96Censors.Clear();
            DirtyPickupSerials.Clear();

            foreach (HashSet<CoroutineHandle> handles in Coroutines.Values)
            {
                foreach(CoroutineHandle handle in handles)
                {
                    Timing.KillCoroutines(handle);
                }
            }
                
            Coroutines.Clear();
        }

        public void OnVerified(VerifiedEventArgs ev)
        {
            foreach (GameObject censor in Scp96Censors.Values)
            {
                ev.Player.HideNetworkObject(censor);
            }
        }

        private void OnChangedRole(SpawnedEventArgs ev)
        {
            if (SCRAMBLE.ActiveScramblePlayers.Contains(ev.Player))
                ServerUpdateDeactivatingPatch.WearOffProjectScramble(ev.Player.ReferenceHub);

            if (DirtyPlayers.Contains(ev.Player))
            {
                SCRAMBLE.DeObfuscateScp96s(ev.Player);
                DirtyPlayers.Remove(ev.Player);
            }

            if (ev.OldRole == RoleTypeId.Scp096 && ev.Player.Role != RoleTypeId.Scp096)
            {
                RemoveCensor(ev.Player);
                Log.Debug($"Scp96:{ev.Player.Nickname} removed censor");
            }
            else if (ev.Player.Role == RoleTypeId.Scp096)
            {
                Timing.CallDelayed(0.5f, () => AddCensor(ev.Player));
                Log.Debug($"Scp96:{ev.Player.Nickname} added censor");
            }
        }

        private void OnChangingSpectatedPlayer(ChangingSpectatedPlayerEventArgs ev)
        {
            if (ev.OldTarget != null && SCRAMBLE.ActiveScramblePlayers.Contains(ev.OldTarget))
            {
                SCRAMBLE.DeObfuscateScp96s(ev.Player);
                DirtyPlayers.Remove(ev.Player);
            }

            if (ev.NewTarget != null && SCRAMBLE.ActiveScramblePlayers.Contains(ev.NewTarget))
            {
                SCRAMBLE.ObfuscateScp96s(ev.Player);
                DirtyPlayers.Add(ev.Player);
            }
        }

        private void OnPickupAdded(PickupAddedEventArgs ev)
        {
            if (!SCRAMBLE.Check(ev.Pickup))
                return;

            if (DirtyPickupSerials.Contains(ev.Pickup.Serial))
                ev.Pickup.Destroy();

            DirtyPickupSerials.Add(ev.Pickup.Serial);
        }

        private void OnPickupDestroyed(PickupDestroyedEventArgs ev)
        {
            if (DirtyPickupSerials.Contains(ev.Pickup.Serial))
                DirtyPickupSerials.Remove(ev.Pickup.Serial);
        }
    }
}