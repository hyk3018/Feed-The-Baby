using TMPro;
using UnityEditor;
using UnityEngine;

namespace FeedTheBaby.UI
{
    [ExecuteInEditMode]
    public class TextBoxFitter : MonoBehaviour
    {
        [SerializeField] float maxWidth = 150;
        [SerializeField] float maxHeight = 250;
        TextMeshProUGUI _text;

        void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        void Update()
        {
        }

        public void SetText(string text)
        {
            _text.text = text;
        }

        public void Refit()
        {
            while (_text.preferredWidth > maxWidth && _text.preferredHeight > maxHeight)
            {
                _text.fontSize = Mathf.Min(_text.fontSize - 0.5f, 10);
            }
            
            while (_text.preferredWidth <= maxWidth || _text.preferredHeight <= maxHeight)
            {
                _text.fontSize = Mathf.Min(_text.fontSize + 0.5f, 10);
            }
        }
    }

    [CustomEditor(typeof(TextBoxFitter))]
    public class TextBoxFitterEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Refit"))
                ((TextBoxFitter) target).Refit();
        }
    }
}
