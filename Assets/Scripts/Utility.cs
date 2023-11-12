using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
