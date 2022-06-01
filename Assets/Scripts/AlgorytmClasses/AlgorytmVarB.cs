using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AlgorytmVarB : AlgorytmVar
{
    public bool Value = false;
    private bool prevValue;

    public override bool DidChange()
    {
        bool result = Value != prevValue;
        prevValue = Value;
        return result;
    }
}
