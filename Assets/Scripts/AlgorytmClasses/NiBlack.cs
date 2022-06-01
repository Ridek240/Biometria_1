using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NiBlack : Algorytm
{
    public AlgorytmVarF_1010 Threshold;

    public override void Run(int maskRadius)
    {
        base.RunInit(maskRadius);

        computeShader.SetFloat("NiblackVar", Threshold.Value);
        computeShader.Dispatch(kernelID, renderTextureInput.width, renderTextureInput.height, 1);
    }

    public override bool DidChange()
    {
        return base.DidChange() ||
            Threshold.DidChange();
    }
}
