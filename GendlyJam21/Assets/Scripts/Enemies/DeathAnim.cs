using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAnim : AnimFinishedListener
{
    public override void OnAnimFinished()
    {
        Destroy(gameObject);
    }
}
