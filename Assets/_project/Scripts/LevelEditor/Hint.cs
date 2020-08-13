using System;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace FeedTheBaby.LevelEditor
{
    public class Hint : MonoBehaviour
    {
        public Animator hintAnimator = null;
        public TextMeshProUGUI hintText;
        public BoxCollider2D triggerRect;
        public float duration;
        public bool showOnce;
        static readonly int Show = Animator.StringToHash("Show");
        static readonly int Hide = Animator.StringToHash("Hide");

        void Awake()
        {
            triggerRect = GetComponent<BoxCollider2D>();
            hintAnimator = GetComponentInChildren<Animator>();
            hintText = GetComponentInChildren<TextMeshProUGUI>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                if (!showOnce)
                {
                    Timer timer = GetComponent<Timer>();
                    timer.TimerEnd += timer1 =>
                    {
                        hintAnimator.SetTrigger(Hide);
                    };
                    
                    timer.StartCount(duration);
                }
                
                hintAnimator.SetTrigger(Show);
            }
        }

        public void LoadHint(HintData hint)
        {
            transform.position = hint.position;
            hintText.text = hint.text;
            triggerRect.offset = hint.triggerOffset;
            triggerRect.size = hint.triggerSize;
            duration = hint.duration;
            showOnce = hint.showOnce;
        }

        public HintData AsHintData()
        {
            return new HintData(transform.position, hintText.text, duration, showOnce,
                triggerRect.offset, triggerRect.size);
        }
    }
    
    #if UNITY_EDITOR
    
    [CustomEditor(typeof(Hint))]
    public class HintEditor : UnityEditor.Editor
    {
        GUIStyle _errorStyle;
        Hint hint => target as Hint;

        void Awake()
        {
            hint.triggerRect = hint.GetComponent<BoxCollider2D>();
            hint.hintText = hint.GetComponentInChildren<TextMeshProUGUI>();
            hint.hintAnimator = hint.GetComponentInChildren<Animator>();
            _errorStyle = new GUIStyle
                {normal = {textColor = Color.red}, alignment = TextAnchor.MiddleCenter};
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label("Hint Editor", EditorStyles.boldLabel);

            if (!hint.hintAnimator)
            {
                GUILayout.Label("Hint does not have an Animator child component!", _errorStyle);
            }

            if (hint.hintText)
            {
                EditorGUI.BeginChangeCheck();
                bool textAreaWordWrapOld = EditorStyles.textArea.wordWrap;
                EditorStyles.textArea.wordWrap = true;
                hint.hintText.text = EditorGUILayout.TextArea(hint.hintText.text, EditorStyles.textArea);
                EditorStyles.textArea.wordWrap = textAreaWordWrapOld;

                if (EditorGUI.EndChangeCheck())
                {
                    EditorUtility.SetDirty(hint.hintText);
                    EditorUtility.SetDirty(hint.hintText.transform.parent);
                }
            }
            else
            {
                GUILayout.Label("Hint does not have a TextMeshProUGUI child component!", _errorStyle);
            }

            hint.transform.position = EditorGUILayout.Vector2Field("Hint Position : ", hint.transform.position);
            
            hint.duration = EditorGUILayout.FloatField("Hint Show Duration : ", hint.duration);
            hint.showOnce = EditorGUILayout.Toggle("Show Hint Once ? ", hint.showOnce);

            if (hint.triggerRect)
            {
                EditorGUI.BeginChangeCheck();
                hint.triggerRect.offset =
                    EditorGUILayout.Vector2Field("Trigger Collider Offset : ", hint.triggerRect.offset);
                hint.triggerRect.size =
                    EditorGUILayout.Vector2Field("Trigger Collider Size : ", hint.triggerRect.size);
            }
            else
            {
                GUILayout.Label("Hint does not have a BoxCollider2D component!", _errorStyle);
            }
        }
    }
    
    #endif
    
    
    [Serializable]
    public struct HintData
    {
        public Vector2 position;
        public string text;
        public float duration;
        public bool showOnce;
        public Vector2 triggerOffset;
        public Vector2 triggerSize;

        public HintData(Vector2 position, string text, float duration, bool showOnce,
            Vector2 triggerOffset, Vector2 triggerSize)
        {
            this.position = position;
            this.text = text;
            this.duration = duration;
            this.showOnce = showOnce;
            this.triggerOffset = triggerOffset;
            this.triggerSize = triggerSize;
        }
    }
}
