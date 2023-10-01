using System;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.ScriptableObjectCore.Scripts.Variables;
using BML.Scripts.SpaceGraph;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BML.Scripts.Dialogue
{
    public class DialogueController : MonoBehaviour
    {
        [SerializeField] private GameEvent _onReceiveReward;
        [SerializeField] private SpaceNodeReference _currentSpaceNode;
        [SerializeField] private IntReference _currentFoodCount;
        [SerializeField] private IntReference _currentFuelCount;
        [SerializeField] private IntReference _currentPersonCount;
        
        [Header("Disposition")]
        [SerializeField] private float _stingySuccessChance = .2f;
        [SerializeField] private float _generousSuccessChance = .9f;

        private void OnEnable()
        {
            _onReceiveReward.Subscribe(ReceiveReward);
            _currentSpaceNode.Subscribe(SetCurrentNodeValues);
            SetCurrentNodeValues();
        }
        
        private void OnDisable()
        {
            _onReceiveReward.Unsubscribe(ReceiveReward);
            _currentSpaceNode.Unsubscribe(SetCurrentNodeValues);
        }

        private void SetCurrentNodeValues()
        {
            DialogueLua.SetVariable("RandomRoll", _currentSpaceNode.Value.SpaceNode.RandomRoll);
            
            if (_currentSpaceNode.Value.SpaceNode.Disposition == Disposition.Stingy)
                DialogueLua.SetVariable("SuccessChance", _stingySuccessChance);
            else if (_currentSpaceNode.Value.SpaceNode.Disposition == Disposition.Generous)
                DialogueLua.SetVariable("SuccessChance", _generousSuccessChance);
            
            DialogueLua.SetVariable("PopulationDensity", (int) _currentSpaceNode.Value.SpaceNode.PopulationDensity);
            DialogueLua.SetVariable("SoilFertility", (int) _currentSpaceNode.Value.SpaceNode.SoilFertility);
            DialogueLua.SetVariable("FuelSupply", (int) _currentSpaceNode.Value.SpaceNode.FuelSupply);
            DialogueLua.SetVariable("Disposition", (int) _currentSpaceNode.Value.SpaceNode.Disposition);
        }

        private void ReceiveReward()
        {
            if (DialogueLua.GetVariable("FoodRequested").asBool)
            {
                var foodReward = (int) _currentSpaceNode.Value.SpaceNode.SoilFertility;
                _currentFoodCount.Value += foodReward;
                Debug.Log($"Rewarded {foodReward} food");
            }
            else if (DialogueLua.GetVariable("FuelRequested").asBool)
            {
                var fuelReward = (int) _currentSpaceNode.Value.SpaceNode.FuelSupply;
                _currentFuelCount.Value += fuelReward;
                Debug.Log($"Rewarded {fuelReward} fuel");
            }
            else if (DialogueLua.GetVariable("PersonRequested").asBool)
            {
                var personReward = (int) _currentSpaceNode.Value.SpaceNode.PopulationDensity;
                _currentPersonCount.Value += personReward;
                Debug.Log($"Rewarded {personReward} people");
            }
        }
    }
}