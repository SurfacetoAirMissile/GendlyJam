using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileParentSingleton : MonoBehaviour
{
    public Transform projectileParent => this.transform;



    public static ProjectileParentSingleton Instance => V2_Singleton<ProjectileParentSingleton>.instanceElseLogError;
    public static ProjectileParentSingleton/*Nullable*/ InstanceNullable => V2_Singleton<ProjectileParentSingleton>.instanceNullable;

    private void Awake()
    {
        if (!V2_Singleton<ProjectileParentSingleton>.OnAwake(this, V2_SingletonDuplicateMode.Ignore))
        {
            return;
        }
    }
}
