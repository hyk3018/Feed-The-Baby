using System;
using UnityEngine;

namespace FeedTheBaby
{
    [RequireComponent(typeof(AudioSource))]
    public class CameraSound : MonoBehaviour
    {
        static CameraSound _instance;
        
        public static CameraSound Instance => _instance;

        AudioSource _audioSource;

        void Awake()
        {
            if (_instance == null || _instance != this)
            {
                _instance = this;
                _audioSource = GetComponent<AudioSource>();
            }
        }

        public static void PlaySoundAtCameraPosition(AudioClip audioClip, float volume)
        {
            Instance._audioSource.clip = audioClip;
            Instance._audioSource.Play();
        }

        public static void PlaySoundAtCameraDepth(AudioClip audioClip, Vector3 position, float volume)
        {
            position.z = Instance.transform.position.z;
            AudioSource.PlayClipAtPoint(audioClip, position, volume);
        }
    }
}