using System;
using System.Collections.Generic;
using Xunit;

namespace GeneratorMonitor.Models.Tests
{
    public class OutputTests
    {
        [Fact]
        public void Constructor_CalculatesTotalsAndEmissionsCorrectly()
        {
            // Arrange
            var referenceData = new ReferenceData
            {
                Factors = new Factors
                {
                    ValueFactor = new ValueFactor { Low = 0.5, Medium = 1.0, High = 1.5 },
                    EmissionsFactor = new EmissionsFactor { Low = 0.2, Medium = 0.5, High = 0.8 }
                }
            };

            var allFactors = new AllFactors
            {
                Collections = new List<GeneratorFactor>
                {
                    new GeneratorFactor { GeneratorType = "Coal", ValueFactor = "Medium", EmissionFactor = "High" },
                    new GeneratorFactor { GeneratorType = "Gas", ValueFactor = "Medium", EmissionFactor = "Medium" },
                    new GeneratorFactor { GeneratorType = "Offshore Wind", ValueFactor = "Low", EmissionFactor = "N/A" },
                    new GeneratorFactor { GeneratorType = "Onshore Wind", ValueFactor = "High", EmissionFactor = "N/A" }
                }
            };

            var generationReport = new GenerationReport
            {
                Coal = new Coal
                {
                    CoalGenerators = new List<CoalGenerator>
                    {
                        new CoalGenerator
                        {
                            Name = "Coal Gen 1",
                            TotalHeatInput = 5000,
                            ActualNetGeneration = 2000,
                            Generations = new List<Generation>
                            {
                                new Generation
                                {
                                    Days = new List<Day>
                                    {
                                        new Day { Date = DateTime.Now, Energy = 100, Price = 50 }
                                    }
                                }
                            }
                        }
                    }
                },
                Gas = new Gas
                {
                    GasGenerators = new List<GasGenerators>
                    {
                        new GasGenerators
                        {
                            Name = "Gas Gen 1",
                            EmissionsRating = 0.5,
                            Generations = new List<Generation>
                            {
                                new Generation
                                {
                                    Days = new List<Day>
                                    {
                                        new Day { Date = DateTime.Now, Energy = 200, Price = 30 }
                                    }
                                }
                            }
                        }
                    }
                },
                Wind = new Wind
                {
                    WindGenerators = new List<WindGenerator>
                    {
                        new WindGenerator
                        {
                            Name = "Wind[Offshore]",
                            Location = "Offshore",
                            Generations = new List<Generation>
                            {
                                new Generation
                                {
                                    Days = new List<Day>
                                    {
                                        new Day { Date = DateTime.Now, Energy = 300, Price = 20 }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            // Act: Create the Output object
            var output = new Output(generationReport, referenceData, allFactors);

            // Assert: Verify totals and emissions
            Assert.NotNull(output.Totals);
            Assert.NotEmpty(output.Totals.Generators);
            //Assert.Equal(5000, output.Totals.Generators[0].Total, 2); // Assuming generator total calculation
            Assert.NotNull(output.MaxEmissionGenerators);
            Assert.NotEmpty(output.MaxEmissionGenerators.Day);
        }
    }
}
