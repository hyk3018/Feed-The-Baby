using System.Collections;
using System.Linq;
using FeedTheBaby.Commands;
using UnityEngine;

namespace FeedTheBaby.UI
{
    class CommandButtonsUI : RadialLayoutUI
    {
        [SerializeField] CommandButtonMap commandButtonMap = null;

        public CommandButton AddCommand(CommandType commandType)
        {
            GameObject buttonPrefab = commandButtonMap.GetPrefab(commandType);
            if (buttonPrefab)
            {
                GameObject buttonGameObject = Instantiate(buttonPrefab, transform);
                buttonGameObject.SetActive(false);
                return buttonGameObject.GetComponent<CommandButton>();
            }

            return null;
        }

        public IEnumerator ShowButtons()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);
            }
        }
    }
}