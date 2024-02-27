using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Opsive.UltimateCharacterController.Networking.Traits;

public class AtlasNetworkRespawnerMonitor : NetworkBehaviour, INetworkRespawnerMonitor
{
    public void Respawn(Vector3 position, Quaternion rotation, bool transformChange)
    {
        throw new System.NotImplementedException();
    }
}
