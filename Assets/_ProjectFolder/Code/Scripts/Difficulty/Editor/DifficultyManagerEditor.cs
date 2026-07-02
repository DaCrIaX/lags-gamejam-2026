using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DifficultyManager))]
public class DifficultyManagerEditor : Editor
{
    private const float GraphHeight = 150f;
    private const float PointRadius = 4f;
    private const float MinBarNormalizedHeight = 0.12f;

    private SerializedProperty _startingCycle;
    private SerializedProperty _roundsPerCycle;
    private SerializedProperty _maxCyclePhaseShift;
    private SerializedProperty _intensityCurve;
    private SerializedProperty _lowClientRange;
    private SerializedProperty _mediumClientRange;
    private SerializedProperty _highClientRange;
    private SerializedProperty _rushClientRange;
    private SerializedProperty _useVariableClientTime;
    private SerializedProperty _fixedClientTimeLimit;
    private SerializedProperty _lowClientTimeRange;
    private SerializedProperty _mediumClientTimeRange;
    private SerializedProperty _highClientTimeRange;
    private SerializedProperty _rushClientTimeRange;
    private SerializedProperty _vignetteVolume;

    private float _previewPhaseShift;

    private void OnEnable()
    {
        _startingCycle = serializedObject.FindProperty("_startingCycle");
        _roundsPerCycle = serializedObject.FindProperty("_roundsPerCycle");
        _maxCyclePhaseShift = serializedObject.FindProperty("_maxCyclePhaseShift");
        _intensityCurve = serializedObject.FindProperty("_intensityCurve");
        _lowClientRange = serializedObject.FindProperty("_lowClientRange");
        _mediumClientRange = serializedObject.FindProperty("_mediumClientRange");
        _highClientRange = serializedObject.FindProperty("_highClientRange");
        _rushClientRange = serializedObject.FindProperty("_rushClientRange");
        _useVariableClientTime = serializedObject.FindProperty("_useVariableClientTime");
        _fixedClientTimeLimit = serializedObject.FindProperty("_fixedClientTimeLimit");
        _lowClientTimeRange = serializedObject.FindProperty("_lowClientTimeRange");
        _mediumClientTimeRange = serializedObject.FindProperty("_mediumClientTimeRange");
        _highClientTimeRange = serializedObject.FindProperty("_highClientTimeRange");
        _rushClientTimeRange = serializedObject.FindProperty("_rushClientTimeRange");
        _vignetteVolume = serializedObject.FindProperty("_vignetteVolume");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawSettings();

        serializedObject.ApplyModifiedProperties();

        EditorGUILayout.Space(10f);
        DrawRuntimeDebug();
        DrawDifficultyGraph();
        DrawRoundsTable();

        if (Application.isPlaying)
            Repaint();
    }

