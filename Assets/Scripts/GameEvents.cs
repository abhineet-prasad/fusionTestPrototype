using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvents 
{
    public static Action<GameObject, bool> OnMoving = delegate { };
}
