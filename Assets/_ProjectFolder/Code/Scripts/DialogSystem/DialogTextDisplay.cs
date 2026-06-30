using System.Collections;
using TMPro;
using UnityEngine;

public class DialogTextDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private bool _clearOnAwake = true;

    private void Awake()
    {
        if (_text == null)
        {
            _text = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (_clearOnAwake)
        {
            Clear();
        }
    }

    public IEnumerator Display(string message, float charactersPerSecond)
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

    public void Clear()
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
}
