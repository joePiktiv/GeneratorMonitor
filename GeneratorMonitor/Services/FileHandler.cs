using CsvHelper;
using GeneratorMonitor.Models;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;

namespace GeneratorMonitor.Services;

public class FileHandler
{
    public static AllFactors GetFactors(string sourcefile)
    {
        List<GeneratorFactor> factors;
        using (var reader = new StreamReader(sourcefile))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Context.RegisterClassMap<GeneratorMap>();
            factors = csv.GetRecords<GeneratorFactor>().ToList();
        }
        return new AllFactors() { Collections = factors};
    }

    public static T GetDeserialized<T>(string fileName)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(T));
        T result;
        using (FileStream fileStream = new FileStream(fileName, FileMode.Open))
        {
            result = (T) serializer.Deserialize(fileStream);
        }
        return result;
    }

    public static void OutputFile<T>(T source, string fileName) 
    {

        XmlSerializer serializer = new XmlSerializer(typeof(T));
        XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
        namespaces.Add(string.Empty, string.Empty);
        using (var writer = XmlWriter.Create(fileName, new XmlWriterSettings { Indent = true }))
        {
            serializer.Serialize(writer, source, namespaces);
        }

    }

    public static void MoveFileToDestinationFolder(string filePath, string destinationFolder)
    {
        try
        {
            string fileName = Path.GetFileName(filePath);
            if (!Directory.Exists(destinationFolder)) Directory.CreateDirectory(destinationFolder);
            string destinationPath = UniqueFileName( Path.Combine(destinationFolder, fileName), destinationFolder, "archive");

            File.Move(filePath, destinationPath);
            Console.WriteLine($"File moved to processed folder: {destinationPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error moving file: {ex.Message}");
        }
    }

    public static string UniqueFileName(string fileName, string folderName, string tag)
    {
        
        if (File.Exists(fileName))
        {
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            return Path.Combine(folderName, $"{tag}{Path.GetFileNameWithoutExtension(fileName)}_{timestamp}{Path.GetExtension(fileName)}");
        } else return fileName;
    }
}
