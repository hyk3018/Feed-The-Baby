using System.Linq;
using FeedTheBaby.Commands;
using UnityEngine;

namespace FeedTheBaby.UI
{
    class CommandButtonsUI : RadialLayoutUI
    {
        [SerializeField] CommandButtonMap commandButtonMap = null;

        public void AddCommand(CommandType commandType)
        {
            GameObject buttonPrefab = commandButtonMap.GetPrefab(commandType);
            if (buttonPrefab)
            {
                Instantiate(buttonPrefab, transform);
            }
        }
    }
}