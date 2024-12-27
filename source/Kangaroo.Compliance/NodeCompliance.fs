namespace Kangaroo.Compliance

open System
open Kangaroo

type IsCompliant = 
    | Compliant
    | Failure

type MacAddressCompliance =
    | Compliant of MacAddress
    | Failure of string
    
type HostnameCompliance =
    | Compliant of string
    | Failure of string
    
type WebServerCompliance =
    | Compliant of string
    | Failure of string
    
type LatencyFailure =
    | Slow
    | None
    | Invalid
    
type LatencyCompliance =
    | Compliant of TimeSpan
    | Failure  of LatencyFailure
    
type QueryTimeCompliance =
    | Compliant of TimeSpan
    | Failure  of LatencyFailure
    
type AliveCompliance =
    | Compliant
    | Failure
    
type NodeComplianceData = {
    Node: NetworkNode
    MacAddress: MacAddressCompliance
    DnsName: HostnameCompliance
    WebServer: WebServerCompliance
    Latency: LatencyCompliance    
    QueryTime: QueryTimeCompliance
    IsAlive: AliveCompliance
} with
    member this.IsCompliant =
        match this with
        | {
            MacAddress = MacAddressCompliance.Compliant _;
            DnsName = HostnameCompliance.Compliant _;
            WebServer = WebServerCompliance.Compliant _;
            Latency = LatencyCompliance.Compliant _;
            QueryTime = QueryTimeCompliance.Compliant _;
            IsAlive = AliveCompliance.Compliant;
            } -> IsCompliant.Compliant
        | _ -> IsCompliant.Failure

module NodeChecks =
   
    let internal CheckMacAddress (compliance: NetworkNode, scanned: NetworkNode, options: NodeComplianceOptions) =
        match (compliance.MacAddress, scanned.MacAddress, options.UseStrictMacAddress) with 
        | (c, s, x) when x = false -> MacAddressCompliance.Compliant s
        | (c, s, x) when x = true && c = s -> MacAddressCompliance.Compliant s
        | _ -> MacAddressCompliance.Failure "Mac Addresses Do Not Match" 
       
    let internal CheckHostname (compliance: NetworkNode, scanned: NetworkNode, options: NodeComplianceOptions) =
        match (compliance.HostName, scanned.HostName, options.UseStrictHostname) with 
        | (c, s, x) when x = false -> HostnameCompliance.Compliant s
        | (c, s, x) when x = true && c = s -> HostnameCompliance.Compliant s
        | _ -> HostnameCompliance.Failure "DNS Names Do Not Match"
       
    let internal CheckWebServer (compliance: NetworkNode, scanned: NetworkNode, options: NodeComplianceOptions) =
        match (compliance.WebServer, scanned.WebServer, options.UseStrictMacAddress) with 
        | (c, s, x) when x = false -> WebServerCompliance.Compliant s
        | (c, s, x) when x = true && c = s -> WebServerCompliance.Compliant s
        | _ -> WebServerCompliance.Failure "Webserver Type Does Not Match"
               
    let internal CheckLatency (compliance: NetworkNode, scanned: NetworkNode, options: NodeComplianceOptions) =
        match (compliance.Latency, scanned.Latency) with
        | (c, s) when c.HasValue && s.HasValue -> 
            match (c.Value, s.Value) with
            | (c, s) when s = TimeSpan.Zero -> LatencyCompliance.Failure LatencyFailure.None
            | (c, s) when s - c <= options.LatencyThreshold -> LatencyCompliance.Compliant (s - c)
            | (c, s) when s - c > options.LatencyThreshold -> LatencyCompliance.Failure LatencyFailure.Slow
            | _ -> LatencyCompliance.Failure LatencyFailure.Invalid
        | (c, s) when not c.HasValue && s.HasValue -> LatencyCompliance.Compliant scanned.Latency.Value
        | _ -> LatencyCompliance.Failure LatencyFailure.None
                       
    let internal CheckQueryTime(compliance: NetworkNode, scanned: NetworkNode, options: NodeComplianceOptions) =
        match (compliance.QueryTime, scanned.QueryTime) with
        | (c, s) when s = TimeSpan.Zero -> QueryTimeCompliance.Failure LatencyFailure.None
        | (c, s) when s - c <= options.LatencyThreshold -> QueryTimeCompliance.Compliant s
        | (c, s) when s - c > options.LatencyThreshold -> QueryTimeCompliance.Failure  LatencyFailure.Slow
        | _ -> QueryTimeCompliance.Failure LatencyFailure.Invalid
        
    let internal CheckAlive(compliance: NetworkNode, scanned: NetworkNode) =
        match (compliance, scanned) with
        | (x, y) when x.Alive && y.Alive -> AliveCompliance.Compliant
        | (x, y) when x.Alive && not y.Alive -> AliveCompliance.Failure
        | _ -> AliveCompliance.Failure
        
    let public CheckNetworkNode (compliance: NetworkNode, scanned: NetworkNode, options: NodeComplianceOptions) =
        {
            Node = scanned;
            MacAddress = CheckMacAddress(compliance, scanned, options) 
            DnsName = CheckHostname(compliance, scanned, options)
            WebServer = CheckWebServer(compliance, scanned, options)
            Latency = CheckLatency(compliance, scanned, options)    
            QueryTime = CheckQueryTime(compliance, scanned, options)
            IsAlive = CheckAlive(compliance, scanned) 
        }