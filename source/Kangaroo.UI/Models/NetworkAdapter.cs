namespace Kangaroo.UI.Models;

/// <summary>
/// Model to store all required network adapter data.
/// </summary>
public class NetworkAdapter
{
    /// <summary>
    /// Name of the adapter
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// IP Address currently assigned to the adapter.
    /// </summary>
    public string IpAddress { get; set; } = string.Empty;

    /// <summary>
    /// Physical address of the adapter.
    /// </summary>
    public string MacAddress { get; set; } = string.Empty;
}