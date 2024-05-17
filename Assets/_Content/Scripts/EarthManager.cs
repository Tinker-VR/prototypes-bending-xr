using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthManager : MonoBehaviour
{
    public static EarthManager Instance { get; private set; }

    [SerializeField] private GameObject rockPrefab;

    private float initialUpwardForce = 5f;

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

    public GameObject SpawnRock(Vector3 position, float scale, GameObject hand)
    {
        Quaternion randomRotation = Quaternion.Euler(0, Random.Range(0, 360), 0);

        GameObject rock = Instantiate(rockPrefab, position, randomRotation);
        rock.transform.localScale *= scale;

        rock.GetComponent<Rigidbody>().mass = rock.GetComponent<Rigidbody>().mass * scale;

        RockMovement rockMovement = rock.AddComponent<RockMovement>();
        rockMovement.Initialize(hand, initialUpwardForce);

        return rock;
    }
    
    public void LiftRock(GameObject rock, GameObject hand)
    {
        RockMovement rockMovement = rock.GetComponent<RockMovement>();
        rockMovement.Initialize(hand, initialUpwardForce);
    }
}