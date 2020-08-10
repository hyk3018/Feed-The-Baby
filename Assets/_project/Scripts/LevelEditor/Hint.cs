using System;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace FeedTheBaby.LevelEditor
{
    public class Hint : MonoBehaviour
    {
        Transform trigger;
        public TextMeshProUGUI hintText;
        public RectTransform rectTransform;
        
        public HintData hintData;

        void Awake()
        {
            
        }

        public void SetData(HintData toSet)
        {
            hintData = toSet;
            hintText.text = toSet.text;
            
        }
    }
    
    #if UNITY_EDITOR
    
    [CustomEditor(typeof(Hint))]
    public class HintEditor : UnityEditor.Editor
    {
        Hint hint => target as Hint;

        void Awake()
        {
            hint.hintText.text = hint.hintData.text;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            GUILayout.Label("Hint Editor");
            
            
            
            EditorGUI.BeginChangeCheck();

            bool textAreaWordWrapOld = EditorStyles.textArea.wordWrap;
            EditorStyles.textArea.wordWrap = true;
            hint.hintData.text = EditorGUILayout.TextArea(hint.hintData.text, EditorStyles.textArea);
            EditorStyles.textArea.wordWrap = textAreaWordWrapOld;
            
            if (EditorGUI.EndChangeCheck())
            {
                hint.hintText.text = hint.hintData.text;
                EditorUtility.SetDirty(hint.hintText);
                EditorUtility.SetDirty(hint.hintText.transform.parent);
            }

            hint.hintData.position = EditorGUILayout.Vector2Field("Hint Position : ", hint.hintData.position);
            hint.hintData.duration = EditorGUILayout.FloatField("Hint Show Duration : ", hint.hintData.duration);
            hint.hintData.showOnce = EditorGUILayout.Toggle("Show Hint Once ? ", hint.hintData.showOnce);
            
            
        }
    }
    
    #endif
}
