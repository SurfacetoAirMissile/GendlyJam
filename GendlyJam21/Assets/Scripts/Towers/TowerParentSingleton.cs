using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerParentSingleton : MonoBehaviour
{
    public Transform theParent => this.transform;



    public static TowerParentSingleton Instance => V2_Singleton<TowerParentSingleton>.instanceElseLogError;
    public static TowerParentSingleton/*Nullable*/ InstanceNullable => V2_Singleton<TowerParentSingleton>.instanceNullable;

    private void Awake()
    {
        if (!V2_Singleton<TowerParentSingleton>.OnAwake(this, V2_SingletonDuplicateMode.Ignore))
        {
            return;
        }
    }
}
