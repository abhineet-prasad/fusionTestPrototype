using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class PlayerColor : NetworkBehaviour
{
    [Networked]
    public bool spawnedProjecttile { get; set; }

    [SerializeField] MeshRenderer meshRenderer;

    private ChangeDetector _changeDetector;

    public override void Spawned()
    {
        _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
    }


    Color _targetColor;
    public void SetTargetColor(Color color)
    {
        _targetColor = color;
    }

    public Material _material;
    PlayerBehaviour _pb;

    private void Awake()
    {
        _material = GetComponentInChildren<MeshRenderer>().material;
        _pb = GetComponent<PlayerBehaviour>();

    }

    public override void Render()
    {

        foreach(var change in _changeDetector.DetectChanges(this))
        {
            switch(change)
            {
                case nameof(spawnedProjecttile):
                    _material.color = Color.white;
                    
                    break;
            }
        }
       
        _material.color = Color.Lerp(_material.color, _pb.isLocal ? Color.blue: Color.red, Time.deltaTime);

    }


}
