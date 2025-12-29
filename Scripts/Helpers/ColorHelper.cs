using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace NamPhuThuy.Common
{
    public static class ColorHelper
{
    public static Color WithAlpha(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }

    public static Color WithAlpha(float alpha)
    {
        return new Color(1, 1, 1, alpha);
    }

    public static Color GetGradientColor(Color color1, Color color2, float percent)
    {
        return Color.Lerp(color1, color2, percent);
    }

    public static Color GetRandomGradientColor(Color color1, Color color2)
    {
        int priority = Random.Range(0, 10);

        int randomNumber = Random.Range(0, 100);

        if (priority < 1)
        {
            randomNumber = Random.Range(20, 100);
        }
        else
        {
            randomNumber = Random.Range(0, 20);
        }

        return Color.Lerp(color1, color2, randomNumber / 100f);
    }

    public static int GetRandomNumber()
    {
        int priority = Random.Range(0, 10);

        int randomNumber = Random.Range(0, 100);

        if (priority < 1)
        {
            randomNumber = Random.Range(20, 100);
        }
        else
        {
            randomNumber = Random.Range(0, 20);
        }

        return randomNumber;
    }

    public static Color GetColorFromHex(string hex)
    {
        string validatedHex = hex.Replace("#", "");

        if (validatedHex.Length < 6)
        {
            throw new System.FormatException("Needs a string with a length of at least 6");
        }

        var r = validatedHex.Substring(0, 2);
        var g = validatedHex.Substring(2, 2);
        var b = validatedHex.Substring(4, 2);

        string alpha;

        if (validatedHex.Length >= 8)
        {
            alpha = validatedHex.Substring(6, 2);
        }
        else
        {
            alpha = "FF";
        }

        return
        new Color
        (
            int.Parse(r, NumberStyles.HexNumber) / 255f,
            int.Parse(g, NumberStyles.HexNumber) / 255f,
            int.Parse(b, NumberStyles.HexNumber) / 255f,
            int.Parse(alpha, NumberStyles.HexNumber) / 255f
        );
    }

    public static Color ToGrayscale(Color color)
    {
        float grayValue = (color.r + color.g + color.b) / 3f;

        return new Color(grayValue, grayValue, grayValue);
    }

    public static Color GetBrighterColor(Color color, float increase)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);

        v = Mathf.Clamp01(v + increase);

        return Color.HSVToRGB(h, s, v);
    }

    public static Color GetNearWhiteColor(Color color, float saturationAmount = 0.2f, float brightness = 1f)
    {
        Color.RGBToHSV(color, out float h, out float s, out float v);

        s = Mathf.Clamp01(saturationAmount);
        v = Mathf.Clamp01(brightness);

        return Color.HSVToRGB(h, s, v);
    }
    
    // ========================================================================
    // HELPER: HEX CONVERTER
    // Usage: ColorConst.FromHex("#FF0000")
    // ========================================================================
    public static Color FromHex(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out Color color))
            return color;
        
        Debug.LogWarning($"[ColorConst] Invalid Hex Code: {hex}");
        return Color.white;
    }

    #region CONTRAST
    public static void GetRandomContrastingColorPair(out Color backgroundColor, out Color textColor)
    {
        Color randomBackgroundColor = new Color(Random.value, Random.value, Random.value);

        Color.RGBToHSV(randomBackgroundColor, out float h, out float s, out float v);

        Color randomTextColor = Color.white;

        float luminance = CalculateLuminance(randomTextColor);
        if (luminance < 0.5f)
        {
            randomTextColor = AdjustColorLightness(randomTextColor, 0.8f);
        }
        else
        {
            randomTextColor = AdjustColorLightness(randomTextColor, 0.2f);
        }

        backgroundColor = randomBackgroundColor;
        textColor = randomTextColor;

        Color AdjustColorLightness(Color color, float factor)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            v = Mathf.Clamp01(factor);
            return Color.HSVToRGB(h, s, v);
        }

        float CalculateLuminance(Color color)
        {
            float r = color.r <= 0.03928f ? color.r / 12.92f : Mathf.Pow((color.r + 0.055f) / 1.055f, 2.4f);
            float g = color.g <= 0.03928f ? color.g / 12.92f : Mathf.Pow((color.g + 0.055f) / 1.055f, 2.4f);
            float b = color.b <= 0.03928f ? color.b / 12.92f : Mathf.Pow((color.b + 0.055f) / 1.055f, 2.4f);
            return 0.2126f * r + 0.7152f * g + 0.0722f * b;
        }
    }

    // public static void GetRandomContrastingColorPair(out Color backgroundColor, out Color textColor)
    // {
    //     Color randomBackgroundColor = new Color(Random.value, Random.value, Random.value);

    //     Color.RGBToHSV(randomBackgroundColor, out float h, out float s, out float v);

    //     float triadicHue1 = (h + 0.333f) % 1.0f;  // 120 degrees to the right on the color wheel
    //     float triadicHue2 = (h - 0.333f + 1.0f) % 1.0f;  // 120 degrees to the left

    //     // Create two triadic colors
    //     Color triadicColor1 = Color.HSVToRGB(triadicHue1, s, v);
    //     Color triadicColor2 = Color.HSVToRGB(triadicHue2, s, v);

    //     // Randomly select one of the triadic colors for the text
    //     Color randomTextColor = (Random.value > 0.5f) ? triadicColor1 : triadicColor2;

    //     backgroundColor = randomBackgroundColor;
    //     textColor = randomTextColor;
    // }

    public static Color GetTextColorFromBackground(Color backgroundColor)
    {
        Color.RGBToHSV(backgroundColor, out float h, out float s, out float v);

        Color randomTextColor = backgroundColor;

        float luminance = CalculateLuminance(randomTextColor);
        if (luminance < 0.5f)
        {
            randomTextColor = AdjustColorLightness(randomTextColor, 0.8f);
        }
        else
        {
            randomTextColor = AdjustColorLightness(randomTextColor, 0.2f);
        }

        return randomTextColor;

        Color AdjustColorLightness(Color color, float factor)
        {
            Color.RGBToHSV(color, out float h, out float s, out float v);
            v = Mathf.Clamp01(factor);
            return Color.HSVToRGB(h, s, v);
        }

        float CalculateLuminance(Color color)
        {
            float r = color.r <= 0.03928f ? color.r / 12.92f : Mathf.Pow((color.r + 0.055f) / 1.055f, 2.4f);
            float g = color.g <= 0.03928f ? color.g / 12.92f : Mathf.Pow((color.g + 0.055f) / 1.055f, 2.4f);
            float b = color.b <= 0.03928f ? color.b / 12.92f : Mathf.Pow((color.b + 0.055f) / 1.055f, 2.4f);
            return 0.2126f * r + 0.7152f * g + 0.0722f * b;
        }
    }
    #endregion

    #region MATH
    public static Color Multiply(this Color color, float multiplier)
    {
        Color finalColor = color * multiplier;

        finalColor.a = color.a;

        return finalColor;
    }
    #endregion
}
}
