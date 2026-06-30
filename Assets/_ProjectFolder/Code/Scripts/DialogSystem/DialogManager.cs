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
    [SerializeField] private bool _waitForContinueInput = true;
    [SerializeField] private Key _continueKey = Key.Space;
    [SerializeField] private List<Dialog> _dialogs = new List<Dialog>();
    [SerializeField] private UnityEvent _onDialogStarted;
    [SerializeField] private UnityEvent _onDialogFinished;

    private Dialog _currentDialog;
    private Coroutine _playRoutine;
    private readonly List<Coroutine> _runningEventCoroutines = new List<Coroutine>();

    public bool IsPlaying => _playRoutine != null;

    protected override void Awake()
    {
        base.Awake();

        if (_text == null)
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
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
        Dialog dialog = GetFirstDialog();
        if (dialog == null)
        {
            Debug.LogWarning($"DialogManager '{name}' has no dialogs assigned.", this);
            return;
        }

        Play(dialog);
    }

    public void Play(Dialog dialog)
    {
        if (dialog == null)
        {
            return;
        }

        Stop();
        _currentDialog = dialog;
        _playRoutine = StartCoroutine(PlayRoutine());
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
    }

    private IEnumerator PlayRoutine()
    {
        _onDialogStarted?.Invoke();

        DialogNode currentNode = _currentDialog.GetFirstNode();
        while (currentNode != null)
        {
            yield return RunNode(currentNode);
            currentNode = currentNode.GetNextNode();
        }

        ClearText();
        _onDialogFinished?.Invoke();
        _playRoutine = null;
        _runningEventCoroutines.Clear();
    }

    private IEnumerator RunNode(DialogNode node)
    {
        Coroutine entryCoroutine = StartNodeEvent(node.entryEvent, node.entryEventWait);

        if (!node.displayMessageWhileEntryEventRuns && entryCoroutine != null)
        {
            yield return entryCoroutine;
        }

        if (_text != null)
        {
            yield return DisplayText(node.message, node.displaySpeed);
            yield return WaitForContinueInput(node);
            ClearText();
        }

        if (node.displayMessageWhileEntryEventRuns && entryCoroutine != null)
        {
            yield return entryCoroutine;
        }

        Coroutine exitCoroutine = StartNodeEvent(node.exitEvent, node.exitEventWait);
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

    private IEnumerator WaitForContinueInput(DialogNode node)
    {
        if (!_waitForContinueInput || string.IsNullOrEmpty(node.message))
        {
            yield break;
        }

        yield return new WaitUntil(ContinueKeyWasPressed);
    }

    private bool ContinueKeyWasPressed()
    {
        Keyboard keyboard = Keyboard.current;
        return keyboard != null && keyboard[_continueKey].wasPressedThisFrame;
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

    private Dialog GetFirstDialog()
    {
        for (int i = 0; i < _dialogs.Count; i++)
        {
            Dialog dialog = _dialogs[i];
            if (dialog != null)
            {
                return dialog;
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
