using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Watenk{
public static class GridUtility{
    public static bool IsInBounds(Vector2Int pos, Vector2Int boundPos1, Vector2Int boundPos2){
        if (pos.x < boundPos1.x || pos.x >= boundPos2.x) { return false; }
        if (pos.y < boundPos1.y || pos.y >= boundPos2.y) { return false; }
        return true;
    }

    public static bool IsInBounds(Vector2 pos, Vector2 boundPos1, Vector2 boundPos2){
        if (pos.x < boundPos1.x || pos.x >= boundPos2.x) { return false; }
        if (pos.y < boundPos1.y || pos.y >= boundPos2.y) { return false; }
        return true;
    }
    
    public static bool IsInBounds(Vector3 pos, Vector3 boundPos1, Vector3 boundPos2){
        if (pos.x < boundPos1.x || pos.x >= boundPos2.x) { return false; }
        if (pos.y < boundPos1.y || pos.y >= boundPos2.y) { return false; }
        if (pos.z < boundPos1.z || pos.z >= boundPos2.z) { return false; }
        return true;
    }
}

public static class MathUtility{
    public static float Map(float value, float fromSource, float toSource, float fromTarget, float toTarget){
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }
}

public static class ColorUtility{
    public static Color HSLToRGB(int hue, int saturation, int lightness){
        // Normalize hue, saturation, and lightness values
        float h = (float)hue / 360f; // Hue is usually defined in the range [0, 360]
        float s = Mathf.Clamp01((float)saturation / 100f); // Saturation is usually defined in the range [0, 100]
        float l = Mathf.Clamp01((float)lightness / 100f); // Lightness is usually defined in the range [0, 100]

        float c = (1 - Mathf.Abs(2 * l - 1)) * s;
        float x = c * (1 - Mathf.Abs((h * 6) % 2 - 1));
        float m = l - c / 2;

        float r, g, b;
        if (h < 1f / 6f){
            r = c;
            g = x;
            b = 0;
        }
        else if (h < 1f / 3f){
            r = x;
            g = c;
            b = 0;
        }
        else if (h < 0.5f){
            r = 0;
            g = c;
            b = x;
        }
        else if (h < 2f / 3f){
            r = 0;
            g = x;
            b = c;
        }
        else if (h < 5f / 6f){
            r = x;
            g = 0;
            b = c;
        }
        else{
            r = c;
            g = 0;
            b = x;
        }

        return new Color(r + m, g + m, b + m);
    }

    public static Vector3 RGBToHSL(Color color)
    {
        float r = color.r;
        float g = color.g;
        float b = color.b;

        float max = Mathf.Max(r, Mathf.Max(g, b));
        float min = Mathf.Min(r, Mathf.Min(g, b));
        float h, s, l;

        // Calculate lightness
        l = (max + min) / 2f;

        if (max == min){
            // Achromatic case (no hue)
            h = 0f;
            s = 0f;
        }
        else{
            float d = max - min;

            // Calculate saturation
            s = l > 0.5f ? d / (2f - max - min) : d / (max + min);

            // Calculate hue
            if (max == r)
                h = (g - b) / d + (g < b ? 6f : 0f);
            else if (max == g)
                h = (b - r) / d + 2f;
            else
                h = (r - g) / d + 4f;

            h /= 6f;
        }

        return new Vector3(h * 360f, s * 100f, l * 100f);
    }
}
}