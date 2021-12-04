using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// the game controller manages spawning waves, as well as the timer between waves

public class GameController : MonoBehaviour
{
    private enum LevelPhase
    {
        BeginningPhase,
        PlanningPhase,
        WavePhase
    }

    private LevelPhase currentPhase = LevelPhase.BeginningPhase;

    [SerializeField] int maxWaves = 10;
    [SerializeField] float timeBetweenWaves = 60f;
    private float timeToNextWave = 0;

    // the spawning strategy I'm using:
    // each enemy is assigned a spawningcost the game must pay to spawn that enemy
    // totalcost is the sum of all spawningcosts.
    // every spawn interval, the GameController randomly generates a float between 0 and N (exclusively), where N is the number of different enemies
    // we then determine which enemy spawn using the formula i = Mathf.Floor(((n + 1) / (x + 1)) - 1)
    // if we can't afford the chosen enemy, we go to the next most expensive enemy.
    //
    // NOTE: enemy spawning costs must be in ascending order, or else the array will have to be sorted on Start().

    [Header("Enemy Spawn Logic")]
    [SerializeField] GameObject[] spawnableEnemies;
    [SerializeField] int[] spawningCost;

    [SerializeField] int initialBudget = 100;
    [SerializeField] float linearScale = 10;
    [SerializeField] float multiplicativeScale = 1.5f;
    [SerializeField] float spawnFrequency = 1f; //spawns per second
    private float spawnCooldown = 0;
    [SerializeField] float bigEnemyBoostFactor = 1.1f; // this value skews the spawning formula to choose larger enemies more frequently

    [SerializeField] EnemySpawnZone[] spawnZones;

    private int currentWave = 0;
    private int currentBudget = 100;

    // singleton behavior
    private static GameController instance;
    public int activeEnemies = 0;

    public static GameController GetInstance()
    {
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            currentBudget = initialBudget;
            currentWave = 1;
            currentPhase = LevelPhase.WavePhase;
        }
    }

    private void HandlePlanningPhase()
    {
        timeToNextWave -= Time.deltaTime;
        
        if (timeToNextWave <= 0)
        {
            // scale the budget


            currentBudget = initialBudget;
            currentWave = 1;
            currentPhase = LevelPhase.WavePhase;
        }
    }

    private void HandleWavePhase()
    {
        if (currentBudget <= 0)
        {
            if (activeEnemies <= 0)
            {
                currentPhase = LevelPhase.PlanningPhase;
                timeToNextWave = timeBetweenWaves;
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

                for (int i = enemyIndex; i >= 0; i--)
                {
                    if (spawningCost[i] < currentBudget || i == 0)
                    {
                        // randomly pick a spawn zone
                        int zone = Random.Range(0, spawnZones.Length);
                        spawnZones[zone].spawnEnemyRandomly(spawnableEnemies[i]);

                        activeEnemies += 1;
                        currentBudget -= spawningCost[i];
                        i = 0;
                    }
                }

                spawnCooldown = spawnFrequency;
            }
        }
    }
}
