using UnityEngine;

namespace ProjectSCRAMBLE.Components
{
    public enum Axis
    {
        ReverseX,
        X,
        Y,
        Z
    }

    public class Rotator : MonoBehaviour
    {
        private float rotationSpeed;
        private Vector3 _rotationAxis;
        public void Initialize(Axis axis)
        {
            _rotationAxis = axis switch
            {
                Axis.ReverseX => Vector3.left,
                Axis.X => Vector3.right,
                Axis.Y => Vector3.up,
                Axis.Z => Vector3.forward,
                _ => Vector3.zero,
            };
            rotationSpeed = Plugin.Instance.Config.Censorspeed;
        }

        void Update()
        {
            transform.Rotate(_rotationAxis * rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}
