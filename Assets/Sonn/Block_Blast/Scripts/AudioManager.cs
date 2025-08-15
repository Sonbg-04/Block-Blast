using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BlockBlast
{
    public class AudioManager : MonoBehaviour, ISingleton
    {
        public static AudioManager Ins;
        public AudioSource musicSource, btnClickSource;

        private void Awake()
        {
            MakeSingleton();
        }
        private void Start()
        {
            PlayMusic(musicSource);
        }
        public void PlayMusic(AudioSource source)
        {
            if (source != null)
            {
                source.Play();
            }
        }
        public void PlaySFX(AudioSource source)
        {
            if (source != null)
            {
                source.PlayOneShot(source.clip);
            }    
        }    
        public void StopMusic(AudioSource source)
        {
            if (source != null)
            {
                source.Stop();
            }
        }
        public void PauseMusic(AudioSource source)
        {
            if (source != null)
            {
                if (source.isPlaying)
                {
                    source.Pause();
                }
            }    
        }
        public void UnPauseMusic(AudioSource source)
        {
            if (source != null)
            {
                if (!source.isPlaying)
                {
                    source.UnPause();
                }
            }
        }
        public void MakeSingleton()
        {
            if (Ins == null)
            {
                Ins = this;
                DontDestroyOnLoad(this);
            }    
            else
            {
                Destroy(gameObject);
            }    
        }
    }
}
