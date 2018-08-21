using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class ClientUnit{

    public int X;
    public int Y;

    public bool IsMoving;
    public bool IsSelect;

    public int TargetX;
    public int TargetY;

    public void Init(ClientUnit source)
    {
        X = source.X;
        Y = source.Y;
        IsMoving = source.IsMoving;
        IsSelect = source.IsSelect;
        TargetX = source.TargetX;
        TargetY = source.TargetY;

    }
}
