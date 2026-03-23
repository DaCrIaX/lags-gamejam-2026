using System;

namespace UnityEngine.EventSystems
{
    [Serializable]
    public struct LayoutHandler
    {
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private float min, max;

        public float Evaluate(float time) => Mathf.LerpUnclamped(min, max, curve.Evaluate(time));
    }
}