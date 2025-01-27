using System.Xml.Serialization;

namespace GeneratorMonitor.Models;

public class WindGenerator : EnergyGenerator
{

    [XmlElement("Location")]
    public string? Location { get; set; }
}

public class GasGenerators : EnergyGenerator
{

    [XmlElement("EmissionsRating")]
    public double EmissionsRating { get; set; }
}

public class CoalGenerator : GasGenerators
{

    [XmlElement("TotalHeatInput")]
    public double TotalHeatInput { get; set; }


    [XmlElement("ActualNetGeneration")]
    public double ActualNetGeneration { get; set; }

}

