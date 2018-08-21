using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StateClient {

    public int FieldSize;
    public int NumberOfPlayer;
  
    public List<ClientUnit> Units = new List<ClientUnit>();
}
