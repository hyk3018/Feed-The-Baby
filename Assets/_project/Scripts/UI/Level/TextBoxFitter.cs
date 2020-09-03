using TMPro;
using UnityEngine;

namespace FeedTheBaby.UI
{
    [ExecuteInEditMode]
    public class TextBoxFitter : MonoBehaviour
    {
        [SerializeField] float maxWidth = 150;
        [SerializeField] float maxHeight = 250;
        TextMeshProUGUI _text;
        bool _sizeConfigured;

        void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();
            _sizeConfigured = false;
        }

        void Update()
        {
            if (_sizeConfigured)
                return;
            
            if (_text.preferredWidth > maxWidth && _text.preferredHeight > maxHeight)
            {
                _text.fontSize -= 0.5f;
            }
            else
            {
                _sizeConfigured = true;
            }
        }

        public void SetText(string text)
        {
            _text.text = text;
            _sizeConfigured = false;
        }
    }
}
