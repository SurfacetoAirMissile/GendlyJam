using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private float m_maxHealth = 100;
    public float maxHealth => m_maxHealth;

    private float m_health;
    public float health => maxHealth;

    private void Awake()
    {
        m_health = m_maxHealth;
    }

    public void TakeDamage(float damage)
    {
        m_health -= damage;
        if (m_health <= 0.0f)
        {
            Kill();
        }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }
}
