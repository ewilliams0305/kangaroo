namespace Kangaroo;

/// <summary>
/// extension methods to process network nodes
/// </summary>
public static class NetworkNodeExtensions
{
    /// <summary>
    /// Provides a live update with the network node provided.
    /// </summary>
    /// <param name="node">the node that was updated</param>
    /// <param name="statusUpdate">the updated delegate</param>
    /// <returns>the node that published the update</returns>
    public static NetworkNode PublishStatus(this NetworkNode node,
        Action<NetworkNode, LiveUpdateStatus>? statusUpdate = null)
    {
        statusUpdate?.Invoke(node, LiveUpdateStatus.Completed);
        return node;
    }
}