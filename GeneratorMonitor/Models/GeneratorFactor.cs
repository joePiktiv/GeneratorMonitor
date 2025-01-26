using CsvHelper.Configuration;

namespace GeneratorMonitor.Models;

public class AllFactors
{
    public List<GeneratorFactor> Collections { get; set; }

    public double GetValueFactor(string type, ReferenceData data)
    {
        try
        {
            var key = ConvertToGeneratorType(type);
            var level = Collections.Find(c => key == c.GeneratorType).ValueFactor;
            return data.Factors.ValueFactor.Levels[level];
        } catch
        {
            throw new ArgumentException($"Invalid type: {type}");
        }
    }

    public double GetEmissionFactor(string type, ReferenceData data)
    {
        try
        {
            var key = ConvertToGeneratorType(type);
            var level = Collections.Find(c => key == c.GeneratorType).EmissionFactor;
            return data.Factors.EmissionsFactor.Levels[level];
        }
        catch
        {
            throw new ArgumentException($"Invalid type: {type}");
        }
    }
    public static string ConvertToGeneratorType (string input)
    {
        if (string.IsNullOrEmpty(input)) return "unknown"; 
        if (input.StartsWith("Coal")) return "Coal";
        if (input.StartsWith("Gas")) return "Gas";
        if (input.StartsWith("Wind[Offshore]")) return "Offshore Wind";
        if (input.StartsWith("Wind[Onshore]")) return "Onshore Wind";
        return "unknown";
    }

}


public class GeneratorFactor
{
    public string? GeneratorType { get; set; }
    public required string ValueFactor { get; set; }
    public string EmissionFactor { get; set; }
}

public class GeneratorMap : ClassMap<GeneratorFactor>
{
    public GeneratorMap()
    {
        Map(m => m.GeneratorType).Name("Generator Type");
        Map(m => m.ValueFactor).Name("ValueFactor");
        Map(m => m.EmissionFactor).Name("EmissionFactor");
    }
}