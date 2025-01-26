using GeneratorMonitor.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace GeneratorMonitor.Tests;

public class ReferenceDataTests
{
    [Fact]
    public void Levels_ReturnsCorrectValues()
    {
        // Arrange
        var valueFactor = new ValueFactor
        {
            High = 3.5,
            Medium = 2.2,
            Low = 1.1
        };

        // Act
        var levels = valueFactor.Levels;

        // Assert
        Assert.Equal(3, levels.Count);
        Assert.Equal(1.1, levels["Low"]);
        Assert.Equal(2.2, levels["Medium"]);
        Assert.Equal(3.5, levels["High"]);
    }

    [Fact]
    public void SerializeAndDeserializeReferenceData_WorksCorrectly()
    {
        // Arrange
        var referenceData = new ReferenceData
        {
            Factors = new Factors
            {
                ValueFactor = new ValueFactor
                {
                    High = 4.0,
                    Medium = 2.5,
                    Low = 1.0
                },
                EmissionsFactor = new EmissionsFactor
                {
                    High = 0.8,
                    Medium = 0.5,
                    Low = 0.2
                }
            }
        };

        var xmlSerializer = new XmlSerializer(typeof(ReferenceData));
        string serializedXml;

        // Serialize
        using (var stringWriter = new StringWriter())
        {
            xmlSerializer.Serialize(stringWriter, referenceData);
            serializedXml = stringWriter.ToString();
        }

        // Act
        ReferenceData deserializedData;
        using (var stringReader = new StringReader(serializedXml))
        {
            deserializedData = (ReferenceData)xmlSerializer.Deserialize(stringReader);
        }

        // Assert
        Assert.NotNull(deserializedData);
        Assert.Equal(4.0, deserializedData.Factors.ValueFactor.High);
        Assert.Equal(2.5, deserializedData.Factors.ValueFactor.Medium);
        Assert.Equal(1.0, deserializedData.Factors.ValueFactor.Low);
        Assert.Equal(0.8, deserializedData.Factors.EmissionsFactor.High);
        Assert.Equal(0.5, deserializedData.Factors.EmissionsFactor.Medium);
        Assert.Equal(0.2, deserializedData.Factors.EmissionsFactor.Low);
    }

    [Fact]
    public void Levels_ThrowsKeyNotFoundExceptionForInvalidKey()
    {
        // Arrange
        var emissionsFactor = new EmissionsFactor
        {
            High = 0.8,
            Medium = 0.5,
            Low = 0.2
        };

        // Act & Assert
        Assert.Throws<KeyNotFoundException>(() => _ = emissionsFactor.Levels["InvalidKey"]);
    }
}
