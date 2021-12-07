using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerDetailsPanel : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text powerText;
    [SerializeField] Text speedText;
    [SerializeField] Text rangeText;
    [SerializeField] Text specialText;

    public void UpdateTowerDetails(TowerBuildProperties tbp)
    {
        nameText.text = tbp.towerName;
        powerText.text = "Power: " + tbp.power;
        speedText.text = "Speed: " + tbp.speed;
        rangeText.text = "Range: " + tbp.range;
        specialText.text = tbp.special;
    }
}
