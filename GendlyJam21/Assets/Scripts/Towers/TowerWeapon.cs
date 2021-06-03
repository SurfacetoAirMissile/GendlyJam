using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TowerWeapon : MonoBehaviour
{
    [SerializeField]
    private float m_reloadDuration = 0.3f;
    public float reloadDuration => m_reloadDuration;

    [SerializeField]
    private TowerProjectile m_projectilePrefab;
    public TowerProjectile projectilePrefab => m_projectilePrefab;



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
        var allEnemies = FindObjectsOfType<EnemyEntity>();
        var castlePosition = TilemapSingleton.Instance.castlePosition;
        // If there are no enemies, it is null.
        var enemyClosestToCastle = allEnemies.AnyAsNullable()?.MinBy(e => Vector2.SqrMagnitude((Vector2)e.transform.position - castlePosition));
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
}
