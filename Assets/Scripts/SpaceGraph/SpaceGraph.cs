using System;
using QuikGraph;

namespace BML.Scripts.SpaceGraph
{
    [Serializable]
    public class SpaceGraph : QuikGraph.UndirectedGraph<SpaceNode, SpaceNodeEdge>
    {
        public SpaceGraph() : base(false)
        {
            
        }
        
    }
}