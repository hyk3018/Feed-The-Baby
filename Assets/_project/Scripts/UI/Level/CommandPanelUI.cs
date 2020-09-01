using System;
using System.Collections;
using System.Collections.Generic;
using FeedTheBaby.Commands;
using UnityEngine;

namespace FeedTheBaby.UI
{
    public class CommandPanelUI : MonoBehaviour
    {
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

        void ShowPanel(Vector3 position, Transform transform, CommandType possibleCommands)
        {
            commandPanel.transform.position = new Vector3(position.x, position.y, commandPanel.transform.position.z);

            commandButtons.Clear();
            if (!possibleCommands.HasFlag(CommandType.NONE))
            {
                foreach (CommandType commandType in GetFlags(possibleCommands))
                {
                    commandButtons.AddCommand(commandType);
                }
                
                commandButtons.PositionChildren();
            }

            commandPanel.SetActive(true);
        }

        public void HidePanel()
        {
            Debug.Log("LOL");
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