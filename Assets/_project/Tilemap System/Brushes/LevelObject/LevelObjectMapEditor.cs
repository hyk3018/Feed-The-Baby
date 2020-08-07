#if UNITY_EDITOR

using UnityEditor;

namespace FeedTheBaby.Brushes
{
    public class LevelObjectMapEditor : UnityEditor.Editor
    {
        SerializedObject _serializedObject;
        SerializedProperty _dictionary;

        void OnEnable()
        {
            _serializedObject = new SerializedObject(target);
            _dictionary = _serializedObject.FindProperty("dictionary");
        }

        public override void OnInspectorGUI()
        {
            _serializedObject.UpdateIfRequiredOrScript();
            EditorGUILayout.PropertyField(_dictionary);
            _serializedObject.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}

#endif