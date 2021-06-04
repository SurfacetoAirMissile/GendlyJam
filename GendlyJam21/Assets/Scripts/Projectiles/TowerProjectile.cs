using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CGDP2.Utilities;

/// <summary>
/// A projectile that seeks the target like a guided missile.
/// </summary>
public class TowerProjectile : MonoBehaviour
{
    [SerializeField]
    private float m_damage = 10.0f;
    public float damage => m_damage;

    [SerializeField]
    private float m_speed = 10.0f;
    public float speed => m_speed;

    [SerializeField]
    private float m_selfDestructTimeRemaining = 3.0f;
    public float selfDestructTimeRemaining => m_selfDestructTimeRemaining;

    [SerializeField]
    private GameObject m_explosion;

    [SerializeField]
    private SoundEffect m_explosionSoundPrefab;

    private Vector2 prevDirection /*normalized*/ = Vector2.zero;

    private bool UpdateSelfDestructTimer()
    {
        m_selfDestructTimeRemaining -= Time.deltaTime;
        if (m_selfDestructTimeRemaining <= 0.0f)
        {
            m_selfDestructTimeRemaining = 0.0f;
            return true;
        }
        return false;
    }

    private EnemyEntity m_target;
    public EnemyEntity target => m_target;



    /// <summary>
    /// Called by <see cref="TowerWeapon.ShootAt(EnemyEntity)"/>.
    /// </summary>
    public void Init(EnemyEntity target)
    {
        m_target = target.AsTrueNullable().ThrowIfNull();
    }

    private void Update()
    {
        if (UpdateSelfDestructTimer())
        {
            Explode();
        }
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (!target)
        {
            WanderWithoutTarget();
            return;
        }

        FollowTarget();
    }

    private void WanderWithoutTarget()
    {
        transform.position += (Vector3)(prevDirection * speed * Time.fixedDeltaTime);
    }

    private void FollowTarget()
    {
        var prevPos = (Vector2)transform.position;
        var targetPos = (Vector2)target.transform.position;
        transform.position = Vector2.MoveTowards(prevPos, targetPos, speed * Time.fixedDeltaTime);
        var newPos = (Vector2)transform.position;

        var displacement = newPos - prevPos;
        prevDirection = displacement.normalized;

        var sqrDistToTarget = Vector2.SqrMagnitude(targetPos - newPos);
        if (sqrDistToTarget < 0.01f)
        {
            target.healthComponent.TakeDamage(damage);
            Explode();
        }
    }

    private void Explode()
    {
        if (m_explosion)
        {
            m_explosion.transform.SetParent(transform.parent);
            m_explosion.SetActive(true);
        }

        CgdUtils.TryElseLog(() =>
        {
            if (m_explosionSoundPrefab)
            {
                SoundEffectsParentSingleton.Instance.PlaySound(m_explosionSoundPrefab, transform.position);
            }
        });

        Destroy(gameObject);
    }
}
