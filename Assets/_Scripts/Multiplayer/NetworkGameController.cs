using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkGameController : NetworkBehaviour
{
    private enum LevelPhase
    {
        BeginningPhase,
        PlanningPhase,
        WavePhase,
        PlayerWin,
        PlayerLose
    }

    private LevelPhase currentPhase = LevelPhase.BeginningPhase;

    [SerializeField] int maxWaves = 10;
    [SerializeField] float timeBetweenWaves = 20f;
    [SerializeField] Text overheadTextUI;
    private float timeToNextWave = 0;
    [SerializeField] GameObject winScreen;
    [SerializeField] Text scoreText;
    [SerializeField] PauseScreenController pauseScreen;

    [Header("Enemy Spawn Logic")]
    [SerializeField] GameObject[] spawnableEnemies;
    [SerializeField] int[] spawningCost;

    [SerializeField] int initialBudget = 100;
    [SerializeField] int linearScale = 10; // flat amount added to budget each round
    [SerializeField] float multiplicativeScale = 1.5f; // multiply budget by this factor each round
    [SerializeField] float spawnFrequency = 1f; //spawns per second
    private float spawnCooldown = 0;
    [SerializeField] float bigEnemyBoostFactor = 1f; // this value skews the spawning formula to choose larger enemies more frequently
    [SerializeField] float boostFactorScaling = 0.05f; // increase to the boost faster after each wave

    [SerializeField] NetEnemySpawnZones[] spawnZones;

    private int currentWave = 0;
    private int currentBudget = 100;

    // singleton behavior
    public static int activeEnemies = 0;

    // Start is called before the first frame update
    void Start()
    {
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentPhase)
        {
            case LevelPhase.BeginningPhase:
                HandleBeginningPhase();
                break;
            case LevelPhase.PlanningPhase:
                HandlePlanningPhase();
                break;
            case LevelPhase.WavePhase:
                HandleWavePhase();
                break;
            default:
                break;
        }
    }

    private void HandleBeginningPhase()
    {
        if (!isServer)
        {
            return;
        }

        overheadTextUI.text = "PRESS R TO START";
        RpcUpdateOverheadText("PRESS R TO START");

        if (Input.GetKeyDown(KeyCode.R))
        {
            currentBudget = initialBudget;
            currentWave = 1;
            currentPhase = LevelPhase.WavePhase;
            overheadTextUI.text = "WAVE " + currentWave + " / " + maxWaves;
            RpcUpdateOverheadText("WAVE " + currentWave + " / " + maxWaves);
        }
    }

    private void HandlePlanningPhase()
    {
        if (!isServer)
        {
            return;
        }

        timeToNextWave -= Time.deltaTime;
        overheadTextUI.text = "NEXT WAVE IN\n" + (int)timeToNextWave;

        if (timeToNextWave <= 0)
        {
            // scale the budget
            initialBudget += linearScale;
            initialBudget = (int)(initialBudget * multiplicativeScale);
            bigEnemyBoostFactor += boostFactorScaling;
            spawnFrequency = spawnFrequency * 0.90f;

            activeEnemies = 0;
            currentBudget = initialBudget;
            currentWave++;

            currentPhase = LevelPhase.WavePhase;
            overheadTextUI.text = "WAVE " + currentWave + " / " + maxWaves;
            RpcUpdateOverheadText("WAVE " + currentWave + " / " + maxWaves);
        }
    }

    private void HandleWavePhase()
    {
        if (!isServer)
        {
            return;
        }

        if (currentBudget <= 0)
        {
            if (activeEnemies <= 0)
            {
                if (currentWave + 1 > maxWaves)
                {
                    currentPhase = LevelPhase.PlayerWin;
                    RpcShowVictoryScreen();
                }
                else
                {
                    currentPhase = LevelPhase.PlanningPhase;
                    timeToNextWave = timeBetweenWaves;
                    RpcUpdateOverheadText("PLANNING NEXT WAVE");
                }
            }
        }
        else
        {
            spawnCooldown -= Time.deltaTime;

            if (spawnCooldown <= 0)
            {
                int n = spawnableEnemies.Length;
                float num = Random.Range(0f, (float)n);
                if (num == 0) { num = 0.00000001f; }

                int enemyIndex = Mathf.FloorToInt(((n + 1) / ((num / bigEnemyBoostFactor) + 1)) - 1);
                if (enemyIndex < 0) enemyIndex = 0;

                CmdSpawnEnemy(enemyIndex);

                spawnCooldown = spawnFrequency;
            }
        }
    }

    [Command]
    void CmdSpawnEnemy(int index)
    {
        for (int i = index; i >= 0; i--)
        {
            if (spawningCost[i] < currentBudget || i == 0)
            {
                // randomly pick a spawn zone
                int zone = Random.Range(0, spawnZones.Length);
                GameObject go = spawnZones[zone].spawnEnemyRandomly(spawnableEnemies[i]);
                NetworkServer.Spawn(go);
                RpcSetPath(go, spawnZones[zone].gameObject);

                activeEnemies += 1;
                currentBudget -= spawningCost[i];
                i = 0;
            }
        }
    }

    [ClientRpc]
    private void RpcSetPath(GameObject go, GameObject spawnZone)
    {
        go.GetComponent<IPathable>().SetPath(spawnZone.GetComponent<NetEnemySpawnZones>().path);
    }

    [ClientRpc]
    void RpcUpdateOverheadText(string text)
    {
        overheadTextUI.text = text;
    }

    [ClientRpc]
    void RpcShowVictoryScreen()
    {
        Time.timeScale = 0f;

        pauseScreen.gameObject.SetActive(false);
        pauseScreen.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        NetworkDefensePoint objective = FindObjectOfType<NetworkDefensePoint>();
        NetworkBankController bank = FindObjectOfType<NetworkBankController>();

        if (objective != null && bank != null)
        {
            scoreText.text = "You achieved a score of: " + (bank.GetBalance() * objective.GetHealth());
        }

        winScreen.SetActive(true);
    }
}
