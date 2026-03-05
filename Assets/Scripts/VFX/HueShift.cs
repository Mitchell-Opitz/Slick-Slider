using UnityEngine;

public static class HueShift
{
    public static Color Shift(Color color, float degrees)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);
        h = Mathf.Repeat(h + degrees / 360f, 1f);
        return Color.HSVToRGB(h, s, v);
    }
}