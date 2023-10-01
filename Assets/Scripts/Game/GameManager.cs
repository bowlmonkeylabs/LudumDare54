using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BML.Scripts.SpaceGraph;
using UnityEngine.SceneManagement;
using BML.ScriptableObjectCore.Scripts.Variables;
using BML.Scripts.CaveV2;

namespace BML.Scripts.Game {
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private IntVariable _peopleAmount;
        [SerializeField] private IntVariable _foodAmount;
        [SerializeField] private FloatReference _foodRequiredPerPerson;
        [SerializeField] private FloatReference _chanceToAddFuelOnWait;
        [SerializeField] private SpaceNodeReference _currentSpaceNode;

        [SerializeField] private UnityEvent _onProcessSpaceNodeAttributes;
        [SerializeField] private UnityEvent _onFuelAddedSuccess;
        [SerializeField] private UnityEvent _onPeopleReachesZero;
        [SerializeField] private UnityEvent _onNotEnoughFood;
        [SerializeField] private UnityEvent _onReachedEndNode;
        
        void OnEnable() {
            _peopleAmount.Subscribe(CheckZeroPeople);
            // _foodAmount.Subscribe();
        }

        void OnDisable() {
            _peopleAmount.Unsubscribe(CheckZeroPeople);
            // _foodAmount.Unsubscribe();
        }

        public void ProcessSpaceNodeAttributes(object _spaceNodeTransform) {
            var spaceNodeComp = (_spaceNodeTransform as Transform).GetComponent<SpaceNodeComponent>();
            if(spaceNodeComp != null) {
                var spaceNode = spaceNodeComp.SpaceNode;
                if(spaceNode.IsStartNode()) {
                    return;
                }
                if(spaceNode.IsEndNode()) {
                    _onReachedEndNode.Invoke();
                    return;
                }

                _currentSpaceNode.Value = spaceNodeComp;

                _onProcessSpaceNodeAttributes.Invoke();
            }
        }

        public void CloseDialogueScene() {
            if(SceneManager.GetSceneByName("DialogueScene").isLoaded) {
                SceneManager.UnloadSceneAsync("DialogueScene");
            }
        }

        public void ChanceToAddFuel() {
            SeedManager.Instance.UpdateSteppedSeed("FuelChance");
            Random.InitState(SeedManager.Instance.GetSteppedSeed("FuelChance"));
            if(Random.value <= _chanceToAddFuelOnWait.Value) {
                _onFuelAddedSuccess.Invoke();
            }
        }

        public void CheckEnoughFoodForPeople() {
            float neededAmountOfFood = _foodRequiredPerPerson.Value * _peopleAmount.Value;
            if(_foodAmount.Value < neededAmountOfFood) {
                //people starve, invoke with amount of people starved
                _onNotEnoughFood.Invoke();
            }
        }

        private void CheckZeroPeople() {
            if(_peopleAmount.Value <= 0) {
                _onPeopleReachesZero.Invoke();
            }
        }
    }
}
