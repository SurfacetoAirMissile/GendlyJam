using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectsParentSingleton : MonoBehaviour
{
    public Transform theParent => this.transform;

    public SoundEffect PlaySound(SoundEffect prefab, Vector3 position)
    {
        return Instantiate(prefab, position, Quaternion.identity, theParent);
    }



    public static SoundEffectsParentSingleton Instance => V2_Singleton<SoundEffectsParentSingleton>.instanceElseLogError;
    public static SoundEffectsParentSingleton/*Nullable*/ InstanceNullable => V2_Singleton<SoundEffectsParentSingleton>.instanceNullable;

    private void Awake()
    {
        if (!V2_Singleton<SoundEffectsParentSingleton>.OnAwake(this, V2_SingletonDuplicateMode.Ignore))
        {
            return;
        }
    }
}
