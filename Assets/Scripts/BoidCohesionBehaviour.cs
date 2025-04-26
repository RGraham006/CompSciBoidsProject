using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Boid))]

public class BoidCohesionBehaviour : MonoBehaviour
{

    private Boid boid;
    public float radius;
    public float cohesionWeight;

    void Start()
    {
        boid = GetComponent<Boid>();
    }

    void Update()
    {
        var boids = FindObjectsByType<Boid>(FindObjectsSortMode.None);
        var average = Vector3.zero;
        var foundBoids = 0;

        foreach (var oBoid in boids.Where(b => b != boid))
        {
            var diff = oBoid.transform.position - this.transform.position;
            if(diff.magnitude < radius && !oBoid.hitObject){
                average += diff;
                foundBoids ++;
            }
        }

        if(foundBoids > 0){
            average /= foundBoids;
            boid.acceleration += boid.SteerTowards (average) * cohesionWeight;
        }
    }

}
