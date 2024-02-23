using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public enum MyButtons
{
    Forward = 0,
    Backward = 1,
    Left = 2,
    Right = 3,
    Jump = 4
}


public struct NetworkInputData : INetworkInput
{
    public const byte MOUSEBUTTON0 = 1;
    public const byte MOUSEBUTTON1 = 2;

    public NetworkButtons buttons;
    public Vector3 direction;
}
