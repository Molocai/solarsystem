using UnityEngine;
using System.Collections.Generic;

namespace UbiSolarSystem
{
    public class Planet : MonoBehaviour
    {
        [Header("Physics parameters")]
        /// <summary>
        /// Planet's mass
        /// </summary>
        public float Mass = 2f;
        /// <summary>
        /// Initial velocity at which the planet will start
        /// </summary>
        public Vector3 InitialVelocity;
        /// <summary>
        /// If true, this planet will apply gravitational attraction to other bodies
        /// </summary>
        public bool CanAffectOtherPlanets = true;
        /// <summary>
        /// If true, this planet will receive gravitational attraction from other bodies
        /// </summary>
        public bool IsAffectedByOtherPlanets = true;

        [Header("Trajectory prediction")]
        /// <summary>
        /// How far in time should we calculate the trajectory prediction
        /// </summary>
        public float PredictionTime = 2f;
        /// <summary>
        /// Trajectory accuracy. How many vertex should the trajectory prediction use
        /// </summary>
        public int PredictionSteps = 20;

        [Header("Cosmetic")]
        public GameObject PrefabParticlesExplosion;

        /// <summary>
        /// Describes the current Velocity of the planet
        /// </summary>
        [HideInInspector()]
        public Vector3 Velocity { get; set; }

        private List<Planet> PlanetsAffectingMe;

        void Start()
        {
            PlanetsAffectingMe = new List<Planet>();

            Velocity = InitialVelocity;
        }

        void Update()
        {
            if (IsAffectedByOtherPlanets)
            {
                Vector3 finalForce = GetSumForcesApplyingToMe(transform.position);

                // V(t) = v(t-1) + acceleration * time
                Velocity += finalForce * Time.deltaTime;
                //Debug.DrawLine(transform.position + new Vector3(0, 1, 0), transform.position + Velocity + new Vector3(0, 1, 0), Color.red);
                transform.position = transform.position + (Velocity * Time.deltaTime);

                DrawPrediction();
            }
        }

        /// <summary>
        /// Draws a prediction of the planet's trajectory
        /// </summary>
        private void DrawPrediction()
        {
            float timeStep = PredictionTime / PredictionSteps;

            LineRenderer lr = GetComponent<LineRenderer>();
            if (lr == null) return;

            lr.numPositions = PredictionSteps;
            // First point is current position
            lr.SetPosition(0, transform.position);

            // These will hold last prediction's values
            Vector3 lastVelocity = Velocity;
            Vector3 lastPosition = transform.position;

            for (int i = 1; i < PredictionSteps; i++)
            {
                // Get all the forces
                Vector3 forcesThisStep = GetSumForcesApplyingToMe(lastPosition);
                // Calculate velocity at given time
                Vector3 velocityThisStep = lastVelocity + forcesThisStep * timeStep * i; // (timeStep * i) is the equivalent of Time.deltaTime in the prediction
                // Deduce positin
                Vector3 positionThisStep = lastPosition + velocityThisStep * timeStep * i;

                lr.SetPosition(i, positionThisStep);

                // Raycast from last point to new point to check if the prediction is crossing a planet
                RaycastHit hitInfo;
                Ray rayhit = new Ray(lastPosition, positionThisStep - lastPosition);
                Physics.Raycast(rayhit, out hitInfo, Vector3.Distance(positionThisStep, lastPosition) * 2f, 1 << LayerManager.ToInt(LAYER.PLANET), QueryTriggerInteraction.Ignore);
                // If we hit something stop drawing the prediction
                if (hitInfo.collider != null)
                {
                    lr.numPositions = i + 1;
                    i = PredictionSteps - 1;
                }

                lastPosition = positionThisStep;
                lastVelocity = velocityThisStep;
            }
        }

        /// <summary>
        /// Sums the forces applying to me
        /// </summary>
        /// <param name="myPosition">The position to base the maths on</param>
        /// <returns>The final force vector</returns>
        private Vector3 GetSumForcesApplyingToMe(Vector3 myPosition)
        {
            List<Vector3> forces = GetForcesApplyingToMe(myPosition);

            Vector3 finalForce = Vector3.zero;
            // The final force is equal to the sum of each force. *insert darth vader ascii art*
            foreach (Vector3 force in forces)
            {
                finalForce += force;
            }

            return finalForce;
        }

        /// <summary>
        /// Returns a list of individual forces being applied by surrounding planets affecting others.
        /// </summary>
        /// <param name="myPosition">The position to base the maths on</param>
        /// <returns>The list of forces</returns>
        private List<Vector3> GetForcesApplyingToMe(Vector3 myPosition)
        {
            List<Vector3> forces = new List<Vector3>();

            foreach (Planet otherPlanet in PlanetsAffectingMe)
            {
                if (otherPlanet != null && otherPlanet.CanAffectOtherPlanets)
                {
                    // Get the direction the planet's pulling us
                    Vector3 pullDirection = Vector3.Normalize(otherPlanet.transform.position - myPosition);
                    // Calculate the magnitude of the force using Newton's law of universal gravitation => (Mass1 * Mass2 / (distance)²)
                    float pullMagnitude = (otherPlanet.Mass * this.Mass) / Mathf.Pow(Vector3.Distance(otherPlanet.transform.position, myPosition), 2f);
                    // Final gravitational attraction force
                    Vector3 gravitationalForce = pullDirection * pullMagnitude;

                    forces.Add(gravitationalForce);
                }
            }

            return forces;
        }


        /// <summary>
        /// Returns a list of individual forces being applied by surrounding planets affecting others.
        /// </summary>
        /// <returns>The list of forces</returns>

        private void OnTriggerEnter(Collider other)
        {
            // For some reason, triggers can trigger a trigger if both have a non kinematic
            // rigidbody. So this test is necessary.
            if (other.isTrigger)
            {
                return;
            }

            Planet otherPlanet = other.GetComponent<Planet>();
            if (otherPlanet)
            {
                otherPlanet.AddAffectingPlanet(this);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            // For some reason, triggers can trigger a trigger if both have a non kinematic
            // rigidbody. So this test is necessary.
            if (other.isTrigger)
            {
                return;
            }

            Planet otherPlanet = other.GetComponent<Planet>();
            if (otherPlanet)
            {
                otherPlanet.RemoveAffectingPlanet(this);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Planet otherPlanet = collision.gameObject.GetComponent<Planet>();
            if (Mass <= otherPlanet.Mass)
            {
                // Each planet is responsible for it's own destruction, we don't want cross references
                GameObject explosion = Instantiate(PrefabParticlesExplosion, collision.contacts[0].point, Quaternion.identity);

                Destroy(explosion, 3f);
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Adds the Planet to the list of planets that has an effect on this one
        /// </summary>
        /// <param name="p">The planet to take into account</param>
        public void AddAffectingPlanet(Planet p)
        {
            PlanetsAffectingMe.Add(p);
        }

        /// <summary>
        /// Removes the Planet from the list of planets that has an effect on this one
        /// </summary>
        /// <param name="p">The planet to remove</param>
        public void RemoveAffectingPlanet(Planet p)
        {
            PlanetsAffectingMe.Remove(p);
        }

        public void RandomizeMass(float min, float max, bool adaptScale = true)
        {
            Mass = Random.Range(min, max);

            if (adaptScale)
            {
                gameObject.transform.localScale = new Vector3(Mass / 10, Mass / 10, Mass / 10);
            }
        }

        void OnDrawGizmos()
        {
            // Display initial velocity in the editor
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + InitialVelocity);
        }
    }
}