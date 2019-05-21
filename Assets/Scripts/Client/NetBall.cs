using UnityEngine;

namespace Scripts.Client
{
    public class NetBall
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public GameObject Ball { get; set; }

        public Vector3 Position => new Vector3(X, Y, Z);
    }
}