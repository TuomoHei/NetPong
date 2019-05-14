using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Code.Shared;

public class ServerPlayer : BasePlayer
{
    public ServerPlayer(string name, byte id) : base(name, id)
    {
    }

    public override void ApplyInput(PlayerInputPacket command, float delta)
    {
        base.ApplyInput(command, delta);
    }

    public override void Spawn(Vector2 position)
    {
        base.Spawn(position);
    }
}
