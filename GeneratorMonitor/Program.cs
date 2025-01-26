using GeneratorMonitor.Services;
using Microsoft.Extensions.Configuration;

namespace GeneratorMonitor;
public class Program
{
    static void Main(string[] args)
    {
        try
        {
            var config = new ConfigurationBuilder() 
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true) 
                .Build();

            using (FolderMonitor monitor = new FolderMonitor(config))
            {
                while (Console.ReadKey().Key != ConsoleKey.Q) { }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

}
