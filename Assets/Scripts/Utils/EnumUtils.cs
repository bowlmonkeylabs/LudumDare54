using Mono.CSharp;
using UnityEngine;

namespace BML.Scripts.Utils
{
    public static class EnumUtils
    {
        public static T Random<T>(float randomRoll) where T : System.Enum
        {
            #warning this may favor the last value slightly more?
            var values = System.Enum.GetValues(typeof(T));
            int randomIndex = Mathf.FloorToInt(randomRoll * values.Length);
            var randomValue = (T)values.GetValue(randomIndex);
            return randomValue;
        }
    }
}