using UnityEngine;

public static class TransformExtension
{
    public static void PositionX(this Transform transform, float value)
    {
        Vector3 pos = transform.position;
        pos.x = value;
        transform.position = pos;
    }
    public static float PositionX(this Transform transform) => transform.position.x;
    public static void LocalPositionX(this Transform transform, float value)
    {
        Vector3 pos = transform.localPosition;
        pos.x = value;
        transform.localPosition = pos;
    }
    public static float LocalPositionX(this Transform transform) => transform.localPosition.x;
    public static void ClampPositionX(this Transform transform, float min, float max)
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, min, max);
        transform.position = pos;
    }
    public static void ClampLocalPositionX(this Transform transform, float min, float max)
    {
        Vector3 pos = transform.localPosition;
        pos.x = Mathf.Clamp(pos.x, min, max);
        transform.localPosition = pos;
    }

    public static void PositionY(this Transform transform, float value)
    {
        Vector3 pos = transform.position;
        pos.y = value;
        transform.position = pos;
    }
    public static float PositionY(this Transform transform) => transform.position.y;
    public static void LocalPositionY(this Transform transform, float value)
    {
        Vector3 pos = transform.localPosition;
        pos.y = value;
        transform.localPosition = pos;
    }
    public static float LocalPositionY(this Transform transform) => transform.localPosition.y;
    public static void ClampPositionY(this Transform transform, float min, float max)
    {
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, min, max);
        transform.position = pos;
    }
    public static void ClampLocalPositionY(this Transform transform, float min, float max)
    {
        Vector3 pos = transform.localPosition;
        pos.y = Mathf.Clamp(pos.y, min, max);
        transform.localPosition = pos;
    }

    public static void PositionZ(this Transform transform, float value)
    {
        Vector3 pos = transform.position;
        pos.z = value;
        transform.position = pos;
    }
    public static float PositionZ(this Transform transform) => transform.position.z;
    public static void LocalPositionZ(this Transform transform, float value)
    {
        Vector3 pos = transform.localPosition;
        pos.z = value;
        transform.localPosition = pos;
    }
    public static float LocalPositionZ(this Transform transform) => transform.localPosition.z;
    public static void ClampPositionZ(this Transform transform, float min, float max)
    {
        Vector3 pos = transform.position;
        pos.z = Mathf.Clamp(pos.z, min, max);
        transform.position = pos;
    }
    public static void ClampLocalPositionZ(this Transform transform, float min, float max)
    {
        Vector3 pos = transform.localPosition;
        pos.z = Mathf.Clamp(pos.z, min, max);
        transform.localPosition = pos;
    }
}