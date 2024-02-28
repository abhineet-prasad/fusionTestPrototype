using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Opsive.Shared.Events;

public class Spawner : MonoBehaviour
{
    public GameObject localPlayerPrefab;
    public GameObject remotePlayerPrefab;


    private GameObject m_GameObject;

    public bool spawn = false;
    int spawnCount = 0;
    private void Update()
    {
        if (spawn)
        {
            spawn = false;
            GameObject prefab = spawnCount == 0 ? localPlayerPrefab : remotePlayerPrefab;
            Spawn(localPlayerPrefab);
            spawnCount++;
        }
    }

    public void Spawn(GameObject prefab)
    {
        m_GameObject =  GameObject.Instantiate(prefab, Vector3.zero,Quaternion.identity);
        m_GameObject.GetComponent<AtlasFusionBehaviour>().Initialize(spawnCount == 0);
        EventHandler.RegisterEvent<bool>(m_GameObject, "OnTestEvent", TestEvent);
    }

    private void OnDestroy()
    {
        EventHandler.UnregisterEvent<bool>(m_GameObject, "OnTestEvent", TestEvent);
    }

    void TestEvent(bool executeEvent)
    {
        Debug.Log("On test event called");
    }
}
