using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CGDP2.Utilities;

public class EnemyMovement : MonoBehaviour
{
    private TilemapSingleton m_tilemapSingleton;
    public TilemapSingleton tilemapSingleton => this.CacheFetchComponent(ref m_tilemapSingleton,
        () => TilemapSingleton.Instance);

    private Rigidbody2D m_rb;
    public Rigidbody2D rb => this.CacheGetComponent(ref m_rb);



    [SerializeField]
    private float m_speed = 1.0f;
    public float speed => m_speed;




    /// <summary>
    /// Index into the <see cref="TilemapSingleton.invasionPath"/> list.
    /// Cannot be called before this script's <see cref="Start"/> event.
    /// </summary>
    public int currentInvasionPathIndex => m_currentInvasionPathIndex;
    private int m_currentInvasionPathIndex = -1;



    private void Start()
    {
        m_currentInvasionPathIndex = 0;

        var pos = (Vector2)tilemapSingleton.tilemap.GetCellCenterWorld(tilemapSingleton.enemySpawnPoint);
        transform.position = pos.WithZ(transform.position.z);
    }



    private void FixedUpdate()
    {
        var nextTileCell = tilemapSingleton.invasionPath[currentInvasionPathIndex];
        var nextTilePos = (Vector2)tilemapSingleton.tilemap.GetCellCenterWorld(nextTileCell);
        var movementDistance = this.speed * Time.fixedDeltaTime;
        var finalPos = Vector2.MoveTowards(transform.position, nextTilePos, movementDistance);
        bool arrived = (finalPos - nextTilePos).sqrMagnitude < 0.01f;
        rb.MovePosition(arrived ? nextTilePos : finalPos);
        // Assumes this enemy cannot move a distance greater than one tile per update tick.
        transform.position = finalPos.WithZ(transform.position.z);

        if (!arrived)
            return;

        ++m_currentInvasionPathIndex;
        if (m_currentInvasionPathIndex == tilemapSingleton.invasionPath.Count)
        {
            ReachedCastle();
        }
    }

    private void ReachedCastle()
    {
        Debug.LogError("TODO: enemy reached the castle!");
        Destroy(gameObject);
    }
}
