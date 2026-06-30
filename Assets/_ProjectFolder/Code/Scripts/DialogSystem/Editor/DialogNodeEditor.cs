using UnityEditor;
using UnityEngine;
using XNodeEditor;

[CustomNodeEditor(typeof(DialogNode))]
public class DialogNodeEditor : NodeEditor
{
    private const int NodeWidth = 430;
    private const float LabelWidth = 220f;
    private const float MinMessageHeight = 48f;
    private const float MessageHorizontalPadding = 32f;
    private const float BlockSpacing = 20f;
    private const float FieldSpacing = 3f;
    private const float SeparatorHeight = 1f;

    private static GUIStyle _messageStyle;
    private static readonly Color SeparatorColor = new Color(0f, 0f, 0f, 0.28f);

    public override int GetWidth()
    {
        return NodeWidth;
    }

    public override void OnBodyGUI()
    {
        serializedObject.Update();

        float previousLabelWidth = EditorGUIUtility.labelWidth;
        EditorGUIUtility.labelWidth = LabelWidth;

        NodeEditorGUILayout.PropertyField(
            serializedObject.FindProperty(nameof(DialogNode.input)),
            new GUIContent("Entrada"),
            true
        );

        DrawSeparator();

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty(nameof(DialogNode.displaySpeed)),
            new GUIContent("Velocidad de display")
        );

        DrawSeparator();

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty(nameof(DialogNode.entryEvent)),
            new GUIContent("Evento de entrada"),
            true
        );

        SpaceField();

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty(nameof(DialogNode.entryEventWait)),
            new GUIContent("Espera evento de entrada")
        );

        SpaceField();

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty(nameof(DialogNode.displayMessageWhileEntryEventRuns)),
            new GUIContent("Mostrar mensaje durante evento")
        );

        DrawSeparator();

        DrawMessageField();

        DrawSeparator();

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty(nameof(DialogNode.exitEvent)),
            new GUIContent("Evento de salida"),
            true
        );

        SpaceField();

        EditorGUILayout.PropertyField(
            serializedObject.FindProperty(nameof(DialogNode.exitEventWait)),
            new GUIContent("Espera evento de salida")
        );

        DrawSeparator();

        NodeEditorGUILayout.PropertyField(
            serializedObject.FindProperty(nameof(DialogNode.output)),
            new GUIContent("Salida"),
            true
        );

        EditorGUIUtility.labelWidth = previousLabelWidth;
        serializedObject.ApplyModifiedProperties();
    }

    private void DrawMessageField()
    {
        SerializedProperty messageProperty = serializedObject.FindProperty(nameof(DialogNode.message));

        EditorGUILayout.LabelField("Mensaje");

        if (_messageStyle == null)
        {
            _messageStyle = new GUIStyle(EditorStyles.textArea)
            {
                wordWrap = true
            };
        }

        float fieldWidth = Mathf.Max(50f, GetWidth() - MessageHorizontalPadding);
        float messageHeight = _messageStyle.CalcHeight(
            new GUIContent(messageProperty.stringValue),
            fieldWidth
        );

        messageHeight = Mathf.Max(MinMessageHeight, messageHeight + EditorGUIUtility.standardVerticalSpacing);
        messageProperty.stringValue = EditorGUILayout.TextArea(
            messageProperty.stringValue,
            _messageStyle,
            GUILayout.MinHeight(messageHeight)
        );
    }

    private static void SpaceBlock()
    {
        GUILayout.Space(BlockSpacing);
    }

    private static void SpaceField()
    {
        GUILayout.Space(FieldSpacing);
    }

    private static void DrawSeparator()
    {
        GUILayout.Space(BlockSpacing * 0.5f);

        Rect rect = EditorGUILayout.GetControlRect(false, SeparatorHeight);
        EditorGUI.DrawRect(rect, SeparatorColor);

        GUILayout.Space(BlockSpacing * 0.5f);
    }
}
