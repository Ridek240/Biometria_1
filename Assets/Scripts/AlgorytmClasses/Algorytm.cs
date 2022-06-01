using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Algorytm
{
    public AlgorytmVarB IsActive;

    protected ComputeShader computeShader;
    protected RenderTexture renderTextureInput;
    protected RenderTexture renderTextureOutput;

    protected int kernelID;

    public void SetValues(ComputeShader _computeShader, RenderTexture input, RenderTexture output, string kernelName)
    {
        computeShader = _computeShader;
        renderTextureInput = input;
        renderTextureOutput = output;

        kernelID = computeShader.FindKernel(kernelName);
    }

    public virtual void Run(int maskRadius)
    {
        RunInit(maskRadius);

        computeShader.Dispatch(kernelID, renderTextureInput.width, renderTextureInput.height, 1);
    }

    protected void RunInit(int maskRadius)
    {
        computeShader.SetTexture(kernelID, "Input", renderTextureInput);
        computeShader.SetTexture(kernelID, "Result", renderTextureOutput);

        computeShader.SetInt("MaskRadius", maskRadius);
        computeShader.SetInts("imageSize", new int[] { renderTextureInput.width, renderTextureInput.height });
    }
    public virtual bool DidChange() => IsActive.DidChange();
}
