using System.Collections.Generic;
using UnityEngine;

namespace FeedTheBaby.Commands
{
    public class CommandManager : MonoBehaviour
    {
        [SerializeField] int maxCommands = 10;
        List<Command> _commands;

        IMoveCommandExecutor _moveCommandExecutor;

        bool _processingCommand;

        void Awake()
        {
            _commands = new List<Command>(maxCommands);
            _moveCommandExecutor = GetComponent<IMoveCommandExecutor>();
        }

        void Update()
        {
            if (!_processingCommand && _commands.Count > 0)
            {
                _processingCommand = true;
                Execute(GetNextCommand());
            }
        }

        public void AddCommand(Command command)
        {
            _moveCommandExecutor.Interrupt();
            _commands.Add(command);
        }

        Command GetNextCommand()
        {
            if (_commands.Count > 0)
            {
                Command next = _commands[0];
                _commands.RemoveAt(0);
                return next;
            }

            return null;
        }

        void Execute(Command command)
        {
            switch (command)
            {
                case MoveCommand moveCommand:
                    _moveCommandExecutor?.ExecuteMove(moveCommand, OnCommandFinish);
                    break;
            }
        }

        void OnCommandFinish()
        {
            _processingCommand = false;
        }
    }
}