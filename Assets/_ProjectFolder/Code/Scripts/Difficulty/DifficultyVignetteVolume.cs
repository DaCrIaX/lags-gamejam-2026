using PrimeTween;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Volume))]
public class DifficultyVignetteVolume : MonoBehaviour
{
    [SerializeField, Min(0f)] private float _transitionDuration = 0.75f;
    [SerializeField] private Color _lowColor = Color.white;
    [SerializeField] private Color _mediumColor = new Color(0.8196079f, 0.7333333f, 0.1372549f);
    [SerializeField] private Color _highColor = new Color(0.8980392f, 0.5647059f, 0f);
    [SerializeField] private Color _rushColor = new Color(0.8941177f, 0.01960784f, 0f);

    private Volume _volume;
    private Vignette _vignette;
    private Tween _colorTween;

    private void Awake()
    {
        _volume = GetComponent<Volume>();
        EnsureVignette();
    }

    private void OnDestroy()
    {
        if (_colorTween.isAlive)
        {
            _colorTween.Stop();
        }
    }

    public void SetDifficultyLevel(int level)
    {
        Color targetColor = level switch
        {
            0 => _lowColor,
            1 => _mediumColor,
            2 => _highColor,
            3 => _rushColor,
            _ => _lowColor
        };

        SetColor(targetColor);
    }

    public void SetColor(Color targetColor)
    {
        EnsureVignette();
        if (_vignette == null)
        {
            return;
        }

        _vignette.color.overrideState = true;

        if (_colorTween.isAlive)
        {
            _colorTween.Stop();
        }

        Color startColor = _vignette.color.value;
        _colorTween = Tween.Custom(startColor, targetColor, _transitionDuration, ApplyColor);
    }

    private void ApplyColor(Color color)
    {
        if (_vignette != null)
        {
            _vignette.color.value = color;
        }
    }

    private void EnsureVignette()
    {
        if (_volume == null)
        {
            _volume = GetComponent<Volume>();
        }

        if (_volume == null)
        {
            return;
        }

        VolumeProfile profile = _volume.profile;
        if (profile == null)
        {
            return;
        }

        if (!profile.TryGet(out _vignette))
        {
            _vignette = profile.Add<Vignette>(true);
        }

        _vignette.color.overrideState = true;
    }
}
