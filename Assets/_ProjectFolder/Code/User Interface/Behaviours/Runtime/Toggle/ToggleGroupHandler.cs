namespace UnityEngine.UI
{
    [AddComponentMenu("UI (Canvas)/Toggle Group Handler")]
    public class ToggleGroupHandler : ToggleGroup
    {
        public void SetAllTogglesOff() => SetAllTogglesOff(true);
        public void SetAllTogglesOffWithoutNotify() => SetAllTogglesOff(false);
    }
}