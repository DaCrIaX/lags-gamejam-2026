using UnityEngine.UI;

namespace UnityEditor.UI
{
    [CustomEditor(typeof(ButtonString))]
    public class ButtonStringEditor : ButtonEditor
    {
        private SerializedProperty _textUI;
        private SerializedProperty _text;

        protected override void OnEnable()
        {
            base.OnEnable();
            _textUI = serializedObject.FindProperty("_textUI");
            _text = serializedObject.FindProperty("_textPrefix");
        }
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Text Button", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_textUI);
            EditorGUILayout.PropertyField(_text);

            serializedObject.ApplyModifiedProperties();

            EditorGUILayout.Space();
            base.OnInspectorGUI();
        }
    }
}