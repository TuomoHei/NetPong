using UnityEngine;

namespace Code.Shared
{
    public abstract class BasePlayer
    {
        public readonly string Name;

        private float _speed = 3f;
        //private BasePlayerManager _playerManager;

        protected Vector2 _position;

        public const float Radius = 0.5f;
        public Vector2 Position => _position;
        public readonly byte Id;
        public int Ping;

        protected BasePlayer(/*BasePlayerManager playerManager,*/ string name, byte id)
        {
            Id = id;
            Name = name;
            //_playerManager = playerManager;
        }

        public virtual void Spawn(Vector2 position)
        {
            _position = position;
        }

        public virtual void ApplyInput(PlayerInputPacket command, float delta)
        {
            Vector2 velocity = Vector2.zero;

            if ((command.Keys & MovementKeys.Up) != 0)
                velocity.y = -1f;
            if ((command.Keys & MovementKeys.Down) != 0)
                velocity.y = 1f;

            _position += velocity.normalized * _speed * delta;

        }
    }
}