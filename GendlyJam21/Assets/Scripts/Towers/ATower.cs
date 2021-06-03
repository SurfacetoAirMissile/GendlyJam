using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ATower : MonoBehaviour
{
    public virtual void OnPlacement()
    {
        m_Instances.Add(this);
        m_isPlaced = true;
    }

    public virtual void OnCastlePowerChanged(bool _isPowered)
    {
        m_isPowered = _isPowered;
    }

    protected virtual void OnDestroy()
    {
        m_isPowered = false;
        m_isPlaced = false;
        m_Instances.Remove(this);
    }

    private bool m_isPlaced = false;
    public bool isPlaced => m_isPlaced;

    private bool m_isPowered = false;
    public bool isPowered => m_isPowered;

    /// <summary>
    /// Collection of all the towers that are placed (not held).
    /// </summary>
    public static IReadOnlyCollection<ATower> Instances => m_Instances;
    private static HashSet<ATower> m_Instances = new HashSet<ATower>();
}
