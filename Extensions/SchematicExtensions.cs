using MEC;
using UnityEngine;
using Exiled.API.Features;
using ProjectMER.Features.Objects;

namespace ProjectSCRAMBLE.Extensions
{
    public static class SchematicExtensions
    {
        public static void AttachToTransform(this SchematicObject Censor, Transform Head)
        {
            Censor.AdminToyBases[0].syncInterval = 0;

            Timing.RunCoroutine(Methods.TrackHead(Censor.transform, Head));
        }

        public static void RemoveForUnGlassesPlayer(this SchematicObject Schematic, Player SchematicOwner)
        {
            foreach (Player normalply in Player.List)
            {
                if (ProjectSCRAMBLE.ActiveScramblePlayers.ContainsKey(normalply))
                {
                    ProjectSCRAMBLE.ActiveScramblePlayers[normalply].Add(SchematicOwner);
                }
                else
                {
                    normalply.DestroySchematic(Schematic);
                }
            }
        }
    }
}
