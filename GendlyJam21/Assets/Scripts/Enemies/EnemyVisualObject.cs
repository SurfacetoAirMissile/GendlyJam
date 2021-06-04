using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handles flipping the sprite horizontally according to velocity.
/// </summary>
public class EnemyVisualObject : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D m_rb;
    public Rigidbody2D rb => m_rb;

    private bool m_isGoingRight;

    private void ApplyGoingRight(bool _isGoingRight)
    {
        m_isGoingRight = _isGoingRight;
        transform.localScale = new Vector3(m_isGoingRight ? 1 : -1, 1, 1);
    }

    private void Start()
    {
        ApplyGoingRight(false);
    }

    private void LateUpdate()
    {
        if (Mathf.Abs(rb.velocity.x) < 0.001f)
            return;

        var newGoingRight = rb.velocity.x > 0.0f;
        if (m_isGoingRight == newGoingRight)
            return;
        ApplyGoingRight(newGoingRight);
    }
}
