using GeneratorMonitor.Models;
using Microsoft.Extensions.Configuration;

namespace GeneratorMonitor.Services;

public class FolderMonitor : IDisposable
{
    private FileSystemWatcher? _fileWatcher;
    private AllFactors _factors;
    private readonly ReferenceData _reference;
    private readonly object _lock = new object();
    private readonly string _inputFolderName;
    private readonly string _outputFolder;
    private readonly string _archiveFolder;


    public FolderMonitor(IConfiguration config)
    {
        var referencefile = config["FilePaths:BaseDirectory"] + config["FilePaths:SourceDirectory"] + config["FilePaths:ReferenceFile"];
        var sourcefactors = config["FilePaths:BaseDirectory"] + config["FilePaths:SourceDirectory"] + config["FilePaths:Factors"];
        var inputFolder = config["FilePaths:BaseDirectory"] + config["FilePaths:InputFolder"];

        _outputFolder = config["FilePaths:BaseDirectory"] + config["FilePaths:OutputFolder"];
        _archiveFolder = config["FilePaths:BaseDirectory"] + config["FilePaths:ArchiveFolder"];
        _inputFolderName = inputFolder;

        _reference = FileHandler.GetDeserialized<ReferenceData>(referencefile);
        _factors = FileHandler.GetFactors(sourcefactors);

        if (string.IsNullOrWhiteSpace(_inputFolderName))
        {
            throw new ArgumentException("Input folder name cannot be null or empty", nameof(_inputFolderName));
        }

        if (!Directory.Exists(_inputFolderName))
        {
            throw new DirectoryNotFoundException($"The folder '{_inputFolderName}' does not exist.");
        }

        InitializeWatcher();

        Console.WriteLine($"Monitoring input folder...");
        Console.WriteLine("Press Q to quit...");
    }

    private void InitializeWatcher()
    {
        _fileWatcher = new FileSystemWatcher
        {
            Path = _inputFolderName,
            NotifyFilter = NotifyFilters.FileName,
            Filter = "*.xml", 
            EnableRaisingEvents = true
        };

        _fileWatcher.Created += OnFileCreated;
    }

    private void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        lock (_lock) 
        {
            Console.WriteLine($"New file detected: {e.FullPath}");

            try
            {
                Task.Delay(500).Wait();
                ProcessFile(e.FullPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing file: {ex.Message}");
            }
        }
    }

    private void ProcessFile(string filePath)
    {
        Console.WriteLine($"Processing file: {filePath}");
        try
        {
            var fileName = Path.GetFileName(filePath);
            var report = FileHandler.GetDeserialized<GenerationReport>(filePath);
            if (!Directory.Exists(_outputFolder)) Directory.CreateDirectory(_outputFolder);
            var outputFilename = FileHandler.UniqueFileName(_outputFolder +"\\"+ fileName, _outputFolder, "output");
            Report.GenerateEnergyReport(report, _reference, _factors, outputFilename);
            FileHandler.MoveFileToDestinationFolder(filePath, _archiveFolder);
            Console.WriteLine($"Successfully created output report: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to process file: {ex.Message}");
        }
    }

    public void Dispose()
    {
        if (_fileWatcher != null)
        {
            _fileWatcher.EnableRaisingEvents = false;
            _fileWatcher.Created -= OnFileCreated;
            _fileWatcher.Dispose();
            _fileWatcher = null;
        }
    }
}
