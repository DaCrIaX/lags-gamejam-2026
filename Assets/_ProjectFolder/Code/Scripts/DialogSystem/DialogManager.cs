using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogManager : SingletonBasic<DialogManager>
{
    [SerializeField] private bool _playOnStart;
    [SerializeField] private List<DialogEntry> _dialogs = new List<DialogEntry>();
    [SerializeField] private UnityEvent _onDialogStarted;
    [SerializeField] private UnityEvent _onDialogFinished;

    private DialogTextDisplay _textDisplay;
    private Dialog _currentDialog;
    private Coroutine _playRoutine;
    private readonly List<Coroutine> _runningEventCoroutines = new List<Coroutine>();

    public bool IsPlaying => _playRoutine != null;

    [Serializable]
    private sealed class DialogEntry
    {
        public string id;
        public Dialog dialog;
        public DialogSceneGraph sceneGraph;
    }

    protected override void Awake()
    {
        base.Awake();

        if (_textDisplay == null)
        {
            _textDisplay = GetComponentInChildren<DialogTextDisplay>();
        }
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
        DialogEntry entry = GetFirstDialog();
        if (entry == null)
        {
            Debug.LogWarning($"DialogManager '{name}' has no dialogs assigned.", this);
            return;
        }

        Play(entry);
    }

    public void Play(string dialogId)
    {
        DialogEntry entry = FindDialog(dialogId);
        if (entry == null)
        {
            Debug.LogWarning($"Dialog '{dialogId}' was not found in {name}.", this);
            return;
        }

        Play(entry);
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

    public void Play(DialogSceneGraph sceneGraph)
    {
        if (sceneGraph == null || sceneGraph.graph == null)
        {
            return;
        }

        Play(sceneGraph.graph);
    }

    public void Stop()
    {
        if (_playRoutine != null)
        {
            StopCoroutine(_playRoutine);
            _playRoutine = null;
        }

        StopRunningEvents();
        _textDisplay?.Clear();
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

        _textDisplay?.Clear();
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

        if (_textDisplay != null)
        {
            yield return _textDisplay.Display(node.message, node.displaySpeed);
            _textDisplay.Clear();
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

    private void Play(DialogEntry entry)
    {
        if (entry.sceneGraph != null)
        {
            Play(entry.sceneGraph);
            return;
        }

        Play(entry.dialog);
    }

    private DialogEntry FindDialog(string dialogId)
    {
        for (int i = 0; i < _dialogs.Count; i++)
        {
            DialogEntry entry = _dialogs[i];
            if (entry != null && entry.id == dialogId)
            {
                return entry;
            }
        }

        return null;
    }

    private DialogEntry GetFirstDialog()
    {
        for (int i = 0; i < _dialogs.Count; i++)
        {
            DialogEntry entry = _dialogs[i];
            if (entry != null && (entry.dialog != null || entry.sceneGraph != null))
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
