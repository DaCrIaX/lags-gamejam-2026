using UnityEditor;
using UnityEditor.UI;

namespace UnityEngine.UI
{
    [CustomEditor(typeof(HOVLayoutGroupSmooth), true)]
    [CanEditMultipleObjects]
    public class HOVLayoutGroupSmoothEditor : HorizontalOrVerticalLayoutGroupEditor
    {
        private SerializedProperty _curve;
        private SerializedProperty _duration;

        protected override void OnEnable()
        {
            base.OnEnable();
            _curve = serializedObject.FindProperty("_curve");
            _duration = serializedObject.FindProperty("_duration");
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Animation", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_curve, true);
            EditorGUILayout.PropertyField(_duration, true);
        }
    }
}