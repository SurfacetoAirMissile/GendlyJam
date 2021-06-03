using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CGDP2.Utilities;

/// <summary>
/// A manager over all components of this Enemy GameObject.
/// </summary>
public class EnemyEntity : MonoBehaviour
{
    private EnemyHealth m_healthComponent;
    public EnemyHealth healthComponent => this.CacheGetComponent(ref m_healthComponent);

    private EnemyMovement m_movementComponent;
    public EnemyMovement movementComponnet => this.CacheGetComponent(ref m_movementComponent);
}
