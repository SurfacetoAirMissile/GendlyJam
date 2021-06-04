using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    private float m_maxHealth = 100;
    public float maxHealth => m_maxHealth;

    [SerializeField]
    private int m_reward = 50;
    public int reward => m_reward;

    [SerializeField]
    private Slider m_healthBar;
    public Slider healthBar => m_healthBar;

    [SerializeField]
    private GameObject m_deathAnim;

    private void ApplyToHealthBar()
    {
        if (!healthBar)
            return;

        healthBar.value = m_health / m_maxHealth;
    }

    private float m_health;
    public float health => maxHealth;

    private void Awake()
    {
        m_health = m_maxHealth;
        ApplyToHealthBar();
    }

    public void TakeDamage(float damage)
    {
        m_health -= damage;
        ApplyToHealthBar();

        if (m_health <= 0.0f)
        {
            Kill();
        }
    }

    public void Kill()
    {
        m_deathAnim.transform.SetParent(DeathAnimParentSingleton.Instance.theParent);
        m_deathAnim.SetActive(true);
        Destroy(gameObject);
        GameManager.Instance.UpdateCredits(m_reward);
    }
}
