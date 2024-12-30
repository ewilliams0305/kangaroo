using System;
using System.Text.Json.Serialization;

namespace Kangaroo.UI.Database;

public record RecentScanNodeResult(
    [property: JsonPropertyName("ipAddress")] string IpAddress, 
    [property: JsonPropertyName("macAddress")] string MacAddress, 
    [property: JsonPropertyName("hostName")] string? HostName, 
    [property: JsonPropertyName("webServer")] string? WebServer,
    [property: JsonPropertyName("latency")] TimeSpan? Latency, 
    [property: JsonPropertyName("queryTime")] TimeSpan QueryTime, 
    [property: JsonPropertyName("alive")] bool Alive);