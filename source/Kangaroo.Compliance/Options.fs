namespace Kangaroo.Compliance

open System

type NodeComplianceOptions = {
    LatencyThreshold: TimeSpan
    QueryThreshold: TimeSpan
    UseStrictMacAddress: bool
    UseStrictHostname: bool
    UseStrictWebServers: bool
}

type ComplianceOptions = {
    TotalTimeThreshold: TimeSpan
    UseStrictNodes: bool
    NodeOptions: NodeComplianceOptions
}

module Options =
   
    let internal CreateDefaultNodeOptions() =
        { LatencyThreshold = TimeSpan.FromSeconds(1)
          QueryThreshold = TimeSpan.FromSeconds(1)
          UseStrictMacAddress = true
          UseStrictHostname = true
          UseStrictWebServers = true }
        
    let internal CreateNodeOptionsWithLatency(latency: TimeSpan) =
        { LatencyThreshold = latency
          QueryThreshold = latency
          UseStrictMacAddress = true
          UseStrictHostname = true
          UseStrictWebServers = true }
        
    let internal CreateDefaultOptions() =
        { TotalTimeThreshold = TimeSpan.FromSeconds(3)
          UseStrictNodes = true
          NodeOptions = CreateDefaultNodeOptions() }
        
    let internal CreateOptionsWithLatency(totalScanTime: TimeSpan, nodeScanTime: TimeSpan) =
        { TotalTimeThreshold = totalScanTime
          UseStrictNodes = true
          NodeOptions = CreateNodeOptionsWithLatency(nodeScanTime) }