using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UbiSolarSystem
{
    public class UIHandler : MonoBehaviour
    {
        public InputHandler InputHandler;
        public GameObject PlanetPrefab;

        public void BeginDrag()
        {
            GameObject instantiatedPlanet = Instantiate(PlanetPrefab, InputHandler.GetMousePositionInWorld(), Quaternion.identity);
            InputHandler.SelectedPlanet = instantiatedPlanet.GetComponent<Planet>();
        }
    }
}