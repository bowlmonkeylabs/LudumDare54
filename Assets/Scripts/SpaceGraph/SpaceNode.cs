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
        public Vector3 LocalPosition;
        public PopulationDensity PopulationDensity;
        public SoilFertility SoilFertility;
        public FuelSupply FuelSupply;
        public Disposition Disposition;
        public float RandomRoll;

        public SpaceNode(Vector3 localPosition, float randomRoll)
        {
            LocalPosition = localPosition;
            RandomRoll = randomRoll;

            this.PopulationDensity = EnumUtils.Random<PopulationDensity>(randomRoll);
            this.PopulationDensity = EnumUtils.Random<PopulationDensity>(randomRoll);
        }
        
        public SpaceNode(Vector3 localPosition, PopulationDensity populationDensity, SoilFertility soilFertility, FuelSupply fuelSupply, Disposition disposition, float randomRoll)
        {
            LocalPosition = localPosition;
            PopulationDensity = populationDensity;
            SoilFertility = soilFertility;
            FuelSupply = fuelSupply;
            Disposition = disposition;
            RandomRoll = randomRoll;
        }

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
    }
}