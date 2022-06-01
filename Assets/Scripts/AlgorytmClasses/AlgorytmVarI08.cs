using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgorytmVarI08 : AlgorytmVar
{
    [Range(0, 8)]
    public int Value;
    private int prevValue;

    public override bool DidChange()
    {
        bool result = Value != prevValue;
        prevValue = Value;
        return result;
    }
}
