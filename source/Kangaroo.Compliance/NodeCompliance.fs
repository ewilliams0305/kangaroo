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
    
type NodeLatencyCheck =
    | Compliant of TimeSpan
    | Failure  of string
    
type NodeQueryTime =
    | Compliant of TimeSpan
    | Failure  of string
    
type NodeComplianceRun = {
    Node: NetworkNode
    MacAddress: NodeMacAddressCheck
    DnsName: NodeHostnameCheck
    WebServer: NodeWebServerCheck
    Latency: NodeLatencyCheck    
    QueryTime: NodeQueryTime    
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
        match (scanned.Latency, compliance.Latency) with
        | (x, y) when x.HasValue && y.HasValue -> 
            match x.Value - y.Value with 
            | (x) when x <= options.LatencyThreshold -> NodeLatencyCheck.Compliant scanned.Latency.Value
            | (x) when x = TimeSpan.MinValue -> NodeLatencyCheck.Failure "The network returned a Zero latency"
            | (x) when x > options.LatencyThreshold -> NodeLatencyCheck.Failure "The network node is too slow and out of spec"
            | _ -> NodeLatencyCheck.Failure "Invalid Latency"
        | (x, y) when x.HasValue -> NodeLatencyCheck.Compliant scanned.Latency.Value
        | _ -> NodeLatencyCheck.Failure "Missing Compliance Value or Scanned Latency"
                       
    let internal CheckQueryTime(compliance: NetworkNode, scanned: NetworkNode, options: NodeComplianceConfiguration) =
        match (compliance.QueryTime, scanned.QueryTime) with
        | (x, y) when x - y <= options.LatencyThreshold -> NodeQueryTime.Compliant scanned.Latency.Value
        | (x, y)  when x = TimeSpan.MinValue -> NodeQueryTime.Failure "The network returned a Zero latency"
        | (x, y) when x > options.LatencyThreshold -> NodeQueryTime.Failure "The network node is too slow and out of spec"
        | _ -> NodeQueryTime.Failure "Invalid Latency"
        
    let public CheckNetworkNode (compliance: NetworkNode, scanned: NetworkNode, options: NodeComplianceConfiguration) =
        {
            Node = scanned;
            MacAddress = CheckMacAddress(compliance, scanned) 
            DnsName = CheckHostname(compliance, scanned)
            WebServer = CheckWebServer(compliance, scanned)
            Latency = CheckLatency(compliance, scanned, options)    
            QueryTime = CheckQueryTime(compliance, scanned, options)
        }