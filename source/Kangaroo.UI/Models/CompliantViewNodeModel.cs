using Kangaroo.Compliance;

namespace Kangaroo.UI.Models;

public sealed class CompliantViewNodeModel
{
    public bool IsRunning { get; set; }
    public bool IsCompleted => ComplianceData is not null;
    public bool IsError => ComplianceData is not null && ComplianceData.IsCompliant.IsFailure;
    public bool IsCompliant => ComplianceData is not null && ComplianceData.IsCompliant.IsCompliant;
    public string IpAddress => Node?.IpAddress.ToString() ?? "0.0.0.0";
    public string MacAddress => Node?.MacAddress.ToString() ?? "00:00:00:00:00:00";
    public string Hostname => Node?.HostName ?? string.Empty;
    public string WebServer => Node?.WebServer ?? string.Empty;
    public string Latency => Node?.Latency?.ToString() ?? "00:00:00";
    public string QueryTime => Node?.QueryTime.ToString() ?? "00:00:00";
    public NetworkNode? Node { get; set; }
    public NodeComplianceData? ComplianceData { get; }

    public CompliantViewNodeModel(NetworkNode node)
    {
        Node = node;
    }

    public CompliantViewNodeModel(NodeComplianceData complianceData)
    {
        ComplianceData = complianceData;
        Node = complianceData.Node;
        IsRunning = false;
    }
}
