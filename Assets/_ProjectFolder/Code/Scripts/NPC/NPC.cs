using UnityEngine;
using UnityEngine.Splines;

public class NPC : MonoBehaviour
{
    [SerializeField] private SplineContainer _spline;
    [SerializeField] private float _npcSpeed = 1f;

    [SerializeField] private Material _material;
    [SerializeField] private Sprite[] _images;

    public float Speed => _npcSpeed;
    private Transform _transform;

    private void Awake() => _transform = transform;

    public void SetSkinRandom() => _material.mainTexture = _images[Random.Range(0, _images.Length)].texture;
    public void SetOnSpline(float time) => _transform.position = _spline.EvaluatePosition(time);
}