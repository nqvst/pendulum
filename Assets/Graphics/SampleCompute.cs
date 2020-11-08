using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleCompute : MonoBehaviour
{
    [SerializeField] ComputeShader shader;
    [SerializeField] RenderTexture result;

    private int kernel;

    void Start()
    {
        kernel = shader.FindKernel("CSMain");
    }

    void FixedUpdate()
    {
        result = new RenderTexture(512, 512, 24);
        result.enableRandomWrite = true;
        result.Create();

        shader.SetTexture(kernel, "Result", result);
        shader.Dispatch(kernel, 512 / 8, 512 / 8, 1);
    }
}
