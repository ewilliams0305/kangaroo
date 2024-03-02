namespace Kangaroo;

public sealed class ScanResults
{
    public TimeSpan ElapsedTime { get; }
    public IEnumerable<NetworkNode> Nodes { get; }

    public ScanResults(IEnumerable<NetworkNode> nodes, TimeSpan elapsedTime)
    {
        Nodes = nodes;
        ElapsedTime = elapsedTime;
    }
}