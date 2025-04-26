using UnityEngine;

[RequireComponent(typeof(Target))]

public class Boid : MonoBehaviour{

    public Vector3 velocity;
    public Vector3 acceleration;
    public float maxVelocity;
    public float minVelocity;
    public float maxSteerForce;
    public float targetWeight;
    Vector3 targetDiff = Vector3.zero;
    readonly int numberOfRays = 300;
    Vector3[] rayDirections;
    public float boundsRadius;
    public float collisionAvoidDst;
    public float avoidCollisionWeight;
    public LayerMask obstacleMask;
    public bool hitObject = false;

    void Start()
    {
        velocity = this.transform.forward * minVelocity;
        acceleration = Vector3.zero;
    }

    void Update()
    {
        if (!hitObject){
            var targetAverage = Vector3.zero;
            var targetsFound = 0;

            var targets = FindObjectsByType<Target>(FindObjectsSortMode.None);
            if(targets.Length != 0){

                foreach(var target in targets){
                    targetDiff = target.transform.position - this.transform.position;
                    targetAverage += targetDiff;
                    targetsFound ++;
                    
                }
            }

            acceleration += SteerTowards(targetAverage/targetsFound) * targetWeight;

            if (IsHeadingForCollision ()) {
                Vector3 collisionAvoidDir = ObstacleRays ();
                Vector3 collisionAvoidForce = SteerTowards (collisionAvoidDir) * avoidCollisionWeight;
                acceleration += collisionAvoidForce;
            }
            
            velocity += acceleration * Time.deltaTime;
            float speed = velocity.magnitude;
            Vector3 dir = velocity / speed;
            speed = Mathf.Clamp(speed, minVelocity, maxVelocity);
            velocity = dir * speed;

            this.transform.position += velocity * Time.deltaTime;
            this.transform.rotation = Quaternion.LookRotation(velocity);

            if(CheckCollision()){
                hitObject = true;
                velocity = Vector3.zero;
                maxVelocity = 0;
                minVelocity = 0;
                print("hit");
            }
        }
    }

    public Vector3 SteerTowards (Vector3 vector) {
        Vector3 v = vector.normalized * this.maxVelocity - this.velocity;
        return Vector3.ClampMagnitude (v, this.maxSteerForce);
    }

    private Vector3[] ObstacleRayHelper(){
        rayDirections = new Vector3[numberOfRays];

        float goldenRatio = (1 + Mathf.Sqrt(5)) / 2;
        float angleIncrement = Mathf.PI * 2 * goldenRatio;

        for (int i = 0; i < numberOfRays; i ++){
            float t = (float) i / numberOfRays;
            float inclination = Mathf.Acos (1 - 2 * t);
            float rayAngle = angleIncrement * i;

            float x = Mathf.Sin (inclination) * Mathf.Cos(angleIncrement);
            float y = Mathf.Sin (inclination) * Mathf.Sin(angleIncrement);
            float z = Mathf.Cos (inclination);

            rayDirections[i] = new Vector3 (x, y, z);
        }

        return rayDirections;
    }

    Vector3 ObstacleRays() {
        rayDirections = ObstacleRayHelper();
        Vector3[] rays = rayDirections;

        for (int i = 0; i < rayDirections.Length; i++) {
            Vector3 dir = this.transform.TransformDirection (rayDirections[i]);
            Ray ray = new Ray (this.transform.position, dir);
            if (!Physics.SphereCast (ray, boundsRadius, collisionAvoidDst, obstacleMask)) {
                return dir;
            }
        }

        return this.transform.forward;
    }

    bool IsHeadingForCollision () {
        if (Physics.SphereCast(this.transform.position, boundsRadius, this.transform.forward, out RaycastHit hit, collisionAvoidDst, obstacleMask))
        {
            return true;
        }
        return false;
    }

    bool CheckCollision (){
        if (Physics.SphereCast(this.transform.position, boundsRadius, this.transform.forward, out RaycastHit hit, 0.01f, obstacleMask)){
            return true;
        }
        return false;
    }

}
