using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public static class Extensions
{
    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) + 1;
        return Arr[j % Arr.Length];
    }

    public static T Previous<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] Arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf<T>(Arr, src) - 1;
        return Arr[Math.Abs(j < 0 ? Arr.Length - 1 : j % Arr.Length)];
    }
}

public class Utility 
{
    public static int Tmp2Int(TextMeshProUGUI textMP)
    {
        if(int.TryParse(textMP.text.Replace((char)8203, ' '), out int n))
        {
            return n;
        }
        else { return 0; }
        
    }
}
