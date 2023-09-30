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
        
        [TitleGroup("1) Poisson")]
        [SerializeField] private Bounds _bounds;
        [SerializeField, Range(0f, 30f)] private float _minimumDistance = 2f;
        [SerializeField, Range(0, 100)] private int _pointsPerIteration = 30;

        [TitleGroup("2) Delaunay")]
        [SerializeField] private float _planeDistanceTolerance;

        [TitleGroup("3) Render to Scene")]
        [SerializeField] private SafeTransformValueReference _sceneContainer;
        [SerializeField, AssetsOnly] private GameObject _spaceNodePrefab;
        [SerializeField, AssetsOnly] private GameObject _spaceNodeEdgePrefab;
        [SerializeField, AssetsOnly] private GameObject _startNodePrefab;
        [SerializeField, AssetsOnly] private GameObject _endNodePrefab;

        [TitleGroup("Result")]
        [SerializeField, ReadOnly]
        // [ShowInInspector]
        private SpaceGraph _spaceGraph;

        [SerializeField, FoldoutGroup("Debug", expanded: false)] private bool _enableLogs = false;
        [SerializeField, FoldoutGroup("Debug")] private bool _enableGizmos = true;
        [SerializeField, FoldoutGroup("Debug")] private bool _showSamplePoints;
        [SerializeField, FoldoutGroup("Debug")] private bool _showTriangulation;
        [SerializeField, FoldoutGroup("Debug")] private bool _showResultGraph = true;

        [Button("Generate")]
        private void GenerateButton()
        {
            SeedManager.Instance.UpdateRandomSeed();
            #warning should the 'game manager' manager the base seed, not the level?
            
            Generate();
        }

        #endregion

        #region Unity lifecycle

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
            if (!_enableGizmos) return;
            
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(_bounds.center, _bounds.size);
        }

        private void OnDrawGizmosSelected()
        {
            if (!_enableGizmos)
            {
                // Always draw bounds when selected
                Gizmos.color = Color.white;
                Gizmos.DrawWireCube(_bounds.center, _bounds.size);

                return;
            }

            if (_showSamplePoints && _samples != null)
            {
                Gizmos.color = Color.yellow;
                foreach (var vector2 in _samples)
                {
                    var worldPosition = _bounds.center + vector2.xoy();
                    Gizmos.DrawSphere(worldPosition, 0.5f);
                }
            }
            
            if (_showTriangulation && _triangulation != null)
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

            if (_showResultGraph && _spaceGraph != null)
            {
                Gizmos.color = new Color(0.2f, 0.9f, 0.7f);
                foreach (var vertex in _spaceGraph.Vertices)
                {
                    var worldPosition = _bounds.center + vertex.LocalPosition;
                    Gizmos.DrawSphere(worldPosition, 0.5f);
                }
                
                Gizmos.color = new Color(0.2f, 0.9f, 0.7f);
                foreach (var edge in _spaceGraph.Edges)
                {
                    var startPosition = _bounds.center + edge.Source.LocalPosition;
                    var endPosition = _bounds.center + edge.Target.LocalPosition;
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
            var samples = UniformPoissonDiskSampler.SampleRectangle(_bounds.min.xz(), _bounds.max.xz(), _minimumDistance,
                _pointsPerIteration);
            _samples = samples; // DEBUG
            
            // TODO remove some points to make the level less uniform
            // TODO ensure the level is traversable from 'start' to 'end'

            // Generate connections between adjacent points
            var vertices = _samples.Select(v => new DefaultVertex { Position = v.ToDoubleArray() }).ToList();
            var triangulation = MIConvexHull.DelaunayTriangulation<DefaultVertex, DefaultTriangulationCell<DefaultVertex>>.Create(vertices, _planeDistanceTolerance);
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
                    EnumUtils.Random<PopulationDensity>(Random.value),
                    EnumUtils.Random<SoilFertility>(Random.value),
                    EnumUtils.Random<FuelSupply>(Random.value),
                    EnumUtils.Random<Disposition>(Random.value),
                    Random.value
                )).ToList();;
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
            if (_sceneContainer.Value == null)
            {
                Debug.LogError("No scene container assigned.");
                return;
            }
            
            GameObjectUtils.DestroyChildren(_sceneContainer.Value);
        }

        [Button]
        private void GenerateSceneObjects()
        {
            DestroySceneObjects();

            if (_sceneContainer.Value == null)
            {
                Debug.LogError("No scene container assigned.");
                return;
            }
            
            // Spawn objects for vertices
            foreach (var spaceNode in _spaceGraph.Vertices)
            {
                // Pick object to spawn (normal node, start node, end node, etc.)
                GameObject prefabToSpawn = _spaceNodePrefab;
                if (spaceNode.IsStartNode())
                {
                    prefabToSpawn = _startNodePrefab;
                } 
                else if (spaceNode.IsEndNode())
                {
                    prefabToSpawn = _endNodePrefab;
                }
                
                // Instantiate object
                var spaceNodeSceneObject = GameObjectUtils.SafeInstantiate(true, prefabToSpawn, _sceneContainer.Value);
                
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
                    GameObjectUtils.SafeInstantiate(true, _spaceNodeEdgePrefab, _sceneContainer.Value);

                // Assign source SpaceNode data to the scene object
                var spaceNodeEdgeComponent = spaceNodeEdgeSceneObject.GetComponentInChildren<SpaceNodeEdgeComponent>();
                spaceNodeEdgeComponent.Init(spaceNodeEdge);
            }
        }

        #endregion

        #region Graph helper methods

        private Vector3 GetWorldPosition(SpaceNode spaceNode)
        {
            return _bounds.center + spaceNode.LocalPosition;
        }

        #endregion
    }
}