using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FeedTheBaby.UI
{
    public class DisableablePanel : MonoBehaviour, IDisableable
    {
        protected List<Button> _buttons;

        protected virtual void Awake()
        {
            _buttons = gameObject.GetComponentsInChildren<Button>().ToList();
        }

        public virtual void Disable()
        {
            if (_buttons != null)
                foreach (var button in _buttons)
                    button.enabled = false;
        }

        public virtual void Enable()
        {
            if (_buttons != null)
                foreach (var button in _buttons)
                    button.enabled = true;
        }
    }
}