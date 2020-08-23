using System;
using System.Collections.Generic;
using UnityEngine;

namespace FeedTheBaby.Commands
{
    public class CommandManager : MonoBehaviour
    {
        [SerializeField] int maxCommands = 10;
        List<Command> _commands;

        IMoveCommandExecutor _moveCommandExecutor;
        InteractersManager _interactersManager;
        
        bool _processingCommand;

        void Awake()
        {
            _commands = new List<Command>(maxCommands);
            _moveCommandExecutor = GetComponent<IMoveCommandExecutor>();
            _interactersManager = GetComponent<InteractersManager>();
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
            // _moveCommandExecutor.Interrupt();
            _processingCommand = false;
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
                case MoveTransformCommand moveCommand:
                    _moveCommandExecutor?.ExecuteMoveTransform(moveCommand, OnCommandFinish);
                    break;
                case MovePositionCommand movePositionCommand:
                    _moveCommandExecutor?.ExecuteMovePosition(movePositionCommand, OnCommandFinish);
                    break;
                case MoveAndInteractCommand moveAndInteractCommand:
                    _moveCommandExecutor.ExecuteMoveTransform(new MoveTransformCommand(moveAndInteractCommand.target),
                        () =>
                        {
                            Debug.Log("interacting bro");
                            moveAndInteractCommand.interactable.Interact(gameObject, OnCommandFinish);
                            OnCommandFinish();
                        });
                    break;
            }
        }

        void OnCommandFinish()
        {
            _processingCommand = false;
        }
    }

    class InteractersManager : MonoBehaviour, IInteracter
    {
        [SerializeField] List<IInteracter> _interacters = null;

        void Awake()
        {
            var interacters = GetComponentsInChildren<IInteracter>();
            foreach (IInteracter interacter in interacters)
            {
                if (!_interacters.Contains(interacter))
                {
                    _interacters.Add(interacter);
                }
            }
        }

        public void Interact(GameObject interactable, Func<bool> interactionEnd)
        {
            foreach (IInteracter interacter in _interacters)
            {
                interacter.Interact(interactable, interactionEnd);
            }
        }
    }

    public interface IInteracter
    {
        void Interact(GameObject interactable, Func<bool> interactionEnd);
    }
}