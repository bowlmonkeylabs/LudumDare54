using Shapes;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BML.Scripts.SpaceGraph
{
    public class SpaceNodeEdgeComponent : MonoBehaviour
    {
        #region Inspector

        [TitleGroup("Instance params")]
        [SerializeField] public SpaceNodeEdge SpaceNodeEdge;

        [TitleGroup("Prefab params")]
        [SerializeField] private Line _line;

        #endregion

        #region Unity lifecycle

        

        #endregion

        #region Public interface

        public void Init(SpaceNodeEdge spaceNodeEdge)
        {
            this.SpaceNodeEdge = spaceNodeEdge;

            // Position and align
            var edgeDiff = (spaceNodeEdge.Target.LocalPosition - spaceNodeEdge.Source.LocalPosition);
            var edgeRotation = Quaternion.LookRotation(edgeDiff);
            this.transform.localPosition = spaceNodeEdge.Source.LocalPosition;
            this.transform.rotation = edgeRotation;
            _line.Start = Vector3.zero;
            _line.End = new Vector3(0, 0, edgeDiff.magnitude);
        }

        #endregion
    }
}