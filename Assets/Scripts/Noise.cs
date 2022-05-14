using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Noise : MonoBehaviour
{
    // values for FractalBrownianMotion
    
    float smooth = 0.01f; 
    static int octaves = 4;
    static float persistence = 0.5f;
    [HideInInspector]
    public int maxHeight = 150;
    [HideInInspector]
    public int stoneHeight = -30;
    [HideInInspector]
    public int sandHeight = -70;
    [HideInInspector]
    public float frequencyOffset = 1;
    [HideInInspector]
    public float amplitudeOffset = 1;
    [HideInInspector]
    private float maxValueOffset = 0.1f;
    [HideInInspector]
    public float mapOffset = 32000f;
    [HideInInspector]
    private static Noise instance;

    /// <summary>
    /// the singleton instance
    /// </summary>
    public static Noise Instance
    {
        get { if (instance == null) instance = GameObject.FindObjectOfType<Noise>(); return instance; }
    }

    public void Awake()
    {
        instance = this;
    }

    
    public float Map(float min, float max, float originalMin, float oringalmax, float value)
    {
        return Mathf.Lerp(min, max, Mathf.InverseLerp(originalMin, oringalmax, value));
    }
    /// <summary>
    /// generates the heights of the cubes from a x and z value
    /// </summary>

    public int GetSandHeight(float x, float z)
    {
        return (int) Map(0, maxHeight + sandHeight, 0, 1, FractalBrownianMotion(x * smooth * 2, z * smooth * 2, octaves + 1, persistence));
    }

    public int GetStoneHeight(float x, float z)
    {
        return (int) Map(0, maxHeight + stoneHeight, 0, 1, FractalBrownianMotion(x * smooth * 2, z * smooth * 2, octaves + 1, persistence));
    }

    public int GetDirtHeight(float x, float z)
    {
        return (int) Map(0, maxHeight, 0, 1, FractalBrownianMotion(x * smooth, z * smooth, octaves, persistence));        
    }

    /// <summary>
    /// function for the fractalBrownian Motion
    /// </summary>
    /// <param name="x">first imput for perlin noise</param>
    /// <param name="z">second input for perlin noise</param>
    /// <param name="octaves">adds a perlin noise wave</param>
    /// <param name="persistence">how strong the perlin noise waves are</param>
    public float FractalBrownianMotion(float x, float z, int octaves, float persistence)
    {
        float total = 0;
        float frequency = frequencyOffset;
        float amplitude = amplitudeOffset;
        float maxValue = maxValueOffset;
        float offset = mapOffset;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise((x+offset) * frequency, (z+offset) * frequency) * amplitude;
            maxValue += amplitude;
            amplitude *= persistence;
            frequency *= 2;
        }
        return total / maxValue;
    }
    /// <summary>
    /// Fractal Brownian Motion including a 3rd value
    /// </summary>
    public float FractalBrownianMotion3D(float x, float y, float z)
    {
        int oct = 3;
        float per = 0.5f;

        float XY = FractalBrownianMotion(x * smooth * 10, y * smooth * 10, oct, per);
        float XZ = FractalBrownianMotion(x * smooth * 10, z * smooth * 10, oct, per);
        float YZ = FractalBrownianMotion(y * smooth * 10, z * smooth * 10, oct, per);      
        float YX = FractalBrownianMotion(y * smooth * 10, x * smooth * 10, oct, per);
        float ZY = FractalBrownianMotion(z * smooth * 10, y * smooth * 10, oct, per);
        float ZX = FractalBrownianMotion(z * smooth * 10, x * smooth * 10, oct, per);

        return (XY + YZ + XZ + YX + ZY + ZX) / 6f;
    }
}
