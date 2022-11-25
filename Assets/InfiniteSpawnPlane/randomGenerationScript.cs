using UnityEngine;

public class randomGenerationScript : MonoBehaviour 
{
	[Header("Player Game Object")]
	[Tooltip("Drag and drop your in-scene player game object here; this will allow the spawner to follow your player!")]
	public GameObject playerToFollow;

	[Header("Spawn Controls")]
	[Tooltip("Ensure you make each of the arrays below the same size. For example, if you have 2 enemy prefabs you want to spawn make sure there are 2 spawn timers.")]
	public GameObject[] aiPrefabsToSpawn;

	[Tooltip("Set the time for the period in seconds, where the period is the time between each random spawn. Ensure that the Size value of this array matches the Size value of Prefabs To Spawn.")]
	public float[] spawnTimer;

	[Tooltip("Set the 'Ceiling' for spawns; they won't spawn above this value in the world Y-Axis.")]
	public float[] spawnCeiling;

	[Tooltip("Set the 'Floor' for spawns; they won't spawn below this value in the world Y-Axis.")]
	public float[] spawnFloor;

	[Header("Spawn Parameters")]
	[Tooltip("Set how high off the ground the instantiated prefab will be created. Important if you find your prefabs halfway in the ground. Must be same size as the Spawn Controls arrays. Allows varying sized spawns.")]
	public float[] InstantiateHeight;

	[Header("Time of Previous Spawn (Debugging Aid)")]
	[Tooltip("Use this to see when the last object spawned. Great for ensuring that you are at least spawning something (debugging).")]
	public float[] lastSpawnTimer;

	[HideInInspector]
	public float playerFollowSpeed = 50;

	float lengthOfRaycast = 9999;

	Vector3 spawnObjectMoveTowardsVector;

	Vector3 raycastOriginPoint;

	bool spawnIsAGo;

	bool spawnNoGo;

	float timerFunc;

	[Header("Layer Masks")]
	[Tooltip("This layer mask should be set to the layer of the terrain; this is used by the Raycasting.")]
	public LayerMask layerMaskRaycast;

	[Tooltip("This layer mask should be set to the layer of the obstacles in your environment; this is used to prevent the spawner from spawning something inside a rock for example.")]
	public LayerMask layerMaskNoSpawn;

	[Header("Player Buffer Zone")]
	[Tooltip("Enter a float value, prevents spawns from occurring within this distance around the player.")]
	public float setDistance;

	[Header("Terrain Steepness Buffer")]
	[Tooltip("This controls how steep terrain can be before the spawner views it as unsuitable. this value is from 0.0 to 1.0 where 0.1 is extremely steep and 1.0 is flat, and corresponds to the Y-Rotation Axis of the Terrain Normal.")]
	[Range(0.0f, 1.0f)]
	public float steepnessBufferAngle;

	GameObject spawnPlane;

	void Start () 
	{ 
		timerFunc = 0;

		for (int arrayIndex = 0; arrayIndex < lastSpawnTimer.Length; arrayIndex++)
		{
			lastSpawnTimer[arrayIndex] = Time.time;
		}
	}

	void Update () 
	{
		spawnObjectMoveTowardsVector = new Vector3 (playerToFollow.transform.position.x, 
		playerToFollow.transform.position.y + 120, playerToFollow.transform.position.z);
		
		transform.position = Vector3.MoveTowards (transform.position, spawnObjectMoveTowardsVector, 
		playerFollowSpeed * Time.deltaTime);

		for (int arrayIndex = 0; arrayIndex < aiPrefabsToSpawn.Length; arrayIndex++) 
		{
			if (timerFunc == 10) 
			{
				raycastOriginPoint = getRandomLocation ();
				timerFunc = 0;
			} 
			
			else
				timerFunc++;

			Vector3 raycastDirection = transform.TransformDirection (Vector3.down);

			RaycastHit impactPoint;

			if (Physics.Raycast (raycastOriginPoint, raycastDirection, 
			out impactPoint, lengthOfRaycast, layerMaskNoSpawn)) 
			{
				spawnNoGo = true;
			} 
			
			else
			{
				spawnNoGo = false;
			}

			if (Physics.Raycast (raycastOriginPoint, raycastDirection, 
			out impactPoint, lengthOfRaycast, layerMaskRaycast))
			{
				spawnIsAGo = true;
			}

			else 
			{
				spawnIsAGo = false;
			}

			if ((spawnCeiling [arrayIndex] < impactPoint.point.y) 
			|| (spawnFloor [arrayIndex] > impactPoint.point.y)) 
			{
				spawnIsAGo = false;
			}

			if (playerBufferArea (playerToFollow.transform.position, 
			impactPoint.point, setDistance) == false) 
			{
				spawnIsAGo = false;
			}

			if (terrainBufferAngle (impactPoint.normal, steepnessBufferAngle) == false) 
			{
				spawnIsAGo = false;
			}

			if (spawnIsAGo & (!spawnNoGo)) 
			{
				if ((Time.time - lastSpawnTimer [arrayIndex]) > spawnTimer [arrayIndex]) 
				{
					Instantiate(aiPrefabsToSpawn[arrayIndex], new Vector3 (impactPoint.point.x, 
					impactPoint.point.y + InstantiateHeight[arrayIndex], impactPoint.point.z), Quaternion.identity);
					
					lastSpawnTimer[arrayIndex] = Time.time;
				}
			} 
		}
	}

	Vector3 getRandomLocation()
	{
		float randomX = Random.Range (gameObject.transform.position.x - 
		(gameObject.transform.localScale.x * 4), gameObject.transform.position.x + 
		(gameObject.transform.localScale.x * 4)); 
		
		float unchangedY = gameObject.transform.position.y;
		
		float randomZ = Random.Range (gameObject.transform.position.z - (gameObject.transform.localScale.z * 4), 
		gameObject.transform.position.z + (gameObject.transform.localScale.z * 4)); 

		Vector3 randomPosition = new Vector3 (randomX, unchangedY, randomZ);

		Vector3 originForDebugRay = new Vector3 (transform.position.x, 
		transform.position.y + 10, transform.position.z);
		
		Debug.DrawLine(originForDebugRay, randomPosition);

		return randomPosition;
	}

	bool playerBufferArea(Vector3 playerPosition, Vector3 impactPoint, float setDistance)
	{
		float distanceToSpawn = Vector3.Distance (playerPosition, impactPoint);

		if (distanceToSpawn > setDistance) 
		{
			return true;
		} 
		
		else 
		{
			return false;
		}
	}

	bool terrainBufferAngle(Vector3 terrainNormal, float bufferAngle)
	{
		if (bufferAngle <= terrainNormal.y) 
		{
			return true;
		}

		else
		{
			return false;
		}
	}
}