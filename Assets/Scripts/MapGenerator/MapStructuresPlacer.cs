using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapStructuresPlacer : MonoBehaviour
{
    MapGenerator mapGenerator;

    [Header("Florest Structures Locations")]
    [SerializeField]
    float startAreaRadius = 20;

    [SerializeField]
    Vector3 startPoint = new Vector3(0.0f, 0.0f, 0.0f);

    [Header("Purple Mangrove Structures Locations")]
    [SerializeField]
    float lakeSize = 10f;

    [SerializeField]
    Vector3 lakeLocal = new Vector3(0.0f, 0.0f, 0.0f);

    int mapLimit;
    int nBiomes;
    int frontier;
    float radio2;

    public float StartAreaRadius { get => startAreaRadius; private set => startAreaRadius = value; }
    public Vector3 StartPoint { get => startPoint; private set => startPoint = value; }
    public float LakeSize { get => lakeSize; private set => lakeSize = value; }
    public Vector3 LakeLocal { get => lakeLocal; private set => lakeLocal = value; }

    void Start()
    {
        mapGenerator = GetComponent<MapGenerator>();
        mapLimit = mapGenerator.MapLimit;
        nBiomes = mapGenerator.NBiomes;
        frontier = mapGenerator.Frontier;


        // Aqui coloca os valores aleatórios do que vai acontecer no pântano roxo
        SetLakeLocalization();
    }

    private void SetLakeLocalization()
    {
        var radio = UnityEngine.Random.Range(MangroveBegin(mapLimit, frontier + lakeSize), MangroveEnd(mapLimit, frontier - lakeSize));
        var angle = UnityEngine.Random.Range(0.0f, 360.0f);
        var x = Mathf.RoundToInt(radio * Mathf.Sin(angle * Mathf.Deg2Rad));
        var z = Mathf.RoundToInt(radio * Mathf.Cos(angle * Mathf.Deg2Rad));
        lakeLocal = new Vector3(x, 0, z);
    }

    float MangroveBegin(float mapLimit, float frontier)
    {

        return (1 * mapLimit / 4) + frontier;
    }
    float MangroveEnd(float mapLimit, float frontier)
    {

        return (2 * mapLimit / 4) - frontier;
    }
}
