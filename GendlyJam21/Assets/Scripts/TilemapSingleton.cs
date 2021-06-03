using System.Collections;
using System.Collections.Generic;
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
    public Vector3Int castleCell { get; private set; }
    public Vector2 castlePosition { get; private set; }

    public Vector3Int enemySpawnPoint { get; private set; }



    /// <summary>
    /// The only path which leads from the enemy's spawn to the player's base.
    /// Enemies cannot pathfind by themselves (as of 03/06/2021).
    /// </summary>
    private IReadOnlyList<Vector3Int> m_invasionPath;
    public IReadOnlyList<Vector3Int> invasionPath => m_invasionPath;





    public static TilemapSingleton Instance => V2_Singleton<TilemapSingleton>.instanceElseLogError;
    public static TilemapSingleton/*Nullable*/ InstanceNullable => V2_Singleton<TilemapSingleton>.instanceNullable;

    private void Awake()
    {
        if (!V2_Singleton<TilemapSingleton>.OnAwake(this, V2_SingletonDuplicateMode.DestroyComponent))
        {
            return;
        }

        castleCell = tilemap.WorldToCell(m_castlePositionMarker.position);
        castlePosition = (Vector2)tilemap.GetCellCenterWorld(castleCell);
        m_invasionPath = FindInvasionPath();
        enemySpawnPoint = tilemap.WorldToCell(m_enemySpawnPositionMarker.position);
    }

    private IReadOnlyList<Vector3Int> FindInvasionPath()
    {
        // This is hard coded.
        // TODO use pathfinding.
        return new Vector3Int[]
        {
            new Vector3Int(-7, 6, 0),
            new Vector3Int(-7, 5, 0),
            new Vector3Int(-7, 4, 0),
            new Vector3Int(-7, 3, 0),
            new Vector3Int(-7, 2, 0),
            new Vector3Int(-6, 2, 0),
            new Vector3Int(-5, 2, 0),
            new Vector3Int(-5, 3, 0),
            new Vector3Int(-4, 3, 0),
            new Vector3Int(-3, 3, 0),
            new Vector3Int(-2, 3, 0),
            new Vector3Int(-1, 3, 0),
            new Vector3Int(0, 3, 0),
            new Vector3Int(0, 2, 0),
            new Vector3Int(0, 1, 0),
            new Vector3Int(0, 0, 0),
        };
    }
}
