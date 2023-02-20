using UnityEngine;
using Random = UnityEngine.Random;

public class MapStructuresPlacer : MonoBehaviour
{
    MapGenerator mapGenerator;

    [Header("Florest Structures Locations")]
    [SerializeField]
    float startAreaRadius = 5;

    [SerializeField]
    Vector3 startPoint = new Vector3(0.0f, 0.0f, 0.0f);

    [Header("Purple Mangrove Structures Locations")]
    [SerializeField]
    float lakeSize = 10f;

    [SerializeField]
    Vector3 lakeLocal = new Vector3(0.0f, 0.0f, 0.0f);

    [Header("Storehouse Placer")]
    [SerializeField]
    float storehouseSize = 10.0f;

    [SerializeField]
    Vector3 referenceStartPoint = new Vector3(0.0f, 0.0f, 0.0f);
    [SerializeField]
    Vector3 storehouseLocation;

    [SerializeField]
    float distanceFromStartPoint = 100.0f;

    [SerializeField]
    GameObject StoreHouseGameObject = null;

    [SerializeField]
    int chainsBetweenStartPointAndStoreHouse = 1;

    [SerializeField]
    GameObject ChaingGameObject = null;

    int mapLimit;

    int nBiomes;
    
    int frontier;
    
    float radio2;

    public float StartAreaRadius { get => startAreaRadius; private set => startAreaRadius = value; }
    
    public Vector3 StartPoint { get => startPoint; private set => startPoint = value; }

    public float LakeSize { get => lakeSize; private set => lakeSize = value; }

    public Vector3 LakeLocal { get => lakeLocal; private set => lakeLocal = value; }

    public float StorehouseSize { get => storehouseSize; private set => storehouseSize = value; }

    public Vector3 StorehouseLocation { get => storehouseLocation; private set => storehouseLocation = value; }

    void Start()
    {
        mapGenerator = GetComponent<MapGenerator>();

        mapLimit = mapGenerator.MapLimit;
        
        nBiomes = mapGenerator.NBiomes;
        
        frontier = mapGenerator.Frontier;

        SetLakeLocalization();

        SetStoreHouseLocation();
    }

    private void SetStoreHouseLocation()
    {
        var angle = Random.Range(0.0f, 360.0f);

        var x = Mathf.RoundToInt(distanceFromStartPoint * Mathf.Sin(angle * Mathf.Deg2Rad));

        var z = Mathf.RoundToInt(distanceFromStartPoint * Mathf.Cos(angle * Mathf.Deg2Rad));

        StorehouseLocation = new Vector3(x, 0, z);

        var airValue = mapGenerator.GetAirValue();

        int storehouseLevel = 0;

        for (int i = 100; i > -100; i--)
        {
            if (mapGenerator.Generation((int)StorehouseLocation.x, i, (int)StorehouseLocation.z) != airValue)
            {
                storehouseLevel = i;
                break;
            }
        }

        var storeHouse = Instantiate(StoreHouseGameObject, storehouseLocation, Quaternion.identity);

        storeHouse.transform.LookAt(referenceStartPoint);

        StorehouseLocation = new Vector3(x, storehouseLevel, z);

        storeHouse.transform.position = StorehouseLocation;

        var distanceBetweenChains = distanceFromStartPoint / (chainsBetweenStartPointAndStoreHouse + 1);

        Vector3 chainSpawnDirection = (storehouseLocation - referenceStartPoint).normalized;

        for (int i = 1; i <= chainsBetweenStartPointAndStoreHouse; i++)
        {
            Instantiate(ChaingGameObject, chainSpawnDirection * i * distanceBetweenChains, Quaternion.identity);
        }

    }

    void SetLakeLocalization()
    {
        var radio = Random.Range(MangroveBegin(mapLimit, frontier + lakeSize), 
        MangroveEnd(mapLimit, frontier - lakeSize));
        
        var angle = Random.Range(0.0f, 360.0f);
        
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