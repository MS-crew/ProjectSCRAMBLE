using MEC;
using UnityEngine;
using Exiled.API.Features;
using Exiled.API.Features.Toys;

namespace ProjectSCRAMBLE.Extensions
{
    public static class SchematicExtensions
    {
        public static void AttachToTransform(this Primitive Censor, Transform Head)
        {
            Censor.AdminToyBase.syncInterval = 0;

            Timing.RunCoroutine(Methods.TrackHead(Censor.Transform, Head));
        }

        public static void RemoveForUnGlassesPlayer(this Primitive Censor, Player SchematicOwner)
        {
            foreach (Player normalply in Player.List)
            {
                if (ProjectSCRAMBLE.ActiveScramblePlayers.ContainsKey(normalply))
                {
                    ProjectSCRAMBLE.ActiveScramblePlayers[normalply].Add(SchematicOwner);
                }
                else
                {
                    normalply.DestroyCensor(Censor);
                }
            }
        }
    }
}
