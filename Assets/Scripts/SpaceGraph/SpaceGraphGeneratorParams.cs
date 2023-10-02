using BML.ScriptableObjectCore.Scripts.Variables.SafeValueReferences;
using BML.Utils.Random;
using Sirenix.OdinInspector;
using UnityEngine;

namespace BML.Scripts.SpaceGraph
{
    [CreateAssetMenu(fileName = "SpaceGraphGeneratorParams", menuName = "BML/SpaceGraphGeneratorParams")]
    public class SpaceGraphGeneratorParams : ScriptableObject
    {
        #region Inspector
        
        [TitleGroup("1) Poisson")]
        [SerializeField] public Bounds Bounds;
        [SerializeField, Range(0f, 30f)] public float MinimumDistance = 2f;
        [SerializeField, Range(0, 100)] public int PointsPerIteration = 30;

        [TitleGroup("2) Delaunay")]
        [SerializeField] public float PlaneDistanceTolerance;

        [TitleGroup("3) Assign attributes to each node")]
        [SerializeField] public WeightedValueOptions<PopulationDensity> WeightedOptionsPopulationDensity;
        [SerializeField] public WeightedValueOptions<SoilFertility> WeightedOptionsSoilFertility;
        [SerializeField] public WeightedValueOptions<FuelSupply> WeightedOptionsFuelSupply;
        [SerializeField] public WeightedValueOptions<Disposition> WeightedOptionsDisposition;

        [TitleGroup("4) Render to Scene")]
        [SerializeField] public SafeTransformValueReference SceneContainer;
        [SerializeField, AssetsOnly] public GameObject SpaceNodePrefab;
        [SerializeField, AssetsOnly] public GameObject SpaceNodeEdgePrefab;
        [SerializeField, AssetsOnly] public GameObject StartNodePrefab;
        [SerializeField, AssetsOnly] public GameObject EndNodePrefab;

        [SerializeField, FoldoutGroup("Debug", expanded: false)] public bool EnableLogs = false;
        [SerializeField, FoldoutGroup("Debug")] public bool EnableGizmos = true;
        [SerializeField, FoldoutGroup("Debug")] public bool ShowSamplePoints;
        [SerializeField, FoldoutGroup("Debug")] public bool ShowTriangulation;
        [SerializeField, FoldoutGroup("Debug")] public bool ShowResultGraph = true;

        #endregion
    }
}