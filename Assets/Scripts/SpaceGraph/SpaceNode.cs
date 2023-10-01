using System;
using BML.Scripts.Utils;
using UnityEngine;

namespace BML.Scripts.SpaceGraph
{
    public enum PopulationDensity
    {
        Rural,
        Suburban,
        Urban,
    }

    public enum SoilFertility
    {
        Arid,
        Meager,
        Fertile,
    }
    
    public enum FuelSupply
    {
        None,
        WillingToTrade,
        Excess,
    }

    public enum Disposition
    {
        Stingy,
        Generous,
    }

    [Serializable]
    public class SpaceNode : IEquatable<SpaceNode>
    {
        // Core properties. Set on Init and shouldn't be modified after.
        public SpaceGraph ParentGraph { get; }
        public Vector3 LocalPosition { get; }
        public float RandomRoll { get; }
        public PopulationDensity PopulationDensity { get; }
        public SoilFertility SoilFertility { get; }
        public FuelSupply FuelSupply { get; }
        public Disposition Disposition { get; }
        
        // Runtime data. Clear on Init.
        public int PlayerDistance { get; set; } = -1;
        public bool PlayerOccupied { get; set; } = false;
        public bool PlayerOccupiedAdjacent { get; set; } = false;
        public bool PlayerVisited { get; set; } = false;
        public bool PlayerVisitedAdjacent { get; set; } = false;

        public delegate void _OnUpdate();

        public event _OnUpdate OnUpdate;
        public void InvokeUpdate() => OnUpdate?.Invoke();

        public SpaceNode(SpaceGraph parentGraph, Vector3 localPosition, float randomRoll)
        {
            ParentGraph = parentGraph;
            LocalPosition = localPosition;
            RandomRoll = randomRoll;

            this.PopulationDensity = EnumUtils.Random<PopulationDensity>(randomRoll);
            this.PopulationDensity = EnumUtils.Random<PopulationDensity>(randomRoll);
        }
        
        public SpaceNode(SpaceGraph parentGraph, Vector3 localPosition, PopulationDensity populationDensity, SoilFertility soilFertility, FuelSupply fuelSupply, Disposition disposition, float randomRoll)
        {
            ParentGraph = parentGraph;
            LocalPosition = localPosition;
            PopulationDensity = populationDensity;
            SoilFertility = soilFertility;
            FuelSupply = fuelSupply;
            Disposition = disposition;
            RandomRoll = randomRoll;
        }

        #region IEquatable

        public bool Equals(SpaceNode other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return LocalPosition.Equals(other.LocalPosition);
            // return LocalPosition.Equals(other.LocalPosition) && PopulationDensity == other.PopulationDensity && SoilFertility == other.SoilFertility && FuelSupply == other.FuelSupply && Disposition == other.Disposition && RandomRoll.Equals(other.RandomRoll);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SpaceNode)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = LocalPosition.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)PopulationDensity;
                hashCode = (hashCode * 397) ^ (int)SoilFertility;
                hashCode = (hashCode * 397) ^ (int)FuelSupply;
                hashCode = (hashCode * 397) ^ (int)Disposition;
                hashCode = (hashCode * 397) ^ RandomRoll.GetHashCode();
                return hashCode;
            }
        }

        #endregion

        #region Parent graph helpers
        // Helper functions that work with the parent graph data

        public bool IsStartNode()
        {
            return this.Equals(ParentGraph?.Start);
        }

        public bool IsEndNode()
        {
            return this.Equals(ParentGraph?.End);
        }

        #endregion
    }
}