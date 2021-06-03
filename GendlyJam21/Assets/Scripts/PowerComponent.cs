using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerComponent : ATower
{
    public float powerEffect;

    public override void OnPlacement()
    {
        base.OnPlacement();
        GameManager.Instance.UpdateGeneration(powerEffect);
    }
}
