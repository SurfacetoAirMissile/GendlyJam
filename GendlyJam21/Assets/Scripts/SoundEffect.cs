using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CGDP2.Utilities;

[RequireComponent(typeof(AudioSource))]
public class SoundEffect : MonoBehaviour
{
    private AudioSource m_audioSource;
    public AudioSource audioSource => this.CacheGetComponent(ref m_audioSource);

    private void Update()
    {
        if (!audioSource.isPlaying)
        {
            Destroy(gameObject);
        }
    }

    public void OnNoClip()
    {
        Destroy(gameObject);
    }
}
