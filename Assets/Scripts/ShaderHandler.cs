using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShaderHandler : MonoBehaviour
{
    public ComputeShader computeShader;
    public RenderTexture renderTexture;
    public RenderTexture renderTexture2;
    public Texture2D texture;

    public Camera Camera;

    [Range(0, 8)]
    public int MaskRadius = 2;
    private int prevMaskRadius = 0;

    //public bool Threshold = true;
    //private bool prevThreshold = false;

    //public bool AVG = true;
    //private bool prevAVG = false;

    //public bool NiBlack = true;
    //private bool prevNiBlack = false;

    //public bool MedianF = true;
    //private bool prevMedianF = false;

    //public bool LoGF = true;
    //private bool prevLoGF = false;

    public Threshold Threshold;
    public NiBlack NiBlack;
    public MedianF MedianF;
    public LoGF LoGF;

    //[Range(-10, 10)]
    //public float NiBlackThreshold = -0.2f;
    //private float prevNiBlackThreshold = 0;

    //[Range(0, 10)]
    //public float LoGFilter = 1.0f;
    //private float prevLoGFilter = 0;

    //private int niblack;
    //private int medianF;
    //private int logF;

    void Start()
    {
        //niblack = computeShader.FindKernel("Niblack");
        //medianF = computeShader.FindKernel("MedianF");

        renderTexture = new RenderTexture(texture.width, texture.height, 32);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        renderTexture2 = new RenderTexture(texture.width, texture.height, 32);
        renderTexture2.enableRandomWrite = true;
        renderTexture2.Create();

        Threshold.SetValues(computeShader, renderTexture, renderTexture2, "Thresholding");
        NiBlack.SetValues(computeShader, renderTexture, renderTexture2, "Niblack");
        MedianF.SetValues(computeShader, renderTexture, renderTexture2, "MedianF");
        LoGF.SetValues(computeShader, renderTexture, renderTexture2, "LoGF");
        Graphics.Blit(texture, renderTexture);
    }

    private void Niblack()
    {
        Camera.Render();
        //NiBlack.SetValues(computeShader, renderTexture, renderTexture2, "Niblack");
        NiBlack.Run(MaskRadius);
        //computeShader.SetTexture(niblack, "Input", renderTexture);
        //computeShader.SetTexture(niblack, "Result", renderTexture2);

        //computeShader.SetInt("MaskRadius", MaskRadius);
        //computeShader.SetFloat("NiblackVar", NiBlack.Threshold.Value);
        //computeShader.SetInts("imageSize", new int[] { renderTexture.width, renderTexture.height });

        //computeShader.Dispatch(niblack, renderTexture.width, renderTexture.height, 1);
    }

    private void Medianf()
    {
        Camera.Render();
        MedianF.Run(MaskRadius);
        //computeShader.SetTexture(medianF, "Input", renderTexture);
        //computeShader.SetTexture(medianF, "Result", renderTexture2);

        //computeShader.SetInt("MaskRadius", MaskRadius);
        //computeShader.SetInts("imageSize", new int[] { renderTexture.width, renderTexture.height });

        //computeShader.Dispatch(medianF, renderTexture.width, renderTexture.height, 1);
    }

    private void LoGf()
    {
        Camera.Render();
        LoGF.Run(MaskRadius);
        //computeShader.SetTexture(medianF, "Input", renderTexture);
        //computeShader.SetTexture(medianF, "Result", renderTexture2);

        //computeShader.SetInt("MaskRadius", MaskRadius);
        //computeShader.SetInts("imageSize", new int[] { renderTexture.width, renderTexture.height });

        //computeShader.Dispatch(medianF, renderTexture.width, renderTexture.height, 1);
    }

    //private void Thresold()
    //{
    //    //Camera.Render();
    //    computeShader.SetTexture(threshold, "Input", renderTexture);
    //    computeShader.SetTexture(threshold, "Result", renderTexture2);

    //    computeShader.SetBool("avg", AVG);
    //    computeShader.SetFloat("thresholdAVG", ThresholdAVG);
    //    computeShader.SetFloat("thresholdR", ThresholdR);
    //    computeShader.SetFloat("thresholdG", ThresholdG);
    //    computeShader.SetFloat("thresholdB", ThresholdB);

    //    computeShader.Dispatch(threshold, renderTexture.width / 8, renderTexture.height / 8, 1);
    //}

    private void Update()
    {
        if (Threshold.DidChange() ||
            NiBlack.DidChange() ||
            MedianF.DidChange() ||
            LoGF.DidChange() ||
            prevMaskRadius != MaskRadius)
        {
            prevMaskRadius = MaskRadius;

            Graphics.Blit(renderTexture, renderTexture2);

            if (Threshold.IsActive.Value) Threshold.Run();
            if (NiBlack.IsActive.Value) Niblack();
            if (MedianF.IsActive.Value) Medianf();
            if (LoGF.IsActive.Value) LoGf();
        }
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), renderTexture2);
    }
}
