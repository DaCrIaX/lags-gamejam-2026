using System;

namespace UnityEngine.Animations
{
    public interface ITweenCallback
    {
        event Action<bool> onTweenStatusChanged, onTweenCompleted;
        event Action onEnabled, onDisabled;
        event Action onCompleted;

        void OnComplete();
    }
}