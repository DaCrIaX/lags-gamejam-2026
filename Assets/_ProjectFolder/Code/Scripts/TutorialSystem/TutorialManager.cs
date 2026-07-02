using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private DialogManager _dialogManager;
    [SerializeField] private bool _playOnStart = true;
    [SerializeField] private List<CanvasGroup> _uiGroupsToBlock = new List<CanvasGroup>();

    [Header("Events")]
    [SerializeField] private UnityEvent _onTutorialStarted;
    [SerializeField] private UnityEvent _onTutorialFinished;

    private readonly Dictionary<CanvasGroup, CanvasGroupState> _uiGroupStates = new Dictionary<CanvasGroup, CanvasGroupState>();
    private Coroutine _routine;
    private int _currentStepIndex;
    private bool _uiInteractionBlocked;

    private void Awake()
    {
        if (_dialogManager == null)
        {
            _dialogManager = DialogManager.Instance;
        }
    }

    private void Start()
    {
        if (_playOnStart)
        {
            Play();
        }
    }

    public void Play()
    {
        Stop();
        _currentStepIndex = 0;
        ExecuteNextStep();
    }

    public void Stop()
    {
        if (_routine != null)
        {
            StopCoroutine(_routine);
            _routine = null;
        }

        _dialogManager?.Stop();
        AllowUIInteraction();
    }

    public void ExecuteNextStep()
    {
        if (_routine != null)
        {
            return;
        }

        if (_dialogManager == null || _currentStepIndex >= _dialogManager.DialogCount)
        {
            _onTutorialFinished?.Invoke();
            return;
        }

        _routine = StartCoroutine(ExecuteStepRoutine(_currentStepIndex));
        _currentStepIndex++;
    }

    public void BlockUIInteraction()
    {
        if (_uiInteractionBlocked)
        {
            return;
        }

        _uiGroupStates.Clear();

        foreach (CanvasGroup uiGroup in _uiGroupsToBlock)
        {
            if (uiGroup == null)
            {
                continue;
            }

            _uiGroupStates[uiGroup] = new CanvasGroupState(uiGroup);
            uiGroup.interactable = false;
            uiGroup.blocksRaycasts = false;
        }

        _uiInteractionBlocked = true;
    }

    public void AllowUIInteraction()
    {
        if (!_uiInteractionBlocked)
        {
            return;
        }

        foreach (KeyValuePair<CanvasGroup, CanvasGroupState> uiGroupState in _uiGroupStates)
        {
            if (uiGroupState.Key == null)
            {
                continue;
            }

            uiGroupState.Value.Restore(uiGroupState.Key);
        }

        _uiGroupStates.Clear();
        _uiInteractionBlocked = false;
    }

    private IEnumerator ExecuteStepRoutine(int stepIndex)
    {
        if (stepIndex == 0)
        {
            _onTutorialStarted?.Invoke();
        }

        BlockUIInteraction();

        if (_dialogManager.TryPlayAtIndex(stepIndex))
        {
            yield return new WaitWhile(() => _dialogManager.IsPlaying);
        }

        AllowUIInteraction();
        _routine = null;
    }

    private readonly struct CanvasGroupState
    {
        private readonly bool _interactable;
        private readonly bool _blocksRaycasts;

        public CanvasGroupState(CanvasGroup canvasGroup)
        {
            _interactable = canvasGroup.interactable;
            _blocksRaycasts = canvasGroup.blocksRaycasts;
        }

        public void Restore(CanvasGroup canvasGroup)
        {
            canvasGroup.interactable = _interactable;
            canvasGroup.blocksRaycasts = _blocksRaycasts;
        }
    }
}
