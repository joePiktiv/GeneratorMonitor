using System.Xml.Serialization;

namespace GeneratorMonitor.Models;

[XmlRoot("GenerationReport")]
public class GenerationReport
{
    [XmlElement("Wind")]
    public Wind? Wind { get; set; }

    [XmlElement("Gas")]
    public Gas? Gas { get; set; }

    [XmlElement("Coal")]
    public Coal? Coal { get; set; }

}
public class Wind
{
    [XmlElement("WindGenerator")]
    public List<WindGenerator>? WindGenerators { get; set; }
}

public class Gas
{
    [XmlElement("GasGenerator")]
    public List<GasGenerators>? GasGenerators { get; set; }
}

public class Coal
{
    [XmlElement("CoalGenerator")]
    public List<CoalGenerator>? CoalGenerators { get; set; }
}