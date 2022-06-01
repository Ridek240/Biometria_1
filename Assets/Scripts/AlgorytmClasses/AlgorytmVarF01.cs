using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AlgorytmVarF01 : AlgorytmVar
{
    [Range(0, 1)]
    public float Value = 0.5f;
    private float prevValue;

    public override bool DidChange()
    {
        bool result = Value != prevValue;
        prevValue = Value;
        return result;
    }
}
