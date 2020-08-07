using TMPro;
using UnityEngine;

namespace FeedTheBaby
{
    public class TextBoxFitter : MonoBehaviour
    {
        RectTransform _rectTransform;
        TextMeshProUGUI _text;

        void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _text = GetComponent<TextMeshProUGUI>();
            _rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _text.preferredWidth / 3);
        }

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
