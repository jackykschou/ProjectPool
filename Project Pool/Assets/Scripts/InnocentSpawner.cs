using UnityEngine;

public class InnocentSpawner : MonoBehaviour
{
    public static int CurrentLevel;

    public Innocent InnocentPrefab;

    public float SpawnDistanceMin = 1f;
    public float SpawnDistnaceMax = 10f;

    public void Start()
    {
        SpawnInnocents(2000);
    }

    public void SpawnInnocents(int innocentNumber)
    {
        for (int i = 0; i < innocentNumber; i++)
        {
            Instantiate(InnocentPrefab.gameObject,
                new Vector3((Random.value > 0.5f ? 1f : -1f) * Random.Range(SpawnDistanceMin, SpawnDistnaceMax),
                    (Random.value > 0.5f ? 1f : -1f) * Random.Range(SpawnDistanceMin, SpawnDistnaceMax),
                    (Random.value > 0.5f ? 1f : -1f) * Random.Range(SpawnDistanceMin, SpawnDistnaceMax)), 
                Quaternion.identity);
        }
    }

}
