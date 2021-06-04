using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawnProperties : MonoBehaviour
{

    [SerializeField]
    private float m_spawnWeight = default;
    public float spawnWeight => m_spawnWeight;

    [SerializeField]
    private float m_spawnDelay = default;
    public float spawnDelay => m_spawnDelay;
}
