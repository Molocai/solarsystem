using UnityEngine;
using System.Collections.Generic;

namespace UbiSolarSystem
{
    public class Planet : MonoBehaviour
    {
        public float Mass = 10f;
        public bool AffectOtherPlanets = true;

        private List<Planet> PlanetsAffectingMe;

        // Use this for initialization
        void Start()
        {
            PlanetsAffectingMe = new List<Planet>();
        }

        // Update is called once per frame
        void Update()
        {
            foreach(Planet planet in PlanetsAffectingMe)
            {
                float gravityPull = (planet.Mass * this.Mass) / Mathf.Abs(Vector3.Distance(planet.transform.position, transform.position));
                Vector3 unitVector = Vector3.Normalize(planet.transform.position - transform.position);
                //Vector3 gravityForce = gravityPull * unitVector;

                Vector3 newPos = transform.position + unitVector * gravityPull;

                transform.position = newPos * Time.deltaTime;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            Planet otherPlanet = other.GetComponent<Planet>();
            if (otherPlanet)
            {
                PlanetsAffectingMe.Add(otherPlanet);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            Planet otherPlanet = other.GetComponent<Planet>();
            if (otherPlanet)
            {
                PlanetsAffectingMe.Remove(otherPlanet);
            }
        }
    }
}