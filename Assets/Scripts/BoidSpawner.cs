using UnityEngine;

public class BoidSpawner : MonoBehaviour
{

    public GameObject prefab;

    public float radius;

    public int number;
    private int deathCount;

    void Start()
    {
        for(int i = 0; i < number; i++){
            Instantiate(prefab, this.transform.position + Random.insideUnitSphere * radius, Random.rotation);
        }       
    }

    void Update()
    {
        
    }

}
