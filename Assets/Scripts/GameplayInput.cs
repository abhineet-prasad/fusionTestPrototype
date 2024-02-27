using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public struct GameplayInput : INetworkInput
{
    public Vector2 MoveDirection;
    public Vector2 LookRotationDelta;
    public NetworkButtons Actions;

    public const int JUMP_BUTTON = 0;
}