using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace FeedTheBaby.UI
{
    public class Disabler : MonoBehaviour
    {
        [SerializeField] GameObject[] disables = null;

        public virtual void DisableOther()
        {
            foreach (var disable in disables)
                if (disable.GetComponent<IDisableable>() is IDisableable disableable)
                    disableable.Disable();
        }

        public virtual void EnableOther()
        {
            foreach (var disable in disables)
                if (disable.GetComponent<IDisableable>() is IDisableable disableable)
                    disableable.Enable();
        }
    }

    public interface IDisableable
    {
        void Disable();
        void Enable();
    }
}