using System;
using System.Collections.Generic;
using GeneratorMonitor.Models;
using Xunit;

namespace GeneratorMonitor.Tests
{
    public class EnergyGeneratorTests
    {
        // Example test for TotalGeneration property
        [Fact]
        public void TotalGeneration_CalculatesCorrectly()
        {
            // Arrange
            var energyGenerator = new MockEnergyGenerator
            {
                Name = "Test Generator",
                Generations = new List<Generation>
                {
                    new Generation
                    {
                        Days = new List<Day>
                        {
                            new Day { Date = new DateTime(2025, 1, 1), Energy = 10, Price = 2 },
                            new Day { Date = new DateTime(2025, 1, 2), Energy = 15, Price = 1.5 }
                        }
                    }
                }
            };

            // Expected total generation
            var expected = 10 * 2 + 15 * 1.5;

            // Act
            var totalGeneration = energyGenerator.TotalGeneration;

            // Assert
            Assert.Equal(expected, totalGeneration, 2); // Allow for minor precision differences
        }
    }

    // Mock class for testing the abstract EnergyGenerator class
    public class MockEnergyGenerator : EnergyGenerator { }
}
