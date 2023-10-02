using System;
using System.Collections.Generic;
using System.Linq;
using Codice.Client.BaseCommands.TubeClient;
using QuikGraph;
using Sirenix.Utilities;
using UnityEditor.Graphs.AnimationBlendTree;

namespace BML.Scripts.SpaceGraph
{
    [Serializable]
    public class SpaceGraph : QuikGraph.UndirectedGraph<SpaceNode, SpaceNodeEdge>
    {
        public SpaceNode Start;
        public SpaceNode End;
        
        public SpaceGraph() : base(false)
        {
            
        }

        public void PropagatePlayerOccupied(IEnumerable<SpaceNode> playerOccupiedNodes)
        {
#warning TODO call this when the player moves
            
            FloodFillDistance(playerOccupiedNodes, (node, distanceToTarget) =>
            {
                node.PlayerDistance = distanceToTarget;
                node.PlayerOccupied = (distanceToTarget == 0);
                node.PlayerOccupiedAdjacent = (distanceToTarget == 1);
                if (node.PlayerOccupied)
                {
                    node.PlayerVisited = true;
                }
                if (node.PlayerOccupiedAdjacent)
                {
                    node.PlayerVisitedAdjacent = true;
                }
                node.InvokeUpdate();
            });
        }
        
        private void FloodFillDistance(IEnumerable<SpaceNode> targetVertices, Action<SpaceNode, int> updateStoredVertexDistance)
        {
            int distanceToTarget = 0;
            var verticesToCheck = new HashSet<SpaceNode>(targetVertices ?? Vertices);
            var verticesToCheckNext = new HashSet<SpaceNode>();
            var checkedVertices = new HashSet<SpaceNode>(verticesToCheck);

            while (verticesToCheck.Count > 0)
            {
                foreach (var vertex in verticesToCheck)
                {
                    updateStoredVertexDistance(vertex, distanceToTarget);

                    if (this.ContainsVertex(vertex))
                    {
                        var adjacentVertices = this.AdjacentVertices(vertex).Where(v => !checkedVertices.Contains(v));
                        verticesToCheckNext.AddRange(adjacentVertices);
                    }
                }
                
                verticesToCheck.Clear();
                verticesToCheck.AddRange(verticesToCheckNext);
                checkedVertices.AddRange(verticesToCheck);
                verticesToCheckNext.Clear();
                distanceToTarget += 1;
            }
        }
    }
}