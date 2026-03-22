using UnityEngine;
using UnityEngine.UI;

public static class UIExtension
{
    public static void SetImage(this Image image, Sprite sprite) => image.sprite = sprite;
    public static void SetMaterial(this Image image, Material material) => image.material = material;
}