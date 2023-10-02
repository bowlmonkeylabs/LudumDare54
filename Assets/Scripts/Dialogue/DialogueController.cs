using System;
using System.Collections.Generic;
using System.Linq;
using BML.ScriptableObjectCore.Scripts.Events;
using BML.ScriptableObjectCore.Scripts.Variables;
using BML.Scripts.SpaceGraph;
using Codice.CM.Common;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Serialization;
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
        [SerializeField] private List<DispositionSuccessChance> _dispositionSuccessChances;

        [Header("Rewards")]
        [SerializeField] private List<FuelReward> _fuelRewards = new List<FuelReward>();
        [SerializeField] private List<FoodReward> _foodRewards = new List<FoodReward>();
        [SerializeField] private List<PeopleReward> _peopleRewards = new List<PeopleReward>();

        [Header("Dialogue")] [SerializeField]
        private List<DispositionDialogues> _dispositionDialogues = new List<DispositionDialogues>();
        
        [Serializable]
        private class FuelReward
        {
            public FuelSupply _fuelSupply;
            public int _reward;
        }
        
        [Serializable]
        private class FoodReward
        {
            public SoilFertility _soilFertility;
            public int _reward;
        }
        
        [Serializable]
        private class PeopleReward
        {
            public PopulationDensity _populationDensity;
            public int _reward;
        }
        
        [Serializable]
        private class DispositionDialogues
        {
            public Disposition _disposition;
            [TextArea(3,5)]
            public List<string> _dialogues;
        }

        [Serializable]
        private class DispositionSuccessChance
        {
            public Disposition Disposition;
            public float SuccessChance;
        }

        private int personCountValueAtEndOfLastTurn;
        private int fuelCountValueAtEndOfLastTurn;
        private int foodCountValueAtEndOfLastTurn;

        private void OnEnable()
        {
            _onReceiveReward.Subscribe(ReceiveReward);
            _currentSpaceNode.Subscribe(SetCurrentNodeValues);

            MarkEndOfTurnValues();
        }
        
        private void OnDisable()
        {
            _onReceiveReward.Unsubscribe(ReceiveReward);
            _currentSpaceNode.Unsubscribe(SetCurrentNodeValues);
        }

#region Dialgue Rewards
        private void SetCurrentNodeValues()
        {
            DialogueLua.SetVariable("RandomRoll", _currentSpaceNode.Value.SpaceNode.RandomRoll);

            float? successChance = _dispositionSuccessChances
                .FirstOrDefault(d => 
                    d.Disposition == _currentSpaceNode.Value?.SpaceNode.Disposition
                )?.SuccessChance;
            if (successChance == null)
            {
                successChance = 0f;
            }
            DialogueLua.SetVariable("SuccessChance", successChance);
            
            DialogueLua.SetVariable("PopulationDensity", (int) _currentSpaceNode.Value.SpaceNode.PopulationDensity);
            DialogueLua.SetVariable("SoilFertility", (int) _currentSpaceNode.Value.SpaceNode.SoilFertility);
            DialogueLua.SetVariable("FuelSupply", (int) _currentSpaceNode.Value.SpaceNode.FuelSupply);
            DialogueLua.SetVariable("Disposition", (int) _currentSpaceNode.Value.SpaceNode.Disposition);

            var greetingOptions = _dispositionDialogues
                .FirstOrDefault(d => d._disposition == _currentSpaceNode.Value.SpaceNode.Disposition)
                ._dialogues;
            var greeting = greetingOptions[Random.Range(0, greetingOptions.Count)];
            
            DialogueLua.SetVariable("Greeting", greeting);
            
        }

        private void ReceiveReward()
        {
            if (DialogueLua.GetVariable("FoodRequested").asBool)
            {
                var fertility = _currentSpaceNode.Value.SpaceNode.SoilFertility;
                var reward = (_foodRewards.FirstOrDefault(r => r._soilFertility == fertility))._reward;
                _currentFoodCount.Value += reward;
                Debug.Log($"Rewarded {reward} food");
            }
            else if (DialogueLua.GetVariable("FuelRequested").asBool)
            {
                var fuelSupply = _currentSpaceNode.Value.SpaceNode.FuelSupply;
                var reward = (_fuelRewards.FirstOrDefault(r => r._fuelSupply == fuelSupply))._reward;
                _currentFuelCount.Value += reward;
                Debug.Log($"Rewarded {reward} fuel");
            }
            else if (DialogueLua.GetVariable("PersonRequested").asBool)
            {
                var populationDensity = _currentSpaceNode.Value.SpaceNode.PopulationDensity;
                var reward = (_peopleRewards.FirstOrDefault(r => r._populationDensity == populationDensity))._reward;
                _currentPersonCount.Value += reward;
                Debug.Log($"Rewarded {reward} people");
            }
        }

#endregion

#region End turn screen
    public void SetDialogueVariablesForSummary() {
        var personDelta = _currentPersonCount.Value - personCountValueAtEndOfLastTurn;
        DialogueLua.SetVariable("DeathCount", personDelta < 0 ? -personDelta : 0);
        var fuelDelta = _currentFuelCount.Value - fuelCountValueAtEndOfLastTurn;
        DialogueLua.SetVariable("FuelSpent", fuelDelta < 0 ? -fuelDelta : 0);
        var foodDelta = _currentFoodCount.Value - foodCountValueAtEndOfLastTurn;
        DialogueLua.SetVariable("FoodSpent", foodDelta < 0 ? -foodDelta : 0);
    }

    public void MarkEndOfTurnValues() {
        personCountValueAtEndOfLastTurn = _currentPersonCount.Value;
        fuelCountValueAtEndOfLastTurn = _currentFuelCount.Value;
        foodCountValueAtEndOfLastTurn = _currentFoodCount.Value;
    }
#endregion
    }
}