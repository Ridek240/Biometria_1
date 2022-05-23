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

    public bool Threshold = true;
    private bool prevThreshold = false;

    public bool AVG = true;
    private bool prevAVG = false;

    public bool NiBlack = true;
    private bool prevNiBlack = false;

    public bool MedianF = true;
    private bool prevMedianF = false;

    [Range(0, 1)]
    public float ThresholdAVG = 0.5f;
    private float prevThresholdAVG = 0;

    [Range(0, 1)]
    public float ThresholdR = 0.5f;
    private float prevThresholdR = 0;

    [Range(0, 1)]
    public float ThresholdG = 0.5f;
    private float prevThresholdG = 0;

    [Range(0, 1)]
    public float ThresholdB = 0.5f;
    private float prevThresholdB = 0;

    [Range(-10, 10)]
    public float NiBlackThreshold = -0.2f;
    private float prevNiBlackThreshold = 0;

    private int threshold;
    private int niblack;
    private int medianF;

    void Start()
    {
        threshold = computeShader.FindKernel("CSMain");
        niblack = computeShader.FindKernel("Niblack");
        medianF = computeShader.FindKernel("MedianF");

        renderTexture = new RenderTexture(texture.width, texture.height, 32);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        renderTexture2 = new RenderTexture(texture.width, texture.height, 32);
        renderTexture2.enableRandomWrite = true;
        renderTexture2.Create();

        Graphics.Blit(texture, renderTexture);
    }

    private void Niblack()
    {
        Camera.Render();
        computeShader.SetTexture(niblack, "Input", renderTexture);
        computeShader.SetTexture(niblack, "Result", renderTexture2);

        computeShader.SetInt("MaskRadius", MaskRadius);
        computeShader.SetFloat("NiblackVar", NiBlackThreshold);
        computeShader.SetInts("imageSize", new int[] { renderTexture.width , renderTexture.height });

        computeShader.Dispatch(niblack, renderTexture.width, renderTexture.height, 1);
    }

    private void Medianf()
    {
        Camera.Render();
        computeShader.SetTexture(medianF, "Input", renderTexture);
        computeShader.SetTexture(medianF, "Result", renderTexture2);

        computeShader.SetInt("MaskRadius", MaskRadius);
        computeShader.SetInts("imageSize", new int[] { renderTexture.width, renderTexture.height });

        computeShader.Dispatch(medianF, renderTexture.width, renderTexture.height, 1);
    }

    private void Thresold()
    {
        //Camera.Render();
        computeShader.SetTexture(threshold, "Input", renderTexture);
        computeShader.SetTexture(threshold, "Result", renderTexture2);

        computeShader.SetBool("avg", AVG);
        computeShader.SetFloat("thresholdAVG", ThresholdAVG);
        computeShader.SetFloat("thresholdR", ThresholdR);
        computeShader.SetFloat("thresholdG", ThresholdG);
        computeShader.SetFloat("thresholdB", ThresholdB);

        computeShader.Dispatch(threshold, renderTexture.width / 8, renderTexture.height / 8, 1);
    }

    private void Update()
    {
        if (prevThresholdAVG != ThresholdAVG ||
            prevThresholdR != ThresholdR ||
            prevThresholdG != ThresholdG ||
            prevThresholdB != ThresholdB ||
            prevNiBlackThreshold != NiBlackThreshold ||
            prevMaskRadius != MaskRadius ||
            prevThreshold != Threshold ||
            prevAVG != AVG ||
            prevNiBlack != NiBlack ||
            prevMedianF != MedianF)
        {
            prevThresholdAVG = ThresholdAVG;
            prevThresholdR = ThresholdR;
            prevThresholdG = ThresholdG;
            prevThresholdB = ThresholdB;
            prevNiBlackThreshold = NiBlackThreshold;
            prevMaskRadius = MaskRadius;
            prevThreshold = Threshold;
            prevAVG = AVG;
            prevNiBlack = NiBlack;
            prevMedianF = MedianF;

            Graphics.Blit(renderTexture, renderTexture2);

            if (Threshold) Thresold();
            if (NiBlack) Niblack();
            if (MedianF) Medianf();
        }
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), renderTexture2);
    }
}
