using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocationPallete", menuName = "Configs/LocationPallete")]
public class LocationPallete : ScriptableObject {

    public string Name = "Name";

    public float CellRadius;

    public int CellSides = 4;

    public List<PoolObject> Cells = new List<PoolObject>();

    public PoolObject GroupObject;
}
