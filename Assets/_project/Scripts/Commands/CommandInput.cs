using System;
using UnityEngine;

namespace FeedTheBaby.Commands
{
    [RequireComponent(typeof(CommandManager))]
    public class CommandInput : MonoBehaviour
    {
        CommandManager _commandManager;

        void Awake()
        {
            _commandManager = GetComponent<CommandManager>();
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                _commandManager.AddCommand(new MoveCommand(target));
            }
        }
    }
}