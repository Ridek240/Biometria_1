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

    public bool AVG = true;
    private bool prevAVG = false;

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

    private int kernelId;

    void Start()
    {
        kernelId = computeShader.FindKernel("CSMain");
        renderTexture = new RenderTexture(texture.width, texture.height, 32);
        renderTexture.enableRandomWrite = true;
        renderTexture.Create();

        renderTexture2 = new RenderTexture(texture.width, texture.height, 32);
        renderTexture2.enableRandomWrite = true;
        renderTexture2.Create();

        Graphics.Blit(texture, renderTexture);
    }

    private void Thresold()
    {
        Camera.Render();
        computeShader.SetTexture(kernelId, "Input", renderTexture);
        computeShader.SetTexture(kernelId, "Result", renderTexture2);

        computeShader.SetBool("avg", AVG);
        computeShader.SetFloat("thresholdAVG", ThresholdAVG);
        computeShader.SetFloat("thresholdR", ThresholdR);
        computeShader.SetFloat("thresholdG", ThresholdG);
        computeShader.SetFloat("thresholdB", ThresholdB);

        computeShader.Dispatch(kernelId, renderTexture.width / 8, renderTexture.height / 8, 1);
    }

    private void Update()
    {
        if (prevThresholdAVG != ThresholdAVG ||
            prevThresholdR != ThresholdR ||
            prevThresholdG != ThresholdG ||
            prevThresholdB != ThresholdB ||
            prevAVG != AVG)
        {
            prevThresholdAVG = ThresholdAVG;
            prevThresholdR = ThresholdR;
            prevThresholdG = ThresholdG;
            prevThresholdB = ThresholdB;
            prevAVG = AVG;
            Thresold();
        }
        //Graphics.Blit(renderTexture2, Camera.targetTexture);
    }

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, 256, 256), renderTexture2);
    }
}
