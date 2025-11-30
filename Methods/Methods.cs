using UnityEngine;
using System.Collections.Generic;

namespace ProjectSCRAMBLE
{
    public class Methods
    {
        internal static IEnumerator<float> TrackHead(Transform censor, Transform head, float syncinterval)
        {
            while (censor != null && head != null)
            {
                censor.position = head.position;

                yield return syncinterval;
            }
        }

        internal static IEnumerator<float> RotateRandom(Transform tr)
        {
            float t = 0f;

            while (tr != null)
            {
                t += 0.1f;

                float t1 = t * 1.1f;
                float t2 = t * 1.7f;
                float t3 = t * 2.3f;

                float x = Mathf.Sin(t1) * 180f;
                float y = Mathf.Cos(t2) * 180f;
                float z = Mathf.Sin(t3) * 180f;

                tr.rotation = Quaternion.Euler(x, y, z);

                if (t >= 1000f)
                    t = 0f;

                yield return 0.1f;
            }
        }
    }
}
