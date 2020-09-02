using System;
using System.Collections.Generic;
using FeedTheBaby.Commands;
using FeedTheBaby.Game;
using FeedTheBaby.LevelObjects;
using UnityEngine;

namespace FeedTheBaby.UI
{
    public class CommandPanelUI : MonoBehaviour
    {
        [SerializeField] CommandManager commandManager = null;
        [SerializeField] CommandInput commandInput = null;
        [SerializeField] GameObject commandPanel = null;
        [SerializeField] CommandButtonsUI commandButtons = null;

        void Start()
        {
            LevelManager.Instance.LevelEnd += HidePanel;
            commandInput.OnCommandPanelOpen += ShowPanel;
            commandInput.OnCommandPanelClose += HidePanel;
            commandPanel.SetActive(false);
        }

        void ShowPanel(Vector3 position, Transform interactableTransform, CommandType possibleCommands)
        {
            commandPanel.transform.position = new Vector3(position.x, position.y, commandPanel.transform.position.z);

            if (!possibleCommands.HasFlag(CommandType.NONE))
            {
                foreach (CommandType commandType in GetFlags(possibleCommands))
                {
                    CommandButton button = commandButtons.AddCommand(commandType);
                    SetCommandAction(button, position, interactableTransform, commandType);
                }
                //
                // foreach (CommandType commandType in Enum.GetValues(typeof(CommandType)))
                // {
                //     if (commandType == CommandType.NONE)
                //         continue;
                //     
                //     CommandButton button = commandButtons.AddCommand(commandType);
                //     SetCommandAction(button, position, interactableTransform, commandType);
                // }
                
                commandButtons.PositionChildren();
            }

            commandPanel.SetActive(true);
            commandInput.panelOpen = true;
             StartCoroutine(commandButtons.ShowButtons());
        }

        void SetCommandAction(CommandButton button, Vector3 position, Transform interactableTransform, CommandType commandType)
        {
            switch (commandType)
            {
                case CommandType.NONE:
                    break;
                case CommandType.MOVE:
                    button.SetCommand(() =>
                    {
                        commandManager.AddCommand(new MovePositionCommand(position));
                        HidePanel();
                    });
                    break;
                case CommandType.FEED:
                    button.SetCommand(() =>
                    {
                        commandManager.AddCommand(new MoveAndInteractCommand(interactableTransform, typeof(IFeedable)));
                        HidePanel();
                    });
                    break;
                case CommandType.PLANT_FUEL:
                    button.SetCommand(() =>
                    {
                        commandManager.AddCommand(new PlantCommand(position));
                        HidePanel();
                    });
                    break;
                case CommandType.HARVEST:
                    button.SetCommand(() =>
                    {
                        commandManager.AddCommand(new MoveAndInteractCommand(interactableTransform, typeof(IHarvestable)));
                        HidePanel();
                    });
                    break;
                case CommandType.CRAFT:
                    button.SetCommand(() =>
                    {
                        HidePanel();
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null);
            }
        }

        public void HidePanel()
        {
            commandInput.panelOpen = false;
            commandButtons.Clear();
            commandPanel.SetActive(false);
        }
        
        static IEnumerable<Enum> GetFlags(Enum input)
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return value;
        }
    }
}