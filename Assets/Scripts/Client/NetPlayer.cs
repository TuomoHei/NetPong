using UnityEngine;

namespace Scripts.Client
{
    public class NetPlayer
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        public GameObject GameObject { get; set; }
        public bool isGameObjectAdded { get; set; }

        public NetPlayer()
        {
            isGameObjectAdded = false;
        }

        public Vector3 Position => new Vector3(X, Y, Z);
    }
}