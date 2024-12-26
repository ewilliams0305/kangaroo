namespace Kangaroo.Compliance

open System
open Kangaroo

type NodeMacAddressCheck =
    | Compliant of MacAddress
    | Failure of string
    
type NodeHostnameCheck =
    | Compliant of string
    | Failure of string
    
type NodeWebServerCheck =
    | Compliant of string
    | Failure of string
    
type LatencyFailure =
    | Slow
    | None
    | Invalid
    
type NodeLatencyCheck =
    | Compliant of TimeSpan
    | Failure  of LatencyFailure
    
type NodeQueryTime =
    | Compliant of TimeSpan
    | Failure  of LatencyFailure
    
type NodeAlive =
    | Compliant
    | Failure
    
type NodeComplianceRun = {
    Node: NetworkNode
    MacAddress: NodeMacAddressCheck
    DnsName: NodeHostnameCheck
    WebServer: NodeWebServerCheck
    Latency: NodeLatencyCheck    
    QueryTime: NodeQueryTime
    IsAlive: NodeAlive
}

type NodeComplianceConfiguration = {
    LatencyThreshold: TimeSpan
    QueryThreshold: TimeSpan
}

module NodeChecks =
    
    let internal CheckMacAddress (compliance: NetworkNode, scanned: NetworkNode) =
        let itemsMatch = compliance.MacAddress = scanned.MacAddress
        match itemsMatch with 
        | true -> NodeMacAddressCheck.Compliant scanned.MacAddress
        | false -> NodeMacAddressCheck.Failure "Mac Addresses Do Not Match" 
       
    let internal CheckHostname (compliance: NetworkNode, scanned: NetworkNode) =
        let itemsMatch = compliance.HostName = scanned.HostName
        match itemsMatch with 
        | true -> NodeHostnameCheck.Compliant scanned.HostName
        | false -> NodeHostnameCheck.Failure "DNS Names Do Not Match"        
       
    let internal CheckWebServer (compliance: NetworkNode, scanned: NetworkNode) =
        let itemsMatch = compliance.WebServer = scanned.WebServer
        match itemsMatch with 
        | true -> NodeWebServerCheck.Compliant scanned.WebServer
        | false -> NodeWebServerCheck.Failure "Webserver Type Does Not Match"
               
    let internal CheckLatency (compliance: NetworkNode, scanned: NetworkNode, options: NodeComplianceConfiguration) =
        match (compliance.Latency, scanned.Latency) with
        | (c, s) when c.HasValue && s.HasValue -> 
            match (c.Value, s.Value) with
            | (c, s) when s = TimeSpan.Zero -> NodeLatencyCheck.Failure LatencyFailure.None
            | (c, s) when s - c <= options.LatencyThreshold -> NodeLatencyCheck.Compliant (s - c)
            | (c, s) when s - c > options.LatencyThreshold -> NodeLatencyCheck.Failure LatencyFailure.Slow
            | _ -> NodeLatencyCheck.Failure LatencyFailure.Invalid
        | (c, s) when not c.HasValue && s.HasValue -> NodeLatencyCheck.Compliant scanned.Latency.Value
        | _ -> NodeLatencyCheck.Failure LatencyFailure.None
                       
    let internal CheckQueryTime(compliance: NetworkNode, scanned: NetworkNode, options: NodeComplianceConfiguration) =
        match (compliance.QueryTime, scanned.QueryTime) with
        | (c, s) when s = TimeSpan.Zero -> NodeQueryTime.Failure  LatencyFailure.None
        | (c, s) when s - c <= options.LatencyThreshold -> NodeQueryTime.Compliant scanned.Latency.Value
        | (c, s) when s - c > options.LatencyThreshold -> NodeQueryTime.Failure  LatencyFailure.Slow
        | _ -> NodeQueryTime.Failure LatencyFailure.Invalid
        
    let internal CheckAlive(compliance: NetworkNode, scanned: NetworkNode) =
        match (compliance, scanned) with
        | (x, y) when x.Alive && y.Alive -> NodeAlive.Compliant
        | (x, y) when x.Alive && not y.Alive -> NodeAlive.Failure
        | _ -> NodeAlive.Failure
        
    let public CheckNetworkNode (compliance: NetworkNode, scanned: NetworkNode, options: NodeComplianceConfiguration) =
        {
            Node = scanned;
            MacAddress = CheckMacAddress(compliance, scanned) 
            DnsName = CheckHostname(compliance, scanned)
            WebServer = CheckWebServer(compliance, scanned)
            Latency = CheckLatency(compliance, scanned, options)    
            QueryTime = CheckQueryTime(compliance, scanned, options)
            IsAlive = CheckAlive(compliance, scanned) 
        }