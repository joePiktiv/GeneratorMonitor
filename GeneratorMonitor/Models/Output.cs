using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GeneratorMonitor.Models;

[XmlRoot("GenerationOutput")]
public class Output
{

    [XmlElement("Totals")]
    public Totals? Totals { get; set; }

    [XmlElement("MaxEmissionGenerators")]
    public MaxEmissionGenerators? MaxEmissionGenerators { get; set; }

    [XmlElement("ActualHeatRates")]
    public ActualHeatRates? ActualHeatRates { get; set; }
    public Output () { }

    public Output (GenerationReport report, ReferenceData data, AllFactors factors)
    {
        Totals = new Totals ();
        Totals.Generators =
                report.Coal!.CoalGenerators!.Select(g => new GeneratorTotal(g, data, factors))
                    .Concat(report.Gas!.GasGenerators!.Select(g => new GeneratorTotal(g, data, factors)))
                    .Concat(report.Wind!.WindGenerators!.Select(g => new GeneratorTotal(g, data, factors)))
                    .ToList();

        MaxEmissionGenerators = new MaxEmissionGenerators();
        MaxEmissionGenerators.Day =
                report.Coal!.CoalGenerators!.SelectMany(g => g.Generations.SelectMany(gg => gg.Days!.Select(d => new OutputDay(g, d, data, factors))))
                    .Concat(report.Gas.GasGenerators!.SelectMany(g => g.Generations.SelectMany(gg => gg.Days!.Select(d => new OutputDay(g, d, data, factors)))))
                    .ToList();
        ActualHeatRates = new ActualHeatRates();
        ActualHeatRates.CoalGenerator = 
                report.Coal.CoalGenerators!.Select(g => new CoalGeneratorOutput() { Name = g.Name, HeatRate = g.TotalHeatInput / g.ActualNetGeneration})
                    .ToList();
    }
}

public class Totals
{
    [XmlElement("Generator")]
    public List<GeneratorTotal>? Generators { get; set; }
}

public class GeneratorTotal
{
    [XmlElement("Name")]
    public string? Name { get; set; }

    [XmlElement("Total")]
    public double Total { get; set; }
    public GeneratorTotal()
    {
    }
    public GeneratorTotal(EnergyGenerator generator, ReferenceData data, AllFactors factors)
    {
        Name = generator.Name;
        var factor = factors.GetValueFactor(Name, data);
        Total = generator.TotalGeneration * factor;  
    }
}

public class MaxEmissionGenerators
{
    [XmlElement("Day")]
    public List<OutputDay>? Day { get; set; }
}

public class OutputDay
{
    [XmlElement("Name")]
    public string? Name { get; set; }

    [XmlElement("Date")]
    public DateTime? Date { get; set; }

    [XmlElement("Emission")]
    public double Emission { get; set; }
    public OutputDay()
    {
    
    }
    public OutputDay(GasGenerators generator, Day day, ReferenceData data, AllFactors factors)
    {
        Name = generator.Name;
        Date = day.Date;
        var factor = factors.GetEmissionFactor(Name, data);
        Emission = day.Energy * generator.EmissionsRating * factor;
    }
}

public class ActualHeatRates
{
    [XmlElement("CoalGenerator")]
    public List<CoalGeneratorOutput>? CoalGenerator { get; set; }
}

public class CoalGeneratorOutput
{

    [XmlElement("Name")]
    public required string Name { get; set; }

    [XmlElement("HeatRate")]
    public double HeatRate { get; set; }
}


