using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// a collection of information for building towers, used in UI and for determining how much a tower costs
public class TowerBuildProperties : MonoBehaviour
{
    public int buildCost = 10;
    public Sprite icon;

    [Header("Stats Panel Details")]
    public string towerName = "";
    public string speed = "";
    public string power = "";
    public string range = "";
    public string special = "";
}
