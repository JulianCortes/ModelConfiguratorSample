using System.Collections.Generic;

namespace Bunny_TK.ModelConfigurator.Samples
{
    /// <summary>
    /// "Hardcoded" implementation sample of a configuration.
    /// </summary>
    public class SampleConfigurationID : ConfigurationIDBase
    {
        public SampleUtility.BodyColor bodyColor;
        public SampleUtility.InteriorColor interiorColor;
        public SampleUtility.RimType rimType;

        public override bool Similar(ConfigurationIDBase other)
        {
            if (other == null) return false;
            if (other.GetType() != typeof(SampleConfigurationID)) return false; //Se other non è lo stesso tipo
            if (other == this) return true;                                     //Se other è me

            SampleConfigurationID otherConfig = other as SampleConfigurationID;

            //TAVOLA DI VERITA'
            //UNDEF == UNDEF = TRUE
            //A == UNDEF = TRUE
            //A == B = FALSE

            if (otherConfig.bodyColor != SampleUtility.BodyColor.Undefined &&
                            bodyColor != SampleUtility.BodyColor.Undefined &&
                            bodyColor != otherConfig.bodyColor)
                return false;

            if (otherConfig.interiorColor != SampleUtility.InteriorColor.Undefined &&
                            interiorColor != SampleUtility.InteriorColor.Undefined &&
                            interiorColor != otherConfig.interiorColor)
                return false;

            if (otherConfig.rimType != SampleUtility.RimType.Undefined &&
                            rimType != SampleUtility.RimType.Undefined &&
                            rimType != otherConfig.rimType)
                return false;

            return true;
        }
        public override bool Same(ConfigurationIDBase other)
        {
            if (!Similar(other)) return false;

            SampleConfigurationID otherID = other as SampleConfigurationID;

            if (otherID.bodyColor != bodyColor) return false;
            if (otherID.interiorColor != interiorColor) return false;
            if (otherID.rimType != rimType) return false;

            return true;
        }
        public override void Overlap(ConfigurationIDBase other)
        {
            if (!Similar(other)) return;

            SampleConfigurationID otherConfig = other as SampleConfigurationID;

            //Ignora i valori Undefined

            if (otherConfig.bodyColor != SampleUtility.BodyColor.Undefined)
                bodyColor = otherConfig.bodyColor;

            if (otherConfig.interiorColor != SampleUtility.InteriorColor.Undefined)
                interiorColor = otherConfig.interiorColor;

            if (otherConfig.rimType != SampleUtility.RimType.Undefined)
                rimType = otherConfig.rimType;
        }

        public override IEnumerable<string> GetAllTypes()
        {
            return SampleUtility.GetAllTypes();
        }
        public override IEnumerable<string> GetAllValues(int typeIndex)
        {
            return SampleUtility.GetAllValues(typeIndex);
        }
    }
}