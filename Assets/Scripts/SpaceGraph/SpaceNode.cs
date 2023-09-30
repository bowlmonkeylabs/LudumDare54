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

    public class SpaceNode
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
    }
}