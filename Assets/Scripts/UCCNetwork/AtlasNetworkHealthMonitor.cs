using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Opsive.UltimateCharacterController.Networking.Traits;
using Opsive.UltimateCharacterController.Traits.Damage;

public class AtlasNetworkHealthMonitor : NetworkBehaviour, INetworkHealthMonitor
{
    public void Die(Vector3 position, Vector3 force, GameObject attacker)
    {
        throw new System.NotImplementedException();
    }

    public void Heal(float amount)
    {
        throw new System.NotImplementedException();
    }

    public void OnDamage(float amount, Vector3 position, Vector3 direction, float forceMagnitude, int frames, float radius, IDamageSource source, Collider hitCollider)
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
