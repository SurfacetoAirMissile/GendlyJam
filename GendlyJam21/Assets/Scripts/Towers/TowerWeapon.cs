using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CGDP2.Utilities;

public class TowerWeapon : MonoBehaviour
{
    [SerializeField]
    private float m_reloadDuration = 0.3f;
    public float reloadDuration => m_reloadDuration;

    [SerializeField]
    private TowerProjectile m_projectilePrefab;
    public TowerProjectile projectilePrefab => m_projectilePrefab;



    private TowerPower m_towerPower;
    public TowerPower towerPower => this.CacheGetComponent(ref m_towerPower);



    private float m_reloadTimeRemaining = 0.0f;
    public float reloadTimeRemaining => m_reloadTimeRemaining;

    private bool UpdateReloadTimeRemaining()
    {
        m_reloadTimeRemaining -= Time.deltaTime;
        if (m_reloadTimeRemaining <= 0.0f)
        {
            m_reloadTimeRemaining = 0.0f;
            return true;
        }
        return false;
    }

    private void Reload()
    {
        m_reloadTimeRemaining = reloadDuration;
    }

    private void Update()
    {
        CleanEnemiesInRange();

        if (!towerPower.isPlaced)
            return;
        if (!towerPower.isPowered)
            return;

        if (UpdateReloadTimeRemaining())
        {
            if (Shoot())
            {
                Reload();
            }
        }
    }

    /// <returns>true if it did shoot</returns>
    private bool Shoot()
    {
        var castlePosition = TilemapSingleton.Instance.castlePosition;
        // If there are no enemies, it is null.
        var enemyClosestToCastle = m_enemiesInRange.AnyAsNullable()?.MinBy(e => Vector2.SqrMagnitude((Vector2)e.transform.position - castlePosition));
        if (!enemyClosestToCastle)
        {
            return false;
        }
        return ShootAt(enemyClosestToCastle);
    }

    /// <returns>true if it did shoot</returns>
    private bool ShootAt(EnemyEntity enemy)
    {
        var bullet = Instantiate(
            projectilePrefab,
            transform.position,
            Quaternion.identity,
            ProjectileParentSingleton.Instance.projectileParent
            );
        bullet.Init(enemy);
        return true;
    }

    // Assumes each enemy has only one collider that can trigger this tower's attack range trigger collider.
    private HashSet<EnemyEntity> m_enemiesInRange = new HashSet<EnemyEntity>();

    private void CleanEnemiesInRange()
    {
        m_enemiesInRange.RemoveWhere(e => !e);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var enemy = collision.GetComponentInParent<EnemyEntity>();
        if (!enemy)
            return;
        m_enemiesInRange.Add(enemy);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var enemy = collision.GetComponentInParent<EnemyEntity>();
        if (!enemy)
            return;
        m_enemiesInRange.Remove(enemy);
    }
}
