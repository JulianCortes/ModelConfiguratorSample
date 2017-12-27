using System.Collections.Generic;

namespace Bunny_TK.ModelConfigurator.Samples
{
    /// <summary>
    /// Used as definitions in "hardcoded" samples.
    /// </summary>
    public class SampleUtility
    {
        //Definitions are hardcoded in enums.

        public enum BodyColor
        {
            Undefined = int.MaxValue,
            Red = 0,
            Blue = 1,
            Green = 2
        }
        public enum InteriorColor
        {
            Undefined = int.MaxValue,
            Black = 0,
            White = 1
        }
        public enum RimType
        {
            Undefined = int.MaxValue,
            A = 0,
            B = 1
        }

        public static IEnumerable<string> GetAllTypes()
        {
            return new string[] { "BodyColor", "InteriorColor", "RimTYpe" };
        }
        public static IEnumerable<string> GetAllValues(int indexType)
        {
            if (indexType < 0) return new string[] { };
            if (indexType == 0)
                return new string[] { "Red", "Blue", "Green" };
            else if (indexType == 1)
                return new string[] { "Black", "White" };
            else if (indexType == 2)
                return new string[] { "A", "B" };
            return new string[] { };
        }
    }
}