using System;
using UnityEngine;

namespace FeedTheBaby
{
    public class CameraSound : MonoBehaviour
    {
        static CameraSound _instance;
        
        public static CameraSound Instance => _instance;

        void Awake()
        {
            if (_instance == null || _instance != this)
            {
                _instance = this;
            }
        }

        public static void PlaySoundAtCameraDepth(AudioClip audioClip, Vector3 position, float volume)
        {
            position.z = Instance.transform.position.z;
            AudioSource.PlayClipAtPoint(audioClip, position, volume);
        }
    }
}