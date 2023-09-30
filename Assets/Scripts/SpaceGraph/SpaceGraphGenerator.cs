using System;
using System.Collections.Generic;
using System.Linq;
using AwesomeNamespace;
using BML.Scripts.CaveV2;
using BML.Scripts.Utils;
using MIConvexHull;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace BML.Scripts.SpaceGraph
{
    public class SpaceGraphGenerator : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Bounds _bounds;
        
        [TitleGroup("Poisson")]
        [SerializeField, Range(0f, 30f)] private float _minimumDistance = 2f;
        [SerializeField, Range(0, 100)] private int _pointsPerIteration = 30;

        [TitleGroup("Delaunay")]
        [SerializeField] private float _planeDistanceTolerance;

        [ShowInInspector] private SpaceGraph _spaceGraph;

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
            Debug.Log("START GENERATE");
            Generate();
            Debug.Log("FINISH GENERATE");
        }

        private List<Vector2> _samples;
        private DelaunayTriangulation<DefaultVertex, DefaultTriangulationCell<DefaultVertex>> _triangulation;

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(_bounds.center, _bounds.size);

            // if (_samples != null)
            // {
            //     Gizmos.color = new Color(0.2f, 0.9f, 0.7f);
            //     
            //     foreach (var vector2 in _samples)
            //     {
            //         var worldPosition = _bounds.center + vector2.xoy();
            //         Gizmos.DrawSphere(worldPosition, 0.5f);
            //     }
            // }
            //
            // if (_triangulation != null)
            // {
            //     Gizmos.color = new Color(0.2f, 0.9f, 0.7f);
            //     
            //     foreach (var cell in _triangulation.Cells)
            //     {
            //         for (int i = 0; i < cell.Vertices.Length - 1; i++)
            //         {
            //             var start = cell.Vertices[i];
            //             var end = cell.Vertices[i + 1];
            //
            //             var startPosition = VectorUtils.Vector2FromDoubleArray(start.Position).xoy();
            //             var endPosition = VectorUtils.Vector2FromDoubleArray(end.Position).xoy();
            //             Gizmos.DrawLine(startPosition, endPosition);
            //         }
            //     }
            // }

            if (_spaceGraph != null)
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
            foreach (var cell in triangulation.Cells)
            {
                var cellVertices = cell.Vertices.Select(v => new SpaceNode(
                        VectorUtils.Vector2FromDoubleArray(v.Position).xoy(),
                        EnumUtils.Random<PopulationDensity>(Random.value),
                        EnumUtils.Random<SoilFertility>(Random.value),
                        EnumUtils.Random<FuelSupply>(Random.value),
                        EnumUtils.Random<Disposition>(Random.value),
                        Random.value
                    )).ToList();
                var cellEdges = cellVertices.Where((node, i) => i < cellVertices.Count - 1)
                    .Select((node, i) => new SpaceNodeEdge(node, cellVertices[i+1]));
                
                _spaceGraph.AddVerticesAndEdgeRange(cellEdges);
            }
            
            
        }

        #endregion
    }
}