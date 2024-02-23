using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.UltimateCharacterController.Character;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.TextCore.Text;
using Opsive.UltimateCharacterController.Camera;

public class NetworkCharacterLocomotionHandler : UltimateCharacterLocomotionHandler, INetworkRunnerCallbacks
{
    NetworkRunner runner;
    protected override void Awake()
    {
        base.Awake();
        runner = FindObjectOfType<NetworkRunner>();
        runner.AddCallbacks(this);
        

    }

   

    float _hm = 0f;
    float _fm = 0f;

    public void SetHorizontalInput(float hm, float fm)
    {
        _hm = hm;
        _fm = fm;
    }

    public override void GetPositionInput(out float horizontalMovement, out float forwardMovement)
    {
        horizontalMovement = _hm;
        forwardMovement = _fm;
        //base.GetPositionInput(out horizontalMovement, out forwardMovement);
       
    }

    public override void GetRotationInput(float horizontalMovement, float forwardMovement, out float deltaYawRotation)
    {
        base.GetRotationInput(horizontalMovement, forwardMovement, out deltaYawRotation);
    }


    void AttachLookSource()
    {

        if (gameObject.GetComponent<LocalLookSource>() == null)
        {
            gameObject.AddComponent<LocalLookSource>();
        }
    }

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {

    }


    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { Debug.Log("player joined called from NCH"); }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }

}
