using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class DialogManager : SingletonBasic<DialogManager>
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private bool _playOnStart;
    [SerializeField] private List<DialogEntry> _dialogs = new List<DialogEntry>();
    [SerializeField] private UnityEvent _onDialogStarted;
    [SerializeField] private UnityEvent _onDialogFinished;

    private Dialog _currentDialog;
    private List<DialogBubbleEvents> _currentBubbleEvents;
    private Coroutine _playRoutine;
    private readonly List<Coroutine> _runningEventCoroutines = new List<Coroutine>();

    public bool IsPlaying => _playRoutine != null;
    public int DialogCount => _dialogs.Count;

    [Serializable]
    private sealed class DialogEntry
    {
        public Dialog dialog;
        public List<DialogBubbleEvents> bubbleEvents = new List<DialogBubbleEvents>();
    }

    [Serializable]
    private sealed class DialogBubbleEvents
    {
        public UnityEvent entryEvent = new UnityEvent();
        [Min(0f)] public float entryEventWait;
        public bool displayMessageWhileEntryEventRuns;
        public UnityEvent exitEvent = new UnityEvent();
        [Min(0f)] public float exitEventWait;
    }

    protected override void Awake()
    {
        base.Awake();

        if (_text == null)
        {
            _text = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        ClearText();
    }

    private void Start()
    {
        if (_playOnStart)
        {
            PlayFirst();
        }
    }

    public void Play()
    {
        PlayFirst();
    }

    public void PlayFirst()
    {
        DialogEntry entry = GetFirstDialogEntry();
        if (entry == null)
        {
            Debug.LogWarning($"DialogManager '{name}' has no dialogs assigned.", this);
            return;
        }

        Play(entry);
    }

    public void PlayAtIndex(int dialogIndex)
    {
        TryPlayAtIndex(dialogIndex);
    }

    public bool TryPlayAtIndex(int dialogIndex)
    {
        DialogEntry entry = GetDialogEntryAt(dialogIndex);
        if (entry == null)
        {
            Debug.LogWarning($"DialogManager '{name}' has no valid dialog at index {dialogIndex}.", this);
            return false;
        }

        Play(entry);
        return true;
    }

    public int GetDialogIndex(Dialog dialog)
    {
        if (dialog == null)
        {
            return -1;
        }

        for (int i = 0; i < _dialogs.Count; i++)
        {
            DialogEntry entry = _dialogs[i];
            if (entry != null && entry.dialog == dialog)
            {
                return i;
            }
        }

        return -1;
    }

    public Dialog GetDialogAtIndex(int dialogIndex)
    {
        DialogEntry entry = GetDialogEntryAt(dialogIndex);
        return entry?.dialog;
    }

    public void Play(Dialog dialog)
    {
        if (dialog == null)
        {
            return;
        }

        DialogEntry entry = FindDialogEntry(dialog);
        if (entry != null)
        {
            Play(entry);
            return;
        }

        Play(dialog, null);
    }

    public void Stop()
    {
        if (_playRoutine != null)
        {
            StopCoroutine(_playRoutine);
            _playRoutine = null;
        }

        StopRunningEvents();
        ClearText();
        SetTextParentActive(false);
    }

    private void Play(DialogEntry entry)
    {
        Play(entry.dialog, entry.bubbleEvents);
    }

    private void Play(Dialog dialog, List<DialogBubbleEvents> bubbleEvents)
    {
        Stop();
        _currentDialog = dialog;
        _currentBubbleEvents = bubbleEvents;
        _playRoutine = StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine()
    {
        SetTextParentActive(true);
        _onDialogStarted?.Invoke();

        int bubbleIndex = 0;
        DialogNode currentNode = _currentDialog.GetFirstNode();
        while (currentNode != null)
        {
            yield return RunNode(currentNode, GetBubbleEvents(bubbleIndex));
            currentNode = currentNode.GetNextNode();
            bubbleIndex++;
        }

        ClearText();
        _onDialogFinished?.Invoke();
        SetTextParentActive(false);
        _playRoutine = null;
        _runningEventCoroutines.Clear();
    }

    private IEnumerator RunNode(DialogNode node, DialogBubbleEvents bubbleEvents)
    {
        Coroutine entryCoroutine = StartNodeEvent(bubbleEvents?.entryEvent, bubbleEvents?.entryEventWait ?? 0f);

        if (!(bubbleEvents?.displayMessageWhileEntryEventRuns ?? false) && entryCoroutine != null)
        {
            yield return entryCoroutine;
        }

        if (_text != null)
        {
            yield return DisplayText(node.message, node.displaySpeed);
            yield return WaitForClick(node);
            ClearText();
        }

        if ((bubbleEvents?.displayMessageWhileEntryEventRuns ?? false) && entryCoroutine != null)
        {
            yield return entryCoroutine;
        }

        Coroutine exitCoroutine = StartNodeEvent(bubbleEvents?.exitEvent, bubbleEvents?.exitEventWait ?? 0f);
        if (exitCoroutine != null)
        {
            yield return exitCoroutine;
        }
    }

    private Coroutine StartNodeEvent(UnityEvent nodeEvent, float waitAfterInvoke)
    {
        if (nodeEvent == null)
        {
            return null;
        }

        Coroutine coroutine = StartCoroutine(NodeEventRoutine(nodeEvent, waitAfterInvoke));
        _runningEventCoroutines.Add(coroutine);
        return coroutine;
    }

    private IEnumerator NodeEventRoutine(UnityEvent nodeEvent, float waitAfterInvoke)
    {
        nodeEvent.Invoke();

        if (waitAfterInvoke > 0f)
        {
            yield return new WaitForSeconds(waitAfterInvoke);
        }
    }

    private IEnumerator WaitForClick(DialogNode node)
    {
        if (string.IsNullOrEmpty(node.message))
        {
            yield break;
        }

        yield return new WaitUntil(ContinueClickWasPressed);
    }

    private bool ContinueClickWasPressed()
    {
        Mouse mouse = Mouse.current;
        return mouse != null && mouse.leftButton.wasPressedThisFrame;
    }

    private IEnumerator DisplayText(string message, float charactersPerSecond)
    {
        if (_text == null)
        {
            yield break;
        }

        _text.SetText(message ?? string.Empty);
        _text.ForceMeshUpdate();

        SetAllCharactersAlpha(0);

        int characterCount = _text.textInfo.characterCount;
        if (characterCount == 0)
        {
            yield break;
        }

        float delay = charactersPerSecond <= 0f ? 0f : 1f / charactersPerSecond;

        for (int i = 0; i < characterCount; i++)
        {
            SetCharacterAlpha(i, 255);

            if (delay > 0f)
            {
                yield return new WaitForSeconds(delay);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void ClearText()
    {
        if (_text != null)
        {
            _text.SetText(string.Empty);
        }
    }

    private void SetTextParentActive(bool active)
    {
        if (_text == null || _text.transform.parent == null)
        {
            return;
        }

        _text.transform.parent.gameObject.SetActive(active);
    }

    private void SetAllCharactersAlpha(byte alpha)
    {
        TMP_TextInfo textInfo = _text.textInfo;

        for (int i = 0; i < textInfo.characterCount; i++)
        {
            SetCharacterAlpha(i, alpha);
        }
    }

    private void SetCharacterAlpha(int characterIndex, byte alpha)
    {
        TMP_TextInfo textInfo = _text.textInfo;
        TMP_CharacterInfo characterInfo = textInfo.characterInfo[characterIndex];

        if (!characterInfo.isVisible)
        {
            return;
        }

        int meshIndex = characterInfo.materialReferenceIndex;
        int vertexIndex = characterInfo.vertexIndex;
        Color32[] vertexColors = textInfo.meshInfo[meshIndex].colors32;

        vertexColors[vertexIndex].a = alpha;
        vertexColors[vertexIndex + 1].a = alpha;
        vertexColors[vertexIndex + 2].a = alpha;
        vertexColors[vertexIndex + 3].a = alpha;

        _text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    private DialogBubbleEvents GetBubbleEvents(int bubbleIndex)
    {
        if (_currentBubbleEvents == null ||
            bubbleIndex < 0 ||
            bubbleIndex >= _currentBubbleEvents.Count)
        {
            return null;
        }

        return _currentBubbleEvents[bubbleIndex];
    }

    private DialogEntry GetFirstDialogEntry()
    {
        for (int i = 0; i < _dialogs.Count; i++)
        {
            DialogEntry entry = _dialogs[i];
            if (entry != null && entry.dialog != null)
            {
                return entry;
            }
        }

        return null;
    }

    private DialogEntry GetDialogEntryAt(int dialogIndex)
    {
        if (dialogIndex < 0 || dialogIndex >= _dialogs.Count)
        {
            return null;
        }

        DialogEntry entry = _dialogs[dialogIndex];
        if (entry == null || entry.dialog == null)
        {
            return null;
        }

        return entry;
    }

    private DialogEntry FindDialogEntry(Dialog dialog)
    {
        for (int i = 0; i < _dialogs.Count; i++)
        {
            DialogEntry entry = _dialogs[i];
            if (entry != null && entry.dialog == dialog)
            {
                return entry;
            }
        }

        return null;
    }

    private void StopRunningEvents()
    {
        foreach (Coroutine eventCoroutine in _runningEventCoroutines)
        {
            if (eventCoroutine != null)
            {
                StopCoroutine(eventCoroutine);
            }
        }

        _runningEventCoroutines.Clear();
    }
}
