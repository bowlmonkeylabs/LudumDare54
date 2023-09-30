using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BML.Scripts.SpaceGraph
{
    public class SpaceNodeComponent : MonoBehaviour
    {
        #region Inspector

        [TitleGroup("Instance params")]
        [SerializeField] public SpaceNode SpaceNode;
        
        [TitleGroup("Prefab params")]
        [SerializeField] private Sphere _sphere;

        #endregion

        #region Unity lifecycle

        

        #endregion

        #region Public interface

        public void Init(SpaceNode spaceNode)
        {
            this.SpaceNode = spaceNode;
            
            // Position and align
            this.transform.localPosition = spaceNode.LocalPosition;
            this.transform.rotation = Quaternion.identity;
        }

        #endregion
    }
}