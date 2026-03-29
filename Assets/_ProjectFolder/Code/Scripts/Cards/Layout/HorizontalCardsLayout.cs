using UnityEngine;
using UnityEngine.UI;

public class HorizontalCardsLayout : HorizontalLayoutGroupSmooth
{
    protected GameplayManager _gameplay;

    protected override void Awake()
    {
        base.Awake();
        _gameplay = GameplayManager.Instance;
    }
    protected override void LateUpdate()
    {
        if (Application.isPlaying)
        {
            _curve = _gameplay.MovementCurve;
            _duration = _gameplay.LerpTimeInLayout;
        }
        
        base.LateUpdate();
    }
}