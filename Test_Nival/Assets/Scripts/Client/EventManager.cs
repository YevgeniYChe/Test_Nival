using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager {

    public delegate void StateUpdate();
    public static event StateUpdate Step;

    public void OnStep()
    {
        Step();
    }
}
