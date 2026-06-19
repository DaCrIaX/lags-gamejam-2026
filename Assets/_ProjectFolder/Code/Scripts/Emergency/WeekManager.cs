using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class WeekManager : MonoBehaviour
{
    public TextMeshProUGUI dayText;

    private int dayAmount;

    public UnityEvent onCompleteWeek;
    public UnityEvent onCompleteDay;

    private void Check()
    {
        if (dayAmount == 8)
        {
            onCompleteWeek?.Invoke();
            dayAmount = 0;
            dayText.text = dayAmount.ToString();
        }
        else
        {
            onCompleteDay?.Invoke();
        }
    }

    public void IncrementDay()
    {
        dayAmount++;
        dayText.text = dayAmount.ToString();
        Check();
    }
}
