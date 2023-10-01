using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using BML.Scripts.SpaceGraph;
using UnityEngine.SceneManagement;

namespace BML.Scripts.Game {
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onProcessSpaceNodeAttributes;

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
    }
}
