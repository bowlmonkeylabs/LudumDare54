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
        [SerializeField] private FloatReference _chanceToAddFuelOnWait;
        [SerializeField] private UnityEvent _onProcessSpaceNodeAttributes;
        [SerializeField] private UnityEvent _onFuelAddedSuccess;

        public void ProcessSpaceNodeAttributes(object _spaceNodeTransform) {
            var spaceNodeComp = (_spaceNodeTransform as Transform).GetComponent<SpaceNodeComponent>();
            if(spaceNodeComp != null) {
                var spaceNode = spaceNodeComp.SpaceNode;
                if(spaceNode.IsStartNode() || spaceNode.IsEndNode()) {
                    return;
                }

                _onProcessSpaceNodeAttributes.Invoke();
            }
        }

        public void CloseDialogueScene() {
            SceneManager.UnloadSceneAsync("DialogueScene");
        }

        public void ChanceToAddFuel() {
            SeedManager.Instance.UpdateSteppedSeed("FuelChance");
            Random.InitState(SeedManager.Instance.GetSteppedSeed("FuelChance"));
            if(Random.value <= _chanceToAddFuelOnWait.Value) {
                _onFuelAddedSuccess.Invoke();
            }
        }
    }
}
