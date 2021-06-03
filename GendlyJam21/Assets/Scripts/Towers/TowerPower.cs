using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CGDP2.Utilities;

/// <summary>
/// A tower that consumes power and requires power to function.
/// </summary>
public class TowerPower : ATower
{
    private TowerWeapon m_weapon;
    public TowerWeapon weapon => this.CacheGetComponent(ref m_weapon);

    [SerializeField]
    private float m_powerConsumed = 1.0f;
    public float powerConsumed => m_powerConsumed;

    [SerializeField]
    private SpriteRenderer m_spriteRenderer;
    public SpriteRenderer spriteRenderer => m_spriteRenderer;



    public override void OnCastlePowerChanged(bool _isPowered)
    {
        base.OnCastlePowerChanged(_isPowered);
        spriteRenderer.color = _isPowered ? Color.white : Color.gray;
    }

    public override void OnPlacement()
    {
        base.OnPlacement();
        GameManager.Instance.UpdateGeneration(-powerConsumed);
    }
}