    private void DrawSettings()
    {
        EditorGUILayout.PropertyField(_startingCycle);
        EditorGUILayout.PropertyField(_roundsPerCycle);
        EditorGUILayout.PropertyField(_maxCyclePhaseShift);
        EditorGUILayout.PropertyField(_intensityCurve);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_lowClientRange);
        EditorGUILayout.PropertyField(_mediumClientRange);
        EditorGUILayout.PropertyField(_highClientRange);
        EditorGUILayout.PropertyField(_rushClientRange);

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_useVariableClientTime);
        EditorGUILayout.PropertyField(_fixedClientTimeLimit);

        using (new EditorGUI.DisabledScope(!_useVariableClientTime.boolValue))
        {
            EditorGUILayout.PropertyField(_lowClientTimeRange);
            EditorGUILayout.PropertyField(_mediumClientTimeRange);
            EditorGUILayout.PropertyField(_highClientTimeRange);
            EditorGUILayout.PropertyField(_rushClientTimeRange);
        }

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(_vignetteVolume);
    }

    private void DrawRuntimeDebug()
    {
        var manager = (DifficultyManager)target;

        EditorGUILayout.LabelField("Runtime Debug", EditorStyles.boldLabel);

        using (new EditorGUI.DisabledScope(true))
        {
            EditorGUILayout.IntField("Current Cycle", Application.isPlaying ? manager.CurrentCycle : manager.StartingCycle);
            EditorGUILayout.FloatField("Cycle Phase Shift", Application.isPlaying ? manager.CurrentCyclePhaseShift : _previewPhaseShift);
            EditorGUILayout.Toggle("Using Variable Client Time", manager.UseVariableClientTime);
            EditorGUILayout.FloatField("Current Client Time Limit", manager.CurrentClientTimeLimit);

            var currentRound = manager.CurrentRound;
            EditorGUILayout.IntField("Current Round", currentRound != null ? currentRound.RoundNumber : 0);
            EditorGUILayout.EnumPopup("Current Complexity", currentRound != null ? currentRound.Complexity : ComplexityLevel.Bajo);
            EditorGUILayout.IntField("Current Round Clients", currentRound != null ? currentRound.ClientAmount : 0);
            EditorGUILayout.FloatField("Current Round Client Time", currentRound != null ? currentRound.ClientTimeLimit : 0f);
        }

        if (!Application.isPlaying)
        {
            float maxShift = _maxCyclePhaseShift.floatValue;
            _previewPhaseShift = EditorGUILayout.Slider("Preview Phase Shift", _previewPhaseShift, 0f, Mathf.Max(0.0001f, maxShift));
        }
    }

    private void DrawDifficultyGraph()
    {
        var manager = (DifficultyManager)target;
        var rounds = GetVisibleRounds(manager);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(Application.isPlaying ? "Current Cycle Graph" : "Preview Cycle Graph", EditorStyles.boldLabel);

        Rect graphRect = GUILayoutUtility.GetRect(10f, GraphHeight, GUILayout.ExpandWidth(true));
        EditorGUI.DrawRect(graphRect, new Color(0.12f, 0.12f, 0.12f));

        if (rounds.Count == 0)
        {
            EditorGUI.LabelField(graphRect, "No round data generated yet.", CenteredStyle());
            return;
        }

        DrawGrid(graphRect);
        DrawBars(graphRect, rounds);
        DrawLine(graphRect, rounds);
    }

    private void DrawRoundsTable()
    {
        var manager = (DifficultyManager)target;
        var rounds = GetVisibleRounds(manager);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField(Application.isPlaying ? "Current Cycle Rounds" : "Preview Rounds", EditorStyles.boldLabel);

        if (rounds.Count == 0)
        {
            EditorGUILayout.HelpBox("Round data will be generated when the cycle starts.", MessageType.Info);
            return;
        }

        using (new EditorGUILayout.HorizontalScope(EditorStyles.helpBox))
        {
            GUILayout.Label("#", GUILayout.Width(24f));
            GUILayout.Label("Intensity", GUILayout.Width(70f));
            GUILayout.Label("Complexity", GUILayout.Width(80f));
            GUILayout.Label(Application.isPlaying ? "Clients" : "Clients Preview", GUILayout.Width(110f));
            GUILayout.Label(Application.isPlaying ? "Time" : "Time Preview", GUILayout.Width(110f));
        }

        foreach (var round in rounds)
        {
            var clientRange = manager.GetClientRange(round.Complexity);
            var timeRange = manager.GetClientTimeRange(round.Complexity);
            string previewTime = manager.UseVariableClientTime ? $"{timeRange.Min:0.#}-{timeRange.Max:0.#}s" : $"{manager.FixedClientTimeLimit:0.#}s";

            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label(round.RoundNumber.ToString(), GUILayout.Width(24f));
                GUILayout.Label(round.Intensity.ToString("0.00"), GUILayout.Width(70f));
                GUILayout.Label(round.Complexity.ToString(), GUILayout.Width(80f));
                GUILayout.Label(Application.isPlaying ? round.ClientAmount.ToString() : $"{clientRange.Min}-{clientRange.Max}", GUILayout.Width(110f));
                GUILayout.Label(Application.isPlaying ? $"{manager.GetClientTimeLimit(round):0.0}s" : previewTime, GUILayout.Width(110f));
            }
        }
    }

    private List<DifficultyRoundData> GetVisibleRounds(DifficultyManager manager)
    {
        if (Application.isPlaying && manager.Rounds.Count > 0)
            return new List<DifficultyRoundData>(manager.Rounds);

        return new List<DifficultyRoundData>(manager.BuildPreviewRounds(_previewPhaseShift));
    }

    private static void DrawGrid(Rect rect)
    {
        Handles.BeginGUI();
        Handles.color = new Color(1f, 1f, 1f, 0.12f);

        for (int i = 1; i < 4; i++)
        {
            float y = Mathf.Lerp(rect.yMax, rect.yMin, i / 4f);
            Handles.DrawLine(new Vector3(rect.xMin, y), new Vector3(rect.xMax, y));
        }

        Handles.EndGUI();
    }

    private static void DrawBars(Rect rect, IReadOnlyList<DifficultyRoundData> rounds)
    {
        float slotWidth = rect.width / rounds.Count;

        for (int i = 0; i < rounds.Count; i++)
        {
            var round = rounds[i];
            float visualIntensity = Mathf.Lerp(MinBarNormalizedHeight, 1f, round.Intensity);
            float barHeight = Mathf.Lerp(0f, rect.height - 24f, visualIntensity);
            Rect barRect = new(
                rect.xMin + i * slotWidth + slotWidth * 0.18f,
                rect.yMax - barHeight - 18f,
                slotWidth * 0.64f,
                barHeight);

            EditorGUI.DrawRect(barRect, GetComplexityColor(round.Complexity));

            Rect labelRect = new(rect.xMin + i * slotWidth, rect.yMax - 18f, slotWidth, 16f);
            EditorGUI.LabelField(labelRect, round.RoundNumber.ToString(), CenteredStyle());
        }
    }

    private static void DrawLine(Rect rect, IReadOnlyList<DifficultyRoundData> rounds)
    {
        Handles.BeginGUI();
        Handles.color = Color.white;

        Vector3? previous = null;

        for (int i = 0; i < rounds.Count; i++)
        {
            float x = Mathf.Lerp(rect.xMin + 18f, rect.xMax - 18f, rounds.Count <= 1 ? 0.5f : i / (rounds.Count - 1f));
            float y = Mathf.Lerp(rect.yMax - 18f, rect.yMin + 12f, rounds[i].Intensity);
            Vector3 point = new(x, y);

            if (previous.HasValue)
                Handles.DrawLine(previous.Value, point);

            Handles.color = Color.white;
            Handles.DrawSolidDisc(point, Vector3.forward, PointRadius);
            previous = point;
        }

        Handles.EndGUI();
    }

    private static Color GetComplexityColor(ComplexityLevel complexity)
    {
        return complexity switch
        {
            ComplexityLevel.Bajo => new Color(0.35f, 0.8f, 0.35f),
            ComplexityLevel.Medio => new Color(1f, 0.8f, 0.25f),
            ComplexityLevel.Alto => new Color(1f, 0.35f, 0.15f),
            ComplexityLevel.Rush => new Color(1f, 0f, 0f),
            _ => Color.gray
        };
    }

    private static GUIStyle CenteredStyle()
    {
        var style = new GUIStyle(EditorStyles.miniLabel)
        {
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white }
        };

        return style;
    }
}
