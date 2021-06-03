using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapSingleton : MonoBehaviour
{
    public static TilemapSingleton Instance => V2_Singleton<TilemapSingleton>.instanceElseLogError;
    public static TilemapSingleton/*Nullable*/ InstanceNullable => V2_Singleton<TilemapSingleton>.instanceNullable;



    [SerializeField]
    private Tilemap m_tilemap;
    public Tilemap tilemap => m_tilemap;

    [SerializeField]
    private Grid m_tilemapGrid;
    public Grid tilemapGrid => m_tilemapGrid;


    //[Tooltip(@"The cell position of the player's base that enemies are trying to reach.")]
    //[SerializeField]
    private Vector3Int m_castleCell = new Vector3Int(0, 0, 0);
    public Vector3Int castleCell => m_castleCell;

    //[SerializeField]
    private Vector3Int m_enemySpawnPoint = new Vector3Int(-7, 6, 0);
    public Vector3Int enemySpawnPoint => m_enemySpawnPoint;



    /// <summary>
    /// The only path which leads from the enemy's spawn to the player's base.
    /// Enemies cannot pathfind by themselves (as of 03/06/2021).
    /// </summary>
    private IReadOnlyList<Vector3Int> m_invasionPath;
    public IReadOnlyList<Vector3Int> invasionPath => m_invasionPath;



    private void Awake()
    {
        if (!V2_Singleton<TilemapSingleton>.OnAwake(this, V2_SingletonDuplicateMode.Ignore))
        {
            return;
        }

        m_invasionPath = FindInvasionPath();
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
