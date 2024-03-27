using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Watenk;

namespace Watenk{
public static class GridUtility{

    public static bool IsInBounds(Vector2Short pos, Vector2Short boundPos1, Vector2Short boundPos2){
        if (pos.x < boundPos1.x || pos.x >= boundPos2.x) { return false; }
        if (pos.y < boundPos1.y || pos.y >= boundPos2.y) { return false; }
        return true;
    }

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

    public static float SetDecimals(float value, int decimals){
        return Mathf.Round(value * (10f * decimals)) / (10f * decimals);
    }

    public static bool IsInBounds(float value, float bound1, float bound2){
        if (value < bound1 || value >= bound2) { return false; }
        if (value < bound1 || value >= bound2) { return false; }
        return true;
    }

    public static bool IsInBounds(int value, int bound1, int bound2){
        if (value < bound1 || value >= bound2) { return false; }
        if (value < bound1 || value >= bound2) { return false; }
        return true;
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

[Serializable]
public class Vector2Short 
{
    public static readonly Vector2Short Zero = new Vector2Short(0, 0);
    public static readonly Vector2Short One = new Vector2Short(1, 1);
    public static readonly Vector2Short Up = new Vector2Short(0, 1);
    public static readonly Vector2Short Down = new Vector2Short(0, -1);
    public static readonly Vector2Short Left = new Vector2Short(-1, 0);
    public static readonly Vector2Short Right = new Vector2Short(1, 0);

    public short x;
    public short y;

    //------------------------------------------
    // Constructors

    public Vector2Short(){
        x = 0;
        y = 0;
    }

    public Vector2Short(short x, short y){
        this.x = x;
        this.y = y;
    }

    public Vector2Short(int x, int y){
        if (x < (int)short.MinValue || x >= (int)short.MaxValue) Debug.LogWarning("Int " + x + " is out of short range");
        if (y < (int)short.MinValue || y >= (int)short.MaxValue) Debug.LogWarning("Int " + y + " is out of short range");
        this.x = (short)x;
        this.y = (short)y;
    }

    public Vector2Short(float x, float y){
        if (x < (float)short.MinValue || x >= (float)short.MaxValue) Debug.LogWarning("Float " + x + " is out of short range");
        if (y < (float)short.MinValue || y >= (float)short.MaxValue) Debug.LogWarning("Float " + y + " is out of short range");
        this.x = (short)x;
        this.y = (short)y;
    }

    public Vector2Short(Vector2Int vector2Int){
        if (vector2Int.x < (int)short.MinValue || vector2Int.x >= (int)short.MaxValue) Debug.LogWarning("Int " + x + " is out of short range");
        if (vector2Int.y < (int)short.MinValue || vector2Int.y >= (int)short.MaxValue) Debug.LogWarning("Int " + y + " is out of short range");
        this.x = (short)vector2Int.x;
        this.y = (short)vector2Int.y;
    }

    // Operator Overloads
    public static Vector2Short operator +(Vector2Short pos1, Vector2Short pos2){
        return new Vector2Short(pos1.x + pos2.x, pos1.y + pos2.y);
    }

    public static Vector2Short operator -(Vector2Short pos1, Vector2Short pos2){
        return new Vector2Short(pos1.x - pos2.x, pos1.y - pos2.y);
    }

    public static Vector2Short operator *(Vector2Short pos1, Vector2Short pos2){
        return new Vector2Short(pos1.x * pos2.x, pos1.y * pos2.y);
    }

    public static Vector2Short operator /(Vector2Short pos1, Vector2Short pos2){
        return new Vector2Short(pos1.x / pos2.x, pos1.y / pos2.y);
    }

    // System things
    public override string ToString(){
        return string.Format("{0}, {1}", x, y);
    }

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType()) return false;
        

        Vector2Short other = (Vector2Short)obj;
        return x.Equals(other.x) && y.Equals(other.y);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + x.GetHashCode();
            hash = hash * 23 + y.GetHashCode();
            return hash;
        }
    }
}
