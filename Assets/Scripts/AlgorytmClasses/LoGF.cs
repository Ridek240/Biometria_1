using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LoGF : Algorytm
{
    public AlgorytmVarB IsGlobalSTD;
    public AlgorytmVarF01 StandardDeviation;

    public override void Run(int maskRadius)
    {
        base.RunInit(maskRadius);

        computeShader.SetFloat("LoGFVar", StandardDeviation.Value);
        computeShader.SetBool("GlobalSTD", IsGlobalSTD.Value);
        ComputeBuffer tmp = SetMatrixBuffer(maskRadius);
        computeShader.Dispatch(kernelID, renderTextureInput.width, renderTextureInput.height, 1);
        tmp.Release();
    }

    public override bool DidChange()
    {
        return base.DidChange() ||
            StandardDeviation.DidChange() ||
            IsGlobalSTD.DidChange();
    }

    public ComputeBuffer SetMatrixBuffer(int maskRadius)
    {
        float[] matrix = CreateMatrix(maskRadius);
        ComputeBuffer buffer = new ComputeBuffer(matrix.Length, sizeof(float));
        buffer.SetData(matrix);
        computeShader.SetBuffer(kernelID, "LoGFMask", buffer);
        return buffer;
    }

    public float[] CreateMatrix(int maskRadius)
    {
        int matrixLength = (maskRadius * 2 + 1);
        float sd = StandardDeviation.Value != 0 ? StandardDeviation.Value : 0.1f;
        float[] result = new float[matrixLength * matrixLength];


        for (int x = -maskRadius; x <= maskRadius; x++)
        {
            for (int y = -maskRadius; y <= maskRadius; y++)
            {
                float tmp = (x * x + y * y) / (2 * sd * sd);
                result[(x + maskRadius) + (y + maskRadius) * matrixLength] =
                    (1 / (Mathf.PI * Mathf.Pow(sd, 4)))
                    * (1 - tmp)
                    * Mathf.Exp(-tmp);
            }
        }

        return result;
    }
}
