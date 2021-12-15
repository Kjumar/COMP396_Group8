using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildHUDController : MonoBehaviour
{
    [SerializeField] GameObject buildPanel;

    [SerializeField] GameObject[] towers;
    [SerializeField] GameObject previewHologram;
    private GameObject[] buildPanelItems;

    [Header("UI Objects")]
    [SerializeField] GameObject buildPanelItemPrefab;
    [SerializeField] Sprite towerUIFrame;
    [SerializeField] Sprite towerUIFrameSelected;
    [SerializeField] TowerDetailsPanel detailsPanel;
    private RectTransform uiHighlightFrame;

    public bool buildMode = false;
    private int selectedTower = 0;
    private PlayerController userPlayer;

    // Start is called before the first frame update
    void Start()
    {
        buildPanel.SetActive(false);
        detailsPanel.gameObject.SetActive(false);
        previewHologram.SetActive(false); // hide the preview hologram

        // populate the build panel
        buildPanelItems = new GameObject[towers.Length];
        for (int i = 0; i < towers.Length; i++)
        {
            GameObject go = Instantiate(buildPanelItemPrefab, buildPanel.transform);
            go.GetComponent<RectTransform>().anchoredPosition = new Vector2(64 + (128 * i), 0);

            buildPanelItems[i] = go;

            TowerBuildProperties bp = towers[i].GetComponent<TowerBuildProperties>();
            if (bp != null)
                go.GetComponent<Image>().sprite = bp.icon;
            else
                go.GetComponent<Image>().sprite = towerUIFrame;
        }
        // lastly,  instantiate the panel highlight (so it overlays the normal icons)
        if (buildPanelItems.Length > 0)
        {
            GameObject go = Instantiate(buildPanelItemPrefab, buildPanel.transform);
            go.GetComponent<Image>().sprite = towerUIFrameSelected;
            uiHighlightFrame = go.GetComponent<RectTransform>();
            uiHighlightFrame.anchoredPosition = new Vector2(64 + (128 * selectedTower), 0);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && userPlayer != null)
        {
            ToggleBuildMode();
        }

        if (buildMode)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                ScrollThroughTowers(1);
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                ScrollThroughTowers(-1);
            }
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
                        if (BankController.instance != null)
                        {
                            // check if player has enough gold in the bank
                            TowerBuildProperties buildReqs = towers[selectedTower].GetComponent<TowerBuildProperties>();
                            if (BankController.instance.Pay(buildReqs.buildCost))
                            {
                                GameObject newTower = Instantiate(towers[selectedTower], buildNode.transform.position, Quaternion.identity);
                                Destroy(buildNode);
                            }
                        }
                    }
                }
            }
        }
    }

    // build preview
    private void FixedUpdate()
    {
        if (buildMode)
        {
            RaycastHit hit;
            if (Physics.Raycast(userPlayer.gameObject.transform.position, userPlayer.GetCameraLookAt(), out hit, 50))
            {
                GameObject buildNode = hit.collider.gameObject;

                if (buildNode.CompareTag("TowerNode"))
                {
                    previewHologram.SetActive(true);
                    previewHologram.transform.position = buildNode.transform.position;
                }
                else
                {
                    previewHologram.SetActive(false);
                }
            }
            else
            {
                previewHologram.SetActive(false);
            }
        }
        else
        {
            previewHologram.SetActive(false);
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
        detailsPanel.gameObject.SetActive(buildMode);
    }

    private void ScrollThroughTowers(int step)
    {
        // changes the current tower selection, while also updating the frames
        selectedTower += step;
        if (selectedTower < 0) selectedTower = -selectedTower;
        selectedTower = selectedTower % towers.Length;
        uiHighlightFrame.anchoredPosition = new Vector2(64 + (128 * selectedTower), 0);
        detailsPanel.UpdateTowerDetails(towers[selectedTower].GetComponent<TowerBuildProperties>());
    }

    public void SetMainPlayer(PlayerController player)
    {
        // this is an extra step and might seem like jumping through hoops,
        // but to make multiplayer easier to implement I'm enforcing that the player needs to register themself
        // as the main player when they start

        userPlayer = player;
    }

}
