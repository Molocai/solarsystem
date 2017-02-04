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

        [Header("Cosmetic")]
        public GameObject PrefabParticlesExplosion;

        private List<Planet> PlanetsAffectingMe;
        public Vector3 Velocity;

        void Start()
        {
            PlanetsAffectingMe = new List<Planet>();
            Velocity = InitialVelocity;
        }

        void Update()
        {
            // Retrieve all the forces applied by other planets
            List<Vector3> forces = GetForcesApplyingToMe();

            if (IsAffectedByOtherPlanets)
            {
                Vector3 finalForce = Vector3.zero;
                // The final force is equal to the sum of each force. *insert darth vader ascii art*
                foreach (Vector3 force in forces)
                {
                    finalForce += force;
                }

                // V(t) = v(t-1) + acceleration * time
                Velocity = Velocity + finalForce * Time.deltaTime;
                Debug.DrawLine(transform.position + new Vector3(0, 1, 0), transform.position + Velocity + new Vector3(0, 1, 0), Color.red);
                transform.position = transform.position + (Velocity * Time.deltaTime);
            }
        }

        /// <summary>
        /// Returns a list of forces being applied by surrounding planets affecting others.
        /// </summary>
        /// <returns>The list of forces</returns>
        private List<Vector3> GetForcesApplyingToMe()
        {
            List<Vector3> forces = new List<Vector3>();

            foreach (Planet planet in PlanetsAffectingMe)
            {
                if (planet != null && planet.CanAffectOtherPlanets)
                {
                    // Calculate the normalized direction
                    Vector3 pullDirection = Vector3.Normalize(planet.transform.position - transform.position);
                    // Newton law => (Mass1 * Mass2 / (distance)²)
                    float pullForce = (planet.Mass * this.Mass) / Mathf.Pow(Vector3.Distance(planet.transform.position, transform.position), 2f);
                    // Gravitational attraction
                    Vector3 gravityForce = pullDirection * pullForce;

                    forces.Add(gravityForce);
                }
            }

            return forces;
        }

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

        void OnDrawGizmos()
        {
            // Display initial velocity in the editor
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, transform.position + InitialVelocity);
        }
    }
}