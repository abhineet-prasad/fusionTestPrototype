using System.Collections;
using System.Collections.Generic;
using Opsive.Shared.Events;
using Opsive.UltimateCharacterController.Character;
using UnityEngine;

public class TestPrefabBehaviour : MonoBehaviour
{

    public bool fireEvent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (fireEvent)
        {
            fireEvent = false;
            EventHandler.ExecuteEvent<bool>(gameObject, "OnTestEvent", true);
        }
    }

    
}
