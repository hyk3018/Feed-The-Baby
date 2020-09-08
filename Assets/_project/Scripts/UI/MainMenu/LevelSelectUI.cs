using System;
using System.Collections.Generic;
using FeedTheBaby.GameData;
using FeedTheBaby.LevelEditor;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FeedTheBaby.UI
{
    public class LevelSelectUI : DisableablePanel
    {
        [SerializeField] GameObject buttonPrefab = null;
        [SerializeField] GameObject content = null;
        [SerializeField] GameObject focusedLevelUI = null;

        List<Button> _levelButtons;

        static readonly int ButtonDisabled = Animator.StringToHash("Disabled");
        static readonly int ButtonNormal = Animator.StringToHash("Normal");

        protected override void Awake()
        {
            base.Awake();

            _levelButtons = new List<Button>();
            var levelDatas = DataService.Instance.GetLevels();

            for (var i = 0; i < levelDatas.Length; i++)
            {
                var buttonGameObject = Instantiate(buttonPrefab, content.transform);
                var buttonLevelSelect = buttonGameObject.GetComponent<LevelSelector>();
                buttonLevelSelect.SetLevelToSelect(i);
                buttonLevelSelect.focusedLevelUI = focusedLevelUI.GetComponent<FocusedLevelUI>();

                var text = buttonGameObject.transform.GetComponentInChildren<TextMeshProUGUI>();
                text.text = (i + 1).ToString();

                var button = buttonGameObject.GetComponent<Button>();
                if (i >= DataService.Instance.GetLevelsUnlocked())
                {
                    button.enabled = false;
                    buttonGameObject.GetComponent<Animator>().SetTrigger(ButtonDisabled);
                }

                _buttons.Add(button);
                _levelButtons.Add(button);
            }
        }

        public override void Enable()
        {
            foreach (var button in _buttons)
                if (_levelButtons == null || !_levelButtons.Contains(button))
                    button.enabled = true;

            if (_levelButtons != null)
                for (var i = 0; i < _levelButtons.Count; i++)
                    if (i < DataService.Instance.GetLevelsUnlocked())
                    {
                        _levelButtons[i].enabled = true;
                        _levelButtons[i].GetComponent<Animator>().SetTrigger(ButtonNormal);
                    }
                    else
                    {
                        _levelButtons[i].enabled = false;
                        _levelButtons[i].GetComponent<Animator>().SetTrigger(ButtonDisabled);
                    }
        }
    }
}