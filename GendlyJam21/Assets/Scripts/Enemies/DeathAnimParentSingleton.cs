using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathAnimParentSingleton : MonoBehaviour
{
    public Transform theParent => this.transform;



    public static DeathAnimParentSingleton Instance => V2_Singleton<DeathAnimParentSingleton>.instanceElseLogError;
    public static DeathAnimParentSingleton/*Nullable*/ InstanceNullable => V2_Singleton<DeathAnimParentSingleton>.instanceNullable;

    private void Awake()
    {
        if (!V2_Singleton<DeathAnimParentSingleton>.OnAwake(this, V2_SingletonDuplicateMode.Ignore))
        {
            return;
        }
    }
}
