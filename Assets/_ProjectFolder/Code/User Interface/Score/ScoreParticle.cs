using TMPro;

namespace UnityEngine.Pool
{
    public class ScoreParticle : PoolObjectBehaviour
    {
        [SerializeField] private TextMeshProUGUI _text;

        public void SetText(int score)
        {
            if (score > 0)
                _text.SetText($"+{score}");
            else
                _text.SetText($"{score}");
        }
    }
}