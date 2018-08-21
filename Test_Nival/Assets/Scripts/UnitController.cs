using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnitController : MonoBehaviour {

    public ClientUnit UnitProperty = new ClientUnit();
    public int FieldSize;
    public bool ReadyToStep = true;

    [SerializeField]
    private GameObject _marker;


    void Start()
    {
        SelectPers.AddUnit(this);
        ReadyToStep = true;
    }

    public void GetTarget(GameObject tile)
    {
        if (UnitProperty.IsSelect)
        {
            var tileComponent = tile.GetComponent<TileClient>();
            if (tileComponent != null)
            {
                UnitProperty.TargetX = tileComponent.X;
                UnitProperty.TargetY = tileComponent.Y;
            }
            else
            {
                UnitProperty.TargetX = UnitProperty.X;
                UnitProperty.TargetY = UnitProperty.Y;
            }
        }
    }

    public void Deselect()
    {
        if (_marker) _marker.SetActive(false);
        UnitProperty.IsSelect = false;
    }

    public void Select()
    {
        if (_marker) _marker.SetActive(true);
        UnitProperty.IsSelect = true;
    }

}
