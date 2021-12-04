using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildHUDController : MonoBehaviour
{
    [SerializeField] GameObject buildPanel;

    [SerializeField] GameObject[] towers;
    private GameObject[] buildPanelItems;
    [SerializeField] GameObject buildPanelItemPrefab;
    [SerializeField] Sprite towerUIFrame;
    [SerializeField] Sprite towerUIFrameSelected;

    private bool buildMode = false;
    private int selectedTower = 0;
    private PlayerController userPlayer;

    // Start is called before the first frame update
    void Start()
    {
        buildPanel.SetActive(false);

        // populate the build panel
        buildPanelItems = new GameObject[towers.Length];
        for (int i = 0; i < towers.Length; i++)
        {
            GameObject go = Instantiate(buildPanelItemPrefab, buildPanel.transform);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(64 + (128 * i), 0);

            buildPanelItems[i] = go;
        }
        if (buildPanelItems.Length > 0)
        {
            buildPanelItems[selectedTower].GetComponent<Image>().sprite = towerUIFrameSelected;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            ToggleBuildMode();
        }

        if (buildMode)
        {
            if (Input.GetMouseButton(1) && userPlayer != null)
            {
                // in build mode, when the player right clicks, check if the player is looking at a valid tower node
                // if so, build the selected tower on it
                RaycastHit hit;
                if (Physics.Raycast(userPlayer.gameObject.transform.position, userPlayer.GetCameraLookAt(), out hit, 50))
                {
                    GameObject buildNode = hit.collider.gameObject;

                    if (buildNode.CompareTag("TowerNode"))
                    {
                        GameObject newTower = Instantiate(towers[selectedTower], buildNode.transform.position, Quaternion.identity);
                        Destroy(buildNode);
                    }
                }
            }
        }
    }

    private void ToggleBuildMode()
    {
        if (buildMode)
        {
            buildMode = false;
        }
        else
        {
            buildMode = true;
        }
        buildPanel.SetActive(buildMode);
    }

    public void SetMainPlayer(PlayerController player)
    {
        // this is an extra step and might seem like jumping through hoops,
        // but to make multiplayer easier to implement I'm enforcing that the player needs to register themself
        // as the main player when they start

        userPlayer = player;
    }
}
