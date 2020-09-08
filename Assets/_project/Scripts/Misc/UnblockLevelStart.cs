using System;
using System.Collections;
using FeedTheBaby.UI;
using UnityEngine;

namespace FeedTheBaby
{
    public class UnblockLevelStart : MonoBehaviour
    {
        TimedShowHide _timedShowHide;
        bool _hasBlocked;

        void Awake()
        {
            _timedShowHide = GetComponent<TimedShowHide>();
            if (_timedShowHide)
                _timedShowHide.OnHidden += () =>
                {
                    if (!_hasBlocked)
                    {
                        LevelManager.Instance.playing = true;
                        _hasBlocked = true;
                    }
                    
                };
        }
    }
}