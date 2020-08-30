using System;
using FeedTheBaby.Commands;
using UnityEngine;

namespace FeedTheBaby.UI
{
    public class CommandPanelUI : MonoBehaviour
    {
        [SerializeField] CommandInput commandInput = null;
        [SerializeField] GameObject commandPanel = null;

        void Start()
        {
            commandInput.OnTileHeld += ShowPanel;
            commandPanel.SetActive(false);
        }

        void ShowPanel(Vector3 position, Transform transform, HoldType holdType)
        {
            if (holdType == HoldType.GROUND)
            {
                commandPanel.SetActive(true);
                commandPanel.transform.position = new Vector3(position.x, position.y, commandPanel.transform.position.z);
            }
        }

        public void HidePanel()
        {
            commandPanel.SetActive(false);
        }
    }
}