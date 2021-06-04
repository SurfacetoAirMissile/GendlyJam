using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    // serialized values (defaults)

    [Header("Setup")]
    [SerializeField]
    private float m_spawnRate = default;
    public float spawnRate => m_spawnRate;

    [SerializeField]
    private float m_spawnAcceleration = default;
    public float spawnAcceleration => m_spawnAcceleration;

    [SerializeField]
    private float m_spawnPowerVariation = default;
    public float spawnPowerVariation => m_spawnPowerVariation;

    [SerializeField]
    private float m_spawnDelay = default;
    public float spawnDelay => m_spawnDelay;

    [SerializeField]
    private float m_spawnDelayVariation = default;
    public float spawnDelayVariation => m_spawnDelayVariation;

    // calculated runtime values

    [SerializeField]
    private bool m_spawnerActive = default;
    public bool spawnerActive => m_spawnerActive;

    [SerializeField]
    private float m_spawnDelayRemaining = default;
    public float spawnDelayRemaining => m_spawnDelayRemaining;

    [SerializeField]
    private float m_spawnPower = default;
    public float spawnPower => m_spawnPower;

    [SerializeField]
    private float m_individualDelayRemaining = default;
    public float individualDelayRemaining => m_individualDelayRemaining;

    [SerializeField]
    private int m_currentEnemy = default;
    public int currentEnemy => m_currentEnemy;

    [SerializeField]
    private int m_enemiesToSpawn = default;
    public int enemiesToSpawn => m_enemiesToSpawn;



    [SerializeField]
    private EnemyEntity[] m_enemies = default;
    public EnemyEntity[] enemies => m_enemies;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // speed up the spawning rate
        m_spawnRate += m_spawnAcceleration * Time.deltaTime;

        // charge the spawner's power at the spawn rate
        m_spawnPower += m_spawnRate * Time.deltaTime;

        if (m_spawnerActive)
        {
            m_individualDelayRemaining -= Time.deltaTime;
            if (m_individualDelayRemaining <= 0f)
            {
                // spawn enemy
                Instantiate(enemies[m_currentEnemy]);

                m_enemiesToSpawn--;
                if (m_enemiesToSpawn == 0)
                {
                    m_spawnerActive = false;
                }

                m_individualDelayRemaining = m_enemies[m_currentEnemy].enemySpawnProperties.spawnDelay;
            }
        }
        else
        {
            // bring down the delay
            m_spawnDelayRemaining -= Time.deltaTime;
            if (m_spawnDelayRemaining <= 0f)
            {
                m_spawnerActive = true;
                m_spawnDelayRemaining = m_spawnDelay + m_spawnDelayVariation * Random.Range(-1f, 1f);

                // pick a random enemy to spawn a wave of
                m_currentEnemy = Random.Range(0, m_enemies.Length);

                // convert spawnPower into the enemies to spawn
                m_spawnPower += m_spawnPower * m_spawnPowerVariation * Random.Range(-1f, 1f);
                float weight = m_enemies[m_currentEnemy].enemySpawnProperties.spawnWeight;
                m_enemiesToSpawn = Mathf.FloorToInt(m_spawnPower / weight);
                m_spawnPower -= weight * m_enemiesToSpawn;
                m_individualDelayRemaining = 0f;
            }
        }


    }

    public void Rush()
    {
        if (!m_spawnerActive)
        {
            float m_spawnRateStart = m_spawnRate;
            m_spawnRate += m_spawnAcceleration * m_spawnDelayRemaining;
            float average = (m_spawnRate + m_spawnRateStart) / 2f;
            m_spawnPower += average * m_spawnDelayRemaining;
            m_spawnDelayRemaining = 0f;
        }
    }
}
