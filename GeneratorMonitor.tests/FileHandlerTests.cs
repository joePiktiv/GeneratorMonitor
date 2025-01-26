using Moq;
using GeneratorMonitor.Services;
using Microsoft.Extensions.FileProviders;
using System.Xml;
using Microsoft.VisualStudio.Web.CodeGeneration;
using System.IO.Abstractions;

namespace GeneratorMonitor.Models.Tests
{
    public class FileHandlerTests
    {
        private readonly Mock<IFileSystem> _mockFileSystem;
        private readonly Mock<System.IO.Abstractions.IFileInfo> _mockFileInfo;
        private readonly Mock<IDirectory> _mockDirectory;

        public FileHandlerTests()
        {
            _mockFileSystem = new Mock<IFileSystem>();
            _mockFileInfo = new Mock<System.IO.Abstractions.IFileInfo>();
            _mockDirectory = new Mock<IDirectory>();
        }

        [Fact]
        public void GetFactors_ShouldReturnAllFactors()
        {
            // Arrange
            var csvData = "Generator Type,ValueFactor,EmissionFactor\nOffshore Wind,Low,N/A\nOnshore Wind,High,N/A\nGas,Medium,Medium\nCoal,Medium,High";
            var filePath = "C:\\Generators\\Data\\Factors.csv";

            var expectedFactors = new List<GeneratorFactor>
            {
                new GeneratorFactor { GeneratorType = "Offshore Wind", ValueFactor = "Low", EmissionFactor = "N/A" },
                new GeneratorFactor { GeneratorType = "Onshore Wind", ValueFactor = "High", EmissionFactor = "N/A" },
                new GeneratorFactor { GeneratorType = "Gas", ValueFactor = "Medium", EmissionFactor = "Medium" },
                new GeneratorFactor { GeneratorType = "Coal", ValueFactor = "Medium", EmissionFactor = "High" }
            };

            // Mock the file reading operation
            _mockFileSystem.Setup(fs => fs.File.ReadAllText(It.IsAny<string>())).Returns(csvData);

            // Act
            var result = FileHandler.GetFactors(filePath);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedFactors.Count, result.Collections.Count);
            Assert.Equal("Offshore Wind", result.Collections[0].GeneratorType);
            Assert.Equal("Low", result.Collections[0].ValueFactor);
            Assert.Equal("N/A", result.Collections[0].EmissionFactor);
        }
    }
}
