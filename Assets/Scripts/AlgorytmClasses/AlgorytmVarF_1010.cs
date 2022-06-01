using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AlgorytmVarF_1010 : AlgorytmVar
{
    [Range(-10, 10)]
    public float Value;
    private float prevValue;

    public override bool DidChange()
    {
        bool result = Value != prevValue;
        prevValue = Value;
        return result;
    }
}
