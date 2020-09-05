using System;
using System.Collections;
using UnityEngine;

namespace FeedTheBaby.UI
{
    public class TimedShowHide : MonoBehaviour
    {
        Animator _animator;
        static readonly int Show = Animator.StringToHash("Show");
        static readonly int Hide = Animator.StringToHash("Hide");

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        public void ShowForDuration(float duration)
        {
            if (!_animator)
                _animator = GetComponent<Animator>();
            
            _animator.SetTrigger(Show);
            StartCoroutine(HideAfterDuration(duration));
        }

        IEnumerator HideAfterDuration(float duration)
        {
            yield return new WaitForSeconds(duration);
            _animator.SetTrigger(Hide);
        }
    }
}