using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShortEffect : AnimFinishedListener
{
    public override void OnAnimFinished()
    {
        Destroy(gameObject);
    }
}
