using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using Fusion.Addons.Physics;
using UnityEngine.InputSystem;

public class NetworkManager : MonoBehaviour, INetworkRunnerCallbacks
{
    JengaInput inputAsset;
    
    public PlayerInput playerInput;


    public Fusion.GameMode gameMode;
    NetworkRunner _runner;

    [SerializeField] NetworkPrefabRef _playerPrefab;
    [SerializeField] NetworkPrefabRef _atlasPrefab;
    private Dictionary<PlayerRef, NetworkObject> _spawnedCharacters = new Dictionary<PlayerRef, NetworkObject>();

    public Color[] playerColors;

    void Start()
    {
#if UNITY_SERVER
    gameMode = Fusion.GameMode.Server;
#elif UNITY_EDITOR
        gameMode = Fusion.GameMode.Host;
#else
            gameMode = Fusion.GameMode.Client;
#endif
        //if (Application.isBatchMode)
        //{
        //    gameMode = GameMode.Server;
        //}
        //else
        //{
        //    gameMode = GameMode.Client;
        //}
        inputAsset = new JengaInput();
       
        inputAsset.RoamingModeInput.Enable();
        
        StartGame(gameMode);
    }

    async void StartGame(Fusion.GameMode mode)
    {
        // Create the Fusion runner and let it know that we will be providing user input
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        // Create the NetworkSceneInfo from the current scene
        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
        var sceneInfo = new NetworkSceneInfo();
        if (scene.IsValid)
        {
            sceneInfo.AddSceneRef(scene, LoadSceneMode.Additive);
        }
        //Required to simulate photon physics
        gameObject.AddComponent<RunnerSimulatePhysics3D>();

        // Start or join (depends on gamemode) a session with a specific name
        await _runner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "TestRoom",
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }


    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsServer)
        {
            // Create a unique position for the player
            Vector3 spawnPosition = new Vector3((player.RawEncoded % runner.Config.Simulation.PlayerCount) * 3, 1, 0);
            NetworkObject networkPlayerObject = runner.Spawn(_playerPrefab, spawnPosition, Quaternion.identity, player);
            //NetworkObject networkPlayerObject = runner.Spawn(_atlasPrefab, spawnPosition, Quaternion.identity, player);
            // Keep track of the player avatars for easy access
            _spawnedCharacters.Add(player, networkPlayerObject);
            runner.SetPlayerObject(player, networkPlayerObject);
        }
        
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (_spawnedCharacters.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            _spawnedCharacters.Remove(player);
        }
    }


    private bool _mouseButton0;
    private bool _mouseButton1;
    //void Update()
    //{
    //    _mouseButton0 = _mouseButton0 | Input.GetMouseButton(0);
    //    _mouseButton1 = _mouseButton1 | Input.GetMouseButton(1);
    //}

    //public void OnInput(NetworkRunner runner, NetworkInput input)
    //{
    //    var data = new NetworkInputData();

    //    if (Input.GetKey(KeyCode.W))
    //        data.direction += Vector3.forward;

    //    if (Input.GetKey(KeyCode.S))
    //        data.direction += Vector3.back;

    //    if (Input.GetKey(KeyCode.A))
    //        data.direction += Vector3.left;

    //    if (Input.GetKey(KeyCode.D))
    //        data.direction += Vector3.right;

    //    data.buttons.Set(NetworkInputData.MOUSEBUTTON0, _mouseButton0);
    //    _mouseButton0 = false;

    //    data.buttons.Set(NetworkInputData.MOUSEBUTTON1, _mouseButton1);
    //    _mouseButton1 = false;

    //    input.Set(data);
    //}

    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        var actionMap = inputAsset.RoamingModeInput;
        //var actionMap = testInputAsset.gameplay;
        data.direction = new Vector3(actionMap.Horizontal.ReadValue<float>(), 0, actionMap.Vertical.ReadValue<float>());
        data.buttons.Set(NetworkInputData.MOUSEBUTTON0, actionMap.Fire1.IsPressed());
        data.buttons.Set(NetworkInputData.MOUSEBUTTON1, actionMap.Fire2.IsPressed());
       // Debug.Log(data.direction);

        input.Set(data);
    }

    public void OnConnectedToServer(NetworkRunner runner)
    {
        Debug.Log("connected to server " + runner.GetInstanceID());
    }


    #region unusedCallbacks
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {
        throw new NotImplementedException();
    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }

    

    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
    {

    }

   

    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
    {

    }

    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }

   

    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {
       
    }
    #endregion


}
