namespace Kangaroo.UI.Database;

/// <summary>
/// Used to configure repositories.
/// All repositories requiring configuration or executions can implement this interfaces
/// and the configure method will be automatically executed by the framework.
/// </summary>
public interface IConfigureRepository
{
    /// <summary>
    /// Configures the data type mapping required for the repository.
    /// </summary>
    /// <returns>Results of the Configuration execution</returns>
    bool Configure();
}