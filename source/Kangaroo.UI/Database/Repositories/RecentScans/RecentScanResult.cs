using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kangaroo.UI.Database;

public record RecentScanResult(
    [property: JsonPropertyName("nodes")] IEnumerable<RecentScanNodeResult> Nodes,
    [property: JsonPropertyName("elapsedTime")] TimeSpan ElapsedTime,
    [property: JsonPropertyName("numberOfAddressesScanned")] int NumberOfAddressesScanned,
    [property: JsonPropertyName("numberOfAliveNodes")] int NumberOfAliveNodes,
    [property: JsonPropertyName("startAddress")] string StartAddress,
    [property: JsonPropertyName("endAddress")] string EndAddress);