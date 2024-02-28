using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public struct UCCInput : INetworkInput
{

    public float horizontalValue;
    public float verticalValue;

    public Vector2 mousePosition;

    public NetworkButtons networkButtons;
    public const int JUMP_BUTTON = 0;


    

    public float GetAxisByName(string axisName)
    {
        switch (axisName)
        {
            case "Horizontal":
                return horizontalValue;
            case "Vertical":
                return verticalValue;
            default:
                return 0.0f;
        }
    }

    public void SetAxisByName(string axisName, float value)
    {
        switch (axisName)
        {
            case "Horizontal":
                horizontalValue = value ;
                break;
            case "Vertical":
                verticalValue = value;
                break;
            default:
                break;
        }
    }
}


