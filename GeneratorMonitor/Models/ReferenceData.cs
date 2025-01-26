using System.Xml.Serialization;

namespace GeneratorMonitor.Models;

[XmlRoot("ReferenceData")]
public class ReferenceData
{

    [XmlElement("Factors")]
    public Factors Factors { get; set; }
}

public class Factors
{

    [XmlElement("ValueFactor")]
    public ValueFactor ValueFactor { get; set; }

    [XmlElement("EmissionsFactor")]
    public EmissionsFactor EmissionsFactor { get; set; }
}

public abstract class Factor
{

    [XmlElement("High")]
    public double High { get; set; }


    [XmlElement("Medium")]
    public double Medium { get; set; }

    [XmlElement("Low")]
    public double Low { get; set; }

    [XmlIgnore]
    public Dictionary<string,double> Levels => new Dictionary<string, double> 
    { 
        { "Low", Low }, {"Medium", Medium}, {"High", High}
    };
            
    
}

public class ValueFactor : Factor
{

}

public class EmissionsFactor : Factor
{

}

