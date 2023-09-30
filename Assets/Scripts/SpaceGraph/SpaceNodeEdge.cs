using System;
using QuikGraph;

namespace BML.Scripts.SpaceGraph
{
    [Serializable]
    public class SpaceNodeEdge : IEdge<SpaceNode>
    {
        public SpaceGraph ParentGraph { get; }
        public SpaceNode Source { get; }
        public SpaceNode Target { get; }

        public SpaceNodeEdge(SpaceGraph parentGraph, SpaceNode source, SpaceNode target)
        {
            ParentGraph = parentGraph;
            Source = source;
            Target = target;
        }
    }
}