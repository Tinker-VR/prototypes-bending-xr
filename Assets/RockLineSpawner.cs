using System.Collections;
using UnityEngine;

public class RockLineSpawner : MonoBehaviour
{
    public static RockLineSpawner Instance { get; private set; }

    public GameObject[] rockPrefabs; // Array of rock prefabs
    public int numberOfRocks = 10; // Number of rocks to spawn
    public Vector3 spawnDirection = Vector3.forward; // Direction to spawn the rocks
    public Vector3 positionVariation = new Vector3(0.5f, 0.2f, 0.5f); // Position variation
    public Vector3 rotationVariation = new Vector3(10f, 30f, 10f); // Rotation variation
    public Vector3 scaleVariation = new Vector3(0.1f, 0.1f, 0.1f); // Scale variation
    public float gapDistance = 0.1f;

    public float spawnDelayVariation = 0.1f; // Minimum delay between spawns
    public float spawnDelay = 0.05f; // Minimum delay between spawns

    public float startHeightOffset = -0.01f; // Initial height offset for animation
    public float moveDuration = 0.5f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    
    private void Start()
    {
        // StartCoroutine(SpawnRockLine());
    }

    private IEnumerator SpawnRockLine()
    {
        if (rockPrefabs.Length == 0) yield break;

        for (int i = 0; i < numberOfRocks; i++)
        {
            GameObject rockPrefab = GetRockPrefab(i, numberOfRocks);
            Vector3 targetPosition = transform.position + spawnDirection * i * gapDistance;

            // Apply random position variation
            targetPosition += new Vector3(
                Random.Range(-positionVariation.x, positionVariation.x),
                Random.Range(-positionVariation.y, positionVariation.y),
                Random.Range(-positionVariation.z, positionVariation.z)
            );

            Quaternion rotation = Quaternion.Euler(
                Random.Range(-rotationVariation.x, rotationVariation.x),
                Random.Range(-rotationVariation.y, rotationVariation.y),
                Random.Range(-rotationVariation.z, rotationVariation.z)
            );

            Vector3 scale = rockPrefab.transform.localScale;
            scale = Vector3.Scale(scale, new Vector3(
                1 + Random.Range(-scaleVariation.x, scaleVariation.x),
                1 + Random.Range(-scaleVariation.y, scaleVariation.y),
                1 + Random.Range(-scaleVariation.z, scaleVariation.z)
            ));

            Vector3 startPosition = targetPosition + Vector3.up * startHeightOffset;
            GameObject rockInstance = Instantiate(rockPrefab, startPosition, rotation);
            rockInstance.transform.localScale = scale;

            StartCoroutine(MoveToPosition(rockInstance, targetPosition, moveDuration));

            float delay = spawnDelay + Random.Range(0, spawnDelayVariation);
            yield return new WaitForSeconds(delay);
        }
    }

    private IEnumerator MoveToPosition(GameObject rock, Vector3 targetPosition, float duration)
    {
        Vector3 startPosition = rock.transform.position;
        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            rock.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rock.transform.position = targetPosition;
    }

    private GameObject GetRockPrefab(int index, int totalRocks)
    {
        int prefabIndex = Mathf.FloorToInt((float)index / totalRocks * rockPrefabs.Length);
        return rockPrefabs[prefabIndex];
    }
}
