using System.Collections;
using System.Collections.Generic;
using Opsive.Shared.Input;
using UnityEngine;

public class FusionPlayerProxy : PlayerInputProxy
{
    protected override void Awake()
    {

        GetComponentInChildren<FusionUnityInputSystem>().NetworkInput = GetComponent<FusionUCCInputNetworkBehaviour>();
        base.Awake();
    }
}
