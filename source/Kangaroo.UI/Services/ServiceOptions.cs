using System.IO;

namespace Kangaroo.UI.Services;

public class ServiceOptions
{
    public string DatabaseConnection { get; set; }

    public ServiceOptions()
    {
        DatabaseConnection = $"Data Source={Directory.GetCurrentDirectory()}{Path.DirectorySeparatorChar}kangaroo_scanner.db";
    }
}