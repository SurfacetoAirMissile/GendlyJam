using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSingleton : MonoBehaviour
{
    [SerializeField]
    private Tilemap m_tilemap;
    public Tilemap tilemap => m_tilemap;

    [SerializeField]
    private Grid m_tilemapGrid;
    public Grid tilemapGrid => m_tilemapGrid;

    [SerializeField]
    private Transform m_castlePositionMarker;
    [SerializeField]
    private Transform m_enemySpawnPositionMarker;


    /// <summary>
    /// The cell position of the player's base that enemies are trying to reach.
    /// </summary>
    public Vector2Int castleCell { get; private set; }
    public Vector2 castlePosition { get; private set; }

    public Vector2Int enemySpawnPoint { get; private set; }



    /// <summary>
    /// The only path which leads from the enemy's spawn to the player's base.
    /// Enemies cannot pathfind by themselves (as of 03/06/2021).
    /// </summary>
    private IReadOnlyList<Vector2Int> m_invasionPath;
    public IReadOnlyList<Vector2Int> invasionPath => m_invasionPath;





    public static TilemapSingleton Instance => V2_Singleton<TilemapSingleton>.instanceElseLogError.EnsureInit();
    public static TilemapSingleton/*Nullable*/ InstanceNullable => V2_Singleton<TilemapSingleton>.instanceNullable.AsTrueNullable()?.EnsureInit();

    private bool m_initted = false;
    private TilemapSingleton EnsureInit()
    {
        if (m_initted)
            return this;

        m_initted = true;

        castleCell = (Vector2Int)tilemap.WorldToCell(m_castlePositionMarker.position);
        castlePosition = (Vector2)tilemap.GetCellCenterWorld((Vector3Int)castleCell);
        enemySpawnPoint = (Vector2Int)tilemap.WorldToCell(m_enemySpawnPositionMarker.position);
        m_invasionPath = FindInvasionPath();

        return this;
    }

    private void Awake()
    {
        if (!V2_Singleton<TilemapSingleton>.OnAwake(this, V2_SingletonDuplicateMode.DestroyComponent))
        {
            return;
        }

        EnsureInit();
    }

    private IReadOnlyList<Vector2Int> FindInvasionPath()
    {
        return EnemyPathFinder.FindPath((Vector2Int)enemySpawnPoint, (Vector2Int)castleCell)
            .ToArray();
    }
}
