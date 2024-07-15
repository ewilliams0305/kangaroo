namespace Kangaroo;

/// <summary>
/// Provides hooks for status updates.
/// </summary>
public interface IScannerEvents
{
    /// <summary>
    /// Reports the status of the scan when the scan has started and when the scan has ended.
    /// </summary>
    Action<ScanResults, LiveUpdateStatus>? ScanStatusUpdate { get; set; }
    
    /// <summary>
    /// Reports the status of individual network nodes as they are scanned. 
    /// </summary>
    Action<NetworkNode, LiveUpdateStatus>? NodeStatusUpdate { get; set; }
}