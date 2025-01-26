using GeneratorMonitor.Models;

namespace GeneratorMonitor.Services;

public class Report
{
    public static void GenerateEnergyReport(GenerationReport report, ReferenceData reference, AllFactors factors, string outputPath)
    {
        var output = new Output(report, reference, factors);
        FileHandler.OutputFile(output, outputPath);
    }
}
