using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;


public struct UCCInput : INetworkInput
{

    public float horizontalValue;
    public float verticalValue;
    public float mouseXValue;
    public float mouseYValue;
    public float controllerXValue;
    public float controllerYValue;

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
            case "Mouse X":
                return mouseXValue;
            case "Mouse Y":
                return mouseYValue;
            case "Controller X":
                return controllerXValue;
            case "Controller Y":
                return controllerYValue;
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
            case "Mouse X":
                mouseXValue = value;
                break;
            case "Mouse Y":
                mouseYValue = value;
                break;
            case "Controller X":
                controllerXValue = value;
                break;
            case "Controller Y":
                controllerYValue = value;
                break;
            default:
                break;
        }
    }
}


