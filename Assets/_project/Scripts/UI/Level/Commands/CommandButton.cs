using System;
using UnityEngine;

namespace FeedTheBaby.UI
{
    public class CommandButton : MonoBehaviour
    {
        public Action AddCommandAction;

        public void ExecuteAddCommand()
        {
            AddCommandAction();
        }

        public void SetCommand(Action addCommandType)
        {
            AddCommandAction = addCommandType;
        }
    }
}