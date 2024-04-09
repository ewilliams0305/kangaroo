namespace Kangaroo;

public static class NetworkNodeExtensions
{
    public static NetworkNode PublishStatus(this NetworkNode node,
        Action<NetworkNode, LiveUpdateStatus>? statusUpdate = null)
    {
        statusUpdate?.Invoke(node, LiveUpdateStatus.Completed);
        return node;
    }
}