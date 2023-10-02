using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeNamespace;
using BML.ScriptableObjectCore.Scripts.Variables.SafeValueReferences;
using BML.Scripts.CaveV2;
using BML.Scripts.Utils;
using MIConvexHull;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BML.Scripts.SpaceGraph
{
    public class SpaceGraphGenerator : MonoBehaviour
    {
        #region Inspector

        [SerializeField, InlineEditor] private SpaceGraphGeneratorParams _generatorParams;

        [TitleGroup("Result")]
        [SerializeField, ReadOnly]
        // [ShowInInspector, ReadOnly]
        private SpaceGraph _spaceGraph;

        [Button("Generate")]
        private void GenerateButton()
        {
            SeedManager.Instance.UpdateRandomSeed();
            #warning should the 'game manager' manager the base seed, not the level?
            
            Generate();
        }

        #endregion

        #region Unity lifecycle

        // private void Awake()
        // {
        //     DestroySceneObjects();
        // }

        private void Start()
        {
            // TODO
            Debug.Log("START GENERATE");
            Generate();
            Debug.Log("FINISH GENERATE");
        }

        private List<Vector2> _samples;
        private DelaunayTriangulation<DefaultVertex, DefaultTriangulationCell<DefaultVertex>> _triangulation;

        private void OnDrawGizmos()
        {
            if (!_generatorParams.EnableGizmos) return;
            
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(_generatorParams.Bounds.center, _generatorParams.Bounds.size);
        }

        private void OnDrawGizmosSelected()
        {
            if (!_generatorParams.EnableGizmos)
            {
                // Always draw bounds when selected
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(_generatorParams.Bounds.center, _generatorParams.Bounds.size);

                return;
            }

            if (_generatorParams.ShowSamplePoints && _samples != null)
            {
                Gizmos.color = Color.yellow;
                foreach (var vector2 in _samples)
                {
                    var worldPosition = _generatorParams.Bounds.center + vector2.xoy();
                    Gizmos.DrawSphere(worldPosition, 0.5f);
                }
            }
            
            if (_generatorParams.ShowTriangulation && _triangulation != null)
            {
                Gizmos.color = Color.yellow;
                foreach (var cell in _triangulation.Cells)
                {
                    for (int i = 0; i < cell.Vertices.Length - 1; i++)
                    {
                        var start = cell.Vertices[i];
                        var end = cell.Vertices[i + 1];
                        var startPosition = VectorUtils.Vector2FromDoubleArray(start.Position).xoy();
                        var endPosition = VectorUtils.Vector2FromDoubleArray(end.Position).xoy();
                        Gizmos.DrawLine(startPosition, endPosition);
                    }
                }
            }

            if (_generatorParams.ShowResultGraph && _spaceGraph != null)
            {
                Gizmos.color = new Color(0.2f, 0.9f, 0.7f);
                foreach (var vertex in _spaceGraph.Vertices)
                {
                    var worldPosition = _generatorParams.Bounds.center + vertex.LocalPosition;
                    Gizmos.DrawSphere(worldPosition, 0.5f);
                }
                
                Gizmos.color = new Color(0.2f, 0.9f, 0.7f);
                foreach (var edge in _spaceGraph.Edges)
                {
                    var startPosition = _generatorParams.Bounds.center + edge.Source.LocalPosition;
                    var endPosition = _generatorParams.Bounds.center + edge.Target.LocalPosition;
                    Gizmos.DrawLine(startPosition, endPosition);
                }
            }
        }

        #endregion

        #region Graph generation interface

        #endregion

        #region Graph generation internal

        private void Generate()
        {
            Random.InitState(SeedManager.Instance.GetSteppedSeed("SpaceGraph"));

            // Generate points
            // TODO inject fixed positions for the 'start' and 'end' points
            var samples = UniformPoissonDiskSampler.SampleRectangle(
                _generatorParams.Bounds.min.xz(), 
                _generatorParams.Bounds.max.xz(), 
                _generatorParams.MinimumDistance,
                _generatorParams.PointsPerIteration
            );
            _samples = samples; // DEBUG
            
            // TODO remove some points to make the level less uniform
            // TODO ensure the level is traversable from 'start' to 'end'

            // Generate connections between adjacent points
            var vertices = _samples.Select(v => new DefaultVertex { Position = v.ToDoubleArray() }).ToList();
            var triangulation = MIConvexHull.DelaunayTriangulation<DefaultVertex, DefaultTriangulationCell<DefaultVertex>>.Create(vertices, _generatorParams.PlaneDistanceTolerance);
            _triangulation = triangulation; // DEBUG

            // Populate our "space graph" data structure with the points and edges
            _spaceGraph = new SpaceGraph();
            var distinctCellVertices = triangulation.Cells
                .SelectMany(cell => cell.Vertices
                    .Select(v => v.Position)
                ).Distinct()
                .Select(position => new SpaceNode(
                    _spaceGraph,
                    VectorUtils.Vector2FromDoubleArray(position).xoy(),
                    _generatorParams.WeightedOptionsPopulationDensity.RandomWithWeights(Random.value),
                    _generatorParams.WeightedOptionsSoilFertility.RandomWithWeights(Random.value),
                    _generatorParams.WeightedOptionsFuelSupply.RandomWithWeights(Random.value),
                    _generatorParams.WeightedOptionsDisposition.RandomWithWeights(Random.value),
                    Random.value
                )).ToList();
            var allCellEdges = triangulation.Cells
                .SelectMany(cell => cell.Vertices
                    .Where((v, i) => i < cell.Vertices.Length - 1)
                    .Select((v, i) =>
                    {
                        var sourceVertex = v;
                        var targetVertex = cell.Vertices[i + 1];
                        
                        // TODO more efficient lookup?
                        var sourceNode = distinctCellVertices.Find(node =>
                            node.LocalPosition.Equals(VectorUtils.Vector2FromDoubleArray(sourceVertex.Position).xoy()));
                        var targetNode = distinctCellVertices.Find(node =>
                            node.LocalPosition.Equals(VectorUtils.Vector2FromDoubleArray(targetVertex.Position).xoy()));
                        
                        return new SpaceNodeEdge(_spaceGraph, sourceNode, targetNode);
                    }));
            // These methods should handle de-duping internally
            _spaceGraph.AddVertexRange(distinctCellVertices);
            _spaceGraph.AddEdgeRange(allCellEdges);
            
            // Pick the left-most node as the START
            _spaceGraph.Start = _spaceGraph.Vertices.OrderBy(node => node.LocalPosition.x).First();
            // Pick the right-most node as the END
            _spaceGraph.End = _spaceGraph.Vertices.OrderByDescending(node => node.LocalPosition.x).First();
            
            #warning TODO ensure its traversable start to end

            GenerateSceneObjects();
        }

        #endregion

        #region Spawn to scene
        
        [Button]
        private void DestroySceneObjects()
        {
            if (_generatorParams.SceneContainer.Value == null)
            {
                Debug.LogError("No scene container assigned.");
                return;
            }
            
            GameObjectUtils.DestroyChildren(_generatorParams.SceneContainer.Value);
        }

        [Button]
        private void GenerateSceneObjects()
        {
            DestroySceneObjects();

            if (_generatorParams.SceneContainer.Value == null)
            {
                Debug.LogError("No scene container assigned.");
                return;
            }
            
            // Spawn objects for vertices
            foreach (var spaceNode in _spaceGraph.Vertices)
            {
                // Pick object to spawn (normal node, start node, end node, etc.)
                GameObject prefabToSpawn = _generatorParams.SpaceNodePrefab;
                if (spaceNode.IsStartNode())
                {
                    prefabToSpawn = _generatorParams.StartNodePrefab;
                } 
                else if (spaceNode.IsEndNode())
                {
                    prefabToSpawn = _generatorParams.EndNodePrefab;
                }
                
                // Instantiate object
                var spaceNodeSceneObject = GameObjectUtils.SafeInstantiate(true, prefabToSpawn, _generatorParams.SceneContainer.Value);
                
                // Position and align
                var worldPosition = GetWorldPosition(spaceNode);
                spaceNodeSceneObject.transform.SetPositionAndRotation(worldPosition, Quaternion.identity);

                // Assign source SpaceNode data to the scene object
                var spaceNodeComponent = spaceNodeSceneObject.GetComponentInChildren<SpaceNodeComponent>();
                spaceNodeComponent.Init(spaceNode);
            }
            
            // Spawn objects for edges
            foreach (var spaceNodeEdge in _spaceGraph.Edges)
            {
                // Instantiate object
                var spaceNodeEdgeSceneObject =
                    GameObjectUtils.SafeInstantiate(true, _generatorParams.SpaceNodeEdgePrefab, _generatorParams.SceneContainer.Value);

                // Assign source SpaceNode data to the scene object
                var spaceNodeEdgeComponent = spaceNodeEdgeSceneObject.GetComponentInChildren<SpaceNodeEdgeComponent>();
                spaceNodeEdgeComponent.Init(spaceNodeEdge);
            }
        }

        #endregion

        #region Graph helper methods

        private Vector3 GetWorldPosition(SpaceNode spaceNode)
        {
            return _generatorParams.Bounds.center + spaceNode.LocalPosition;
        }

        public void PropagatePlayerOccupied(System.Object obj)
        {
            PropagatePlayerOccupied((Transform)obj);
        }

        public void PropagatePlayerOccupied(Transform spaceNodeTransform)
        {
            var spaceNode = spaceNodeTransform.GetComponent<SpaceNodeComponent>();
            _spaceGraph.PropagatePlayerOccupied(new List<SpaceNode> { spaceNode.SpaceNode });
            // spaceNode.UpdateDisplay();
        }

        #endregion
    }
}