using System.Xml.Serialization;

namespace GeneratorMonitor.Models;

public abstract class EnergyGenerator
{

    [XmlElement("Name")]
    public string Name { get; set; }

    [XmlElement("Generation")]
    public required List<Generation> Generations { get; set; }

    [XmlIgnore]
    public double TotalGeneration => Generations.Sum(g => g.Days.Sum(d => d.TotalGeneration));
}

public class Generation
{
    [XmlElement("Day")]
    public List<Day> Days { get; set; }
}

public class Day
{
    [XmlElement("Date")]
    public DateTime Date { get; set; }

    [XmlElement("Energy")]
    public double Energy { get; set; }

    [XmlElement("Price")]
    public double Price { get; set; }

    [XmlIgnore]
    public double TotalGeneration => Energy * Price ;
}