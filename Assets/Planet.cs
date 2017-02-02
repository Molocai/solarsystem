using UnityEngine;
using System.Collections.Generic;

namespace UbiSolarSystem
{
    public class Planet : MonoBehaviour
    {
        [Header("Physics parameters")]
        public float Mass = 2f;
        public bool CanAffectOtherPlanets = true;
        public bool IsAffectedByOtherPlanets = true;

        [Header("Cosmetic")]
        public GameObject PrefabParticlesExplosion;

        private List<Planet> PlanetsAffectingMe;

        void Start()
        {
            PlanetsAffectingMe = new List<Planet>();
        }

        void Update()
        {
            // Retrieve all the forces applied by other planets
            List<Vector3> forces = GetForcesApplyingToMe();

            if (IsAffectedByOtherPlanets)
            {
                Vector3 finalForce = Vector3.zero;
                foreach (Vector3 force in forces)
                {
                    finalForce += force;
                }

                Debug.DrawLine(transform.position + new Vector3(0, 1, 0), transform.position + finalForce + new Vector3(0, 1, 0), Color.red);

                transform.position = transform.position + finalForce * Time.deltaTime;
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
                if (planet.CanAffectOtherPlanets)
                {
                    // Calculate the normalized direction
                    Vector3 pullDirection = Vector3.Normalize(planet.transform.position - transform.position);
                    // Newton law => (Mass1 * Mass2 / (distance)²)
                    float pullForce = (planet.Mass * this.Mass) / Mathf.Pow(Vector3.Distance(planet.transform.position, transform.position), 2f);
                    // Final gravity force
                    Vector3 gravityForce = pullDirection * pullForce;

                    forces.Add(gravityForce);
                }
            }

            return forces;
        }

        private void OnTriggerEnter(Collider other)
        {
            Planet otherPlanet = other.GetComponent<Planet>();
            if (otherPlanet && !PlanetsAffectingMe.Contains(otherPlanet))
            {
                PlanetsAffectingMe.Add(otherPlanet);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Planet otherPlanet = other.GetComponent<Planet>();
            if (otherPlanet && PlanetsAffectingMe.Contains(otherPlanet))
            {
                PlanetsAffectingMe.Remove(otherPlanet);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            Planet otherPlanet = collision.gameObject.GetComponent<Planet>();
            if (Mass <= otherPlanet.Mass)
            {
                GameObject explosion = Instantiate(PrefabParticlesExplosion, collision.contacts[0].point, Quaternion.identity);

                Destroy(explosion, 3f);
                Destroy(gameObject, 0.3f);
            }
        }
    }
}