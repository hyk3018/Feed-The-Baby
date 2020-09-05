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
        bool _show;

        void Awake()
        {
            _animator = GetComponent<Animator>();
        }

        void Update()
        {
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
                _show = false;
        }

        public void ShowForDuration(float duration)
        {
            if (!_animator)
                _animator = GetComponent<Animator>();
            
            _show = true;
            _animator.SetTrigger(Show);
            StartCoroutine(HideAfterDuration(duration));
            StartCoroutine(ShowUntilTrigger());
        }

        IEnumerator ShowUntilTrigger()
        {
            while (_show)
                yield return null;
            _animator.SetTrigger(Hide);
        }

        IEnumerator HideAfterDuration(float duration)
        {
            yield return new WaitForSeconds(duration);
            _show = false;
        }
    }
}