using System;
using QuikGraph;

namespace BML.Scripts.SpaceGraph
{
    [Serializable]
    public class SpaceNodeEdge : IEdge<SpaceNode>
    {
        public SpaceNode Source { get; }
        public SpaceNode Target { get; }

        public SpaceNodeEdge(SpaceNode source, SpaceNode target)
        {
            Source = source;
            Target = target;
        }
    }
}