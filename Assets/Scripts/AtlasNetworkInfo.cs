using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController;
using Opsive.Shared.Networking;
using Fusion;

public class AtlasNetworkInfo : NetworkBehaviour, INetworkInfo
{

    public bool HasAuthority()
    {
        return HasStateAuthority;
    }

    public bool IsLocalPlayer()
    {
        return HasInputAuthority;
    }

    public bool IsServer()
    {
        return Runner.IsServer;
    }

    public bool IsServerAuthoritative()
    {
        return true;
    }
}
