using System.Collections;
using System.Collections.Generic;
using Opsive.Shared.Integrations.InputSystem;
using UnityEngine;
using Fusion;
using UnityEngine.InputSystem;

public class FusionUnityInputSystem : UnityInputSystem
{

    public FusionUCCInputNetworkBehaviour NetworkInput
    {
        get { return _networkInput; }
        set
        {
            _networkInput = value;
        }
    }

    private FusionUCCInputNetworkBehaviour _networkInput;

    protected override void Awake()
    {
        //_networkInput = GetComponentInParent<FusionUCCInputNetworkBehaviour>();
        base.Awake();
    }

 


    protected override bool GetButtonInternal(string name)
    {
       
        return false;
    }

    /// <summary>
    /// Internal method which returns true if the button was pressed this frame.
    /// </summary>
    /// <param name="name">The name of the button.</param>
    /// <returns>True if the button is pressed this frame.</returns>
    protected override bool GetButtonDownInternal(string name)
    {
        
        return false;
    }

    /// <summary>
    /// Internal method which returnstrue if the button is up.
    /// </summary>
    /// <param name="name">The name of the button.</param>
    /// <returns>True if the button is up.</returns>
    protected override bool GetButtonUpInternal(string name)
    {
        
        return false;
    }

    /// <summary>
    /// Internal method which returns the value of the axis with the specified name.
    /// </summary>
    /// <param name="name">The name of the axis.</param>
    /// <returns>The value of the axis.</returns>
    protected override float GetAxisInternal(string name)
    {

        return _networkInput.CurrentInput.GetAxisByName(name);
    }

    /// <summary>
    /// Internal method which returns the value of the raw axis with the specified name.
    /// </summary>
    /// <param name="name">The name of the axis.</param>
    /// <returns>The value of the raw axis.</returns>
    protected override float GetAxisRawInternal(string name)
    {
        return GetAxisInternal(name);
    }

    /// <summary>
    /// Returns the position of the mouse.
    /// </summary>
    /// <returns>The mouse position.</returns>
    public override Vector2 GetMousePosition()
    {
        return _networkInput.CurrentInput.mousePosition;
    }


    /// <summary>
    /// Enables or disables gameplay input. An example of when it will not be enabled is when there is a fullscreen UI over the main camera.
    /// </summary>
    /// <param name="enable">True if the input is enabled.</param>
    protected override void EnableGameplayInput(bool enable)
    {
        base.EnableGameplayInput(enable);

        if (enable && m_DisableCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    /// <summary>
    /// Does the game have focus?
    /// </summary>
    /// <param name="hasFocus">True if the game has focus.</param>
    protected override void OnApplicationFocus(bool hasFocus)
    {
        base.OnApplicationFocus(hasFocus);

        if (enabled && hasFocus && m_DisableCursor)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

   
}
