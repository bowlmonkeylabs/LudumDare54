// using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BML.Scripts.CaveV2;
using BML.Scripts.Utils;
using TMPro;
using UnityEngine;

namespace BML.Scripts.SpaceGraph {
    public class TraitTextController : MonoBehaviour
    {
        [SerializeField] private SpaceNodeComponent _spaceNodeComponent;
        [SerializeField] private TMP_Text _text1;
        // [SerializeField] private TMP_Text _text2;
        // [SerializeField] private TMP_Text _text3;
        // [SerializeField] private TMP_Text _text4;
        // new string[4]{"Population", "Fuel", "Food", "Disposition"}
        private List<string> _traits = new List<string>{"Population", "Fuel", "Food", "Disposition"};

        void Start() {
            SeedManager.Instance.GetSteppedSeed("TraitText"+_spaceNodeComponent.GetInstanceID());
            string trait = _traits.OrderBy(t => UnityEngine.Random.value).First();
            if(trait == "Population") {
                _text1.text = "Population Density: " + Enum.GetName(typeof(PopulationDensity), _spaceNodeComponent.SpaceNode.PopulationDensity);
            }
            if(trait == "Fuel") {
                _text1.text = "Fuel Supply: " + Enum.GetName(typeof(FuelSupply), _spaceNodeComponent.SpaceNode.FuelSupply);
            }
            if(trait == "Food") {
                _text1.text = "Soil Fertility: " + Enum.GetName(typeof(SoilFertility), _spaceNodeComponent.SpaceNode.SoilFertility);
            }
            if(trait == "Disposition") {
                _text1.text = "Disposition: " + Enum.GetName(typeof(Disposition), _spaceNodeComponent.SpaceNode.Disposition);
            }
        }
    }
}
