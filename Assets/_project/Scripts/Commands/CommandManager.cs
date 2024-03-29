﻿using System.Collections.Generic;
using UnityEngine;

namespace FeedTheBaby.Commands
{
    public class CommandManager : MonoBehaviour
    {
        [SerializeField] int maxCommands = 10;
        List<Command> _commands;

        IMoveCommandExecutor _moveCommandExecutor;
        InteractersManager _interactersManager;
        IPlantCommandExecutor _plantCommandExecutor;
        
        bool _processingCommand;

        void Awake()
        {
            _commands = new List<Command>(maxCommands);
            _moveCommandExecutor = GetComponent<IMoveCommandExecutor>();
            _interactersManager = GetComponent<InteractersManager>();
            _plantCommandExecutor = GetComponent<IPlantCommandExecutor>();
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
            if (_commands.Count < 2)
            {
                _moveCommandExecutor.Interrupt();
                _commands.Add(command);
            }
        }

        Command GetNextCommand()
        {
            if (_commands.Count > 0)
            {
                Command next = _commands[0];
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
                case PlantCommand plantCommand:
                    _moveCommandExecutor?.ExecuteMovePosition(new MovePositionCommand(plantCommand.target),
                        (moveSuccess) =>
                        {
                            if (moveSuccess)
                            {
                                _plantCommandExecutor.ExecutePlant(plantCommand, OnCommandFinish);
                            }
                            else
                                OnCommandFinish(false);
                        });
                    break;
                case MovePositionCommand movePositionCommand:
                    _moveCommandExecutor?.ExecuteMovePosition(movePositionCommand, OnCommandFinish);
                    break;
                case MoveAndInteractCommand moveAndInteractCommand:
                    _moveCommandExecutor.ExecuteMoveTransform(new MoveTransformCommand(moveAndInteractCommand.target),
                        (moveSuccess) =>
                        {
                            if (moveSuccess)
                            {
                                _interactersManager.Interact(moveAndInteractCommand.target, 
                                    moveAndInteractCommand.interactableType, OnCommandFinish);
                            }
                            else
                                OnCommandFinish(false);
                        });
                    break;
            }
        }

        void OnCommandFinish(bool success)
        {
            if (_commands.Count > 0)
                _commands.RemoveAt(0);
            _processingCommand = false;
        }
    }
}