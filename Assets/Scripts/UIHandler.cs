using UnityEngine;
using UnityEngine.SceneManagement;

namespace UbiSolarSystem
{
    public class UIHandler : MonoBehaviour
    {
        public InputHandler InputHandler;
        public GameObject PlanetPrefab;

        public bool RandomizeMass;
        public float MinMass = 5f;
        public float MaxMass = 15f;

        private Animator Animator;

        void Start()
        {
            Animator = GetComponent<Animator>();
        }

        public void BeginDrag()
        {
            GameObject instantiatedPlanet = Instantiate(PlanetPrefab, InputHandler.GetMousePositionInWorld(), Quaternion.identity);
            Planet planet = instantiatedPlanet.GetComponent<Planet>();
            InputHandler.SelectedPlanet = planet;

            if (RandomizeMass)
            {
                planet.RandomizeMass(MinMass, MaxMass);
            }

            Animator.SetTrigger("Invisible");
        }

        public void ReleaseDrag()
        {
            Animator.SetTrigger("Visible");
        }

        public void ReloadScene()
        {
            SceneManager.LoadScene(0);
        }
    }
}