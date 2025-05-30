using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Boid))]
public class BoidAlignmentBehaviour : MonoBehaviour
{

    private Boid boid;

    public float radius;
    public float alignmentWeight;

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
                average += oBoid.velocity;
                foundBoids ++;
            }
        }

        if(foundBoids > 0){
            average /= foundBoids;
            boid.acceleration += boid.SteerTowards(average) * alignmentWeight;
        }
    }

}
