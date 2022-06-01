using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Threshold : Algorytm
{
    public AlgorytmVarF01 RedThreshold;
    public AlgorytmVarF01 GreenThreshold;
    public AlgorytmVarF01 BlueThreshold;

    public AlgorytmVarB IsAVG;

    public AlgorytmVarF01 AVGThreshold;

    public override void Run(int maskRadius = 0)
    {
        base.RunInit(0);

        computeShader.SetBool("avg", IsAVG.Value);
        computeShader.SetFloat("thresholdAVG", AVGThreshold.Value);
        computeShader.SetFloat("thresholdR", RedThreshold.Value);
        computeShader.SetFloat("thresholdG", GreenThreshold.Value);
        computeShader.SetFloat("thresholdB", BlueThreshold.Value);

        computeShader.Dispatch(kernelID, renderTextureInput.width / 8, renderTextureInput.height / 8, 1);
    }

    public override bool DidChange()
    {
        return base.DidChange() ||
            IsAVG.DidChange() ||
            AVGThreshold.DidChange() ||
            RedThreshold.DidChange() ||
            GreenThreshold.DidChange() ||
            BlueThreshold.DidChange();
    }
}
