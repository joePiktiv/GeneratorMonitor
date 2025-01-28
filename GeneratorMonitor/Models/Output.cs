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

    public Output() { }

    public Output(GenerationReport report, ReferenceData data, AllFactors factors)
    {
        try
        {
            Totals = new Totals(report, data, factors);

            MaxEmissionGenerators = new MaxEmissionGenerators(report, data, factors);

            ActualHeatRates = new ActualHeatRates(report);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Error while processing generation data", ex);
        }
    }
}

public class Totals
{
    [XmlElement("Generator")]
    public List<GeneratorTotal> Generators { get; set; } = new List<GeneratorTotal>();

    public Totals() { }

    public Totals(GenerationReport report, ReferenceData data, AllFactors factors)
    {
        Generators = GetTotalEnergies(report, data, factors);
    }

    private List<GeneratorTotal> GetTotalEnergies(
        GenerationReport report,
        ReferenceData data,
        AllFactors factors
    )
    {
        return GetEnergies(report.Coal!.CoalGenerators!, data, factors)!
            .Concat(GetEnergies(report.Gas!.GasGenerators!, data, factors)!)
            .Concat(GetEnergies(report.Wind!.WindGenerators!, data, factors)!)
            .ToList();
    }

    private IEnumerable<GeneratorTotal>? GetEnergies(
        IEnumerable<EnergyGenerator> generators,
        ReferenceData data,
        AllFactors factors
    )
    {
        return generators.Select(g => new GeneratorTotal(g, data, factors)).ToList();
    }
}

public class GeneratorTotal
{
    [XmlElement("Name")]
    public string? Name { get; set; }

    [XmlElement("Total")]
    public double? Total { get; set; }

    public GeneratorTotal() { }

    public GeneratorTotal(EnergyGenerator generator, ReferenceData data, AllFactors factors)
    {
        Name = generator.Name!;
        var factor = factors.GetValueFactor(Name, data);
        Total = generator.TotalGeneration * factor;
    }
}

public class MaxEmissionGenerators
{
    [XmlElement("Day")]
    public List<OutputDay>? Day { get; set; }

    public MaxEmissionGenerators() { }

    public MaxEmissionGenerators(GenerationReport report, ReferenceData data, AllFactors factors)
    {
        Day = GetDays(report.Coal!.CoalGenerators!, data, factors)
            .Concat(GetDays(report.Gas!.GasGenerators!, data, factors))
            .ToList();
    }

    private IEnumerable<OutputDay> GetDays(
        IEnumerable<GasGenerators> generators,
        ReferenceData data,
        AllFactors factors
    )
    {
        return generators.SelectMany(g =>
            g.Generations.SelectMany(gg => gg.Days!.Select(d => new OutputDay(g, d, data, factors)))
        );
    }
}

public class OutputDay
{
    [XmlElement("Name")]
    public string? Name { get; set; }

    [XmlElement("Date")]
    public DateTime? Date { get; set; }

    [XmlElement("Emission")]
    public double? Emission { get; set; }

    public OutputDay() { }

    public OutputDay(GasGenerators generator, Day day, ReferenceData data, AllFactors factors)
    {
        Name = generator.Name!;
        Date = day.Date;
        var factor = factors.GetEmissionFactor(Name, data);
        Emission = day.Energy * generator.EmissionsRating * factor;
    }
}

public class ActualHeatRates
{
    [XmlElement("CoalGenerator")]
    public List<CoalGeneratorOutput>? CoalGenerator { get; set; }

    public ActualHeatRates() { }

    public ActualHeatRates(GenerationReport report)
    {
        CoalGenerator = GetActualHeatRates(report);
    }

    private List<CoalGeneratorOutput>? GetActualHeatRates(GenerationReport report)
    {
        return report
            .Coal!.CoalGenerators!.Select(g => new CoalGeneratorOutput()
            {
                Name = g.Name!,
                HeatRate = g.TotalHeatInput / g.ActualNetGeneration
            })
            .ToList();
    }
}

public class CoalGeneratorOutput
{
    [XmlElement("Name")]
    public string? Name { get; set; }

    [XmlElement("HeatRate")]
    public double? HeatRate { get; set; }
}
