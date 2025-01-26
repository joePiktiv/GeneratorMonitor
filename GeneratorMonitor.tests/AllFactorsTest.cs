using GeneratorMonitor.Models;
using Xunit;

public class AllFactorsTest
{
    private readonly ReferenceData _referenceData;

    public AllFactorsTest()
    {
        // Set up sample reference data
        _referenceData = new ReferenceData
        {
            Factors = new Factors
            {
                ValueFactor = new ValueFactor
                {
                    High = 3.5,
                    Medium = 2.5,
                    Low = 1.5
                },
                EmissionsFactor = new EmissionsFactor
                {
                    High = 0.8,
                    Medium = 0.5,
                    Low = 0.2
                }
            }
        };
    }

    [Fact]
    public void GetValueFactor_ValidType_ReturnsCorrectValue()
    {
        // Arrange
        var allFactors = new AllFactors
        {
            Collections = new List<GeneratorFactor>
            {
                new GeneratorFactor { GeneratorType = "Coal", ValueFactor = "Medium", EmissionFactor = "High" },
                new GeneratorFactor { GeneratorType = "Offshore Wind", ValueFactor = "Low", EmissionFactor = "N/A" }
            }
        };

        // Act
        var coalValueFactor = allFactors.GetValueFactor("Coal", _referenceData);
        var windValueFactor = allFactors.GetValueFactor("Wind[Offshore]", _referenceData);

        // Assert
        Assert.Equal(2.5, coalValueFactor); // Medium = 2.5
        Assert.Equal(1.5, windValueFactor); // Low = 1.5
    }

    [Fact]
    public void GetEmissionFactor_ValidType_ReturnsCorrectValue()
    {
        // Arrange
        var allFactors = new AllFactors
        {
            Collections = new List<GeneratorFactor>
            {
                new GeneratorFactor { GeneratorType = "Coal", ValueFactor = "Medium", EmissionFactor = "High" },
                new GeneratorFactor { GeneratorType = "Gas", ValueFactor = "Medium", EmissionFactor = "Medium" }
            }
        };

        // Act
        var coalEmissionFactor = allFactors.GetEmissionFactor("Coal", _referenceData);
        var gasEmissionFactor = allFactors.GetEmissionFactor("Gas", _referenceData);

        // Assert
        Assert.Equal(0.8, coalEmissionFactor, 2); // High = 0.8
        Assert.Equal(0.5, gasEmissionFactor, 2); // Medium = 0.5
    }

    [Fact]
    public void GetValueFactor_InvalidType_ThrowsException()
    {
        // Arrange
        var allFactors = new AllFactors
        {
            Collections = new List<GeneratorFactor>
            {
                new GeneratorFactor { GeneratorType = "Coal", ValueFactor = "Medium", EmissionFactor = "High" }
            }
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => allFactors.GetValueFactor("InvalidType", _referenceData));
        Assert.Contains("Invalid type", exception.Message);
    }

    [Fact]
    public void GetEmissionFactor_MissingEmissionFactor_ThrowsException()
    {
        // Arrange
        var allFactors = new AllFactors
        {
            Collections = new List<GeneratorFactor>
            {
                new GeneratorFactor { GeneratorType = "Offshore Wind", ValueFactor = "Low" } // No EmissionFactor provided
            }
        };

        // Act & Assert
        var exception = Assert.Throws<ArgumentException>(() => allFactors.GetEmissionFactor("Wind[Offshore]", _referenceData));
        Assert.Contains(($"Invalid type: Wind[Offshore]"), exception.Message);
    }

    [Fact]
    public void ConvertToGeneratorType_ValidInputs_ReturnsExpectedTypes()
    {
        // Act & Assert
        Assert.Equal("Coal", AllFactors.ConvertToGeneratorType("Coal123"));
        Assert.Equal("Gas", AllFactors.ConvertToGeneratorType("Gas456"));
        Assert.Equal("Offshore Wind", AllFactors.ConvertToGeneratorType("Wind[Offshore]"));
        Assert.Equal("Onshore Wind", AllFactors.ConvertToGeneratorType("Wind[Onshore]"));
        Assert.Equal("unknown", AllFactors.ConvertToGeneratorType("Solar"));
    }

    [Fact]
    public void ConvertToGeneratorType_NullOrEmptyInput_ReturnsUnknown()
    {
        // Act & Assert
        Assert.Equal("unknown", AllFactors.ConvertToGeneratorType(null));
        Assert.Equal("unknown", AllFactors.ConvertToGeneratorType(string.Empty));
    }
}
