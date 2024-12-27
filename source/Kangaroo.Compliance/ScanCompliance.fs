namespace Kangaroo.Compliance

open System
open System.Net
open Kangaroo

type ComplianceCheck =
    | NumberOfAliveDevicesMatch
    | ElapsedTimeWithinThreshold
    | IpAddressesMatch

type ComplianceError =
    | NumberOfAliveDevicesDontMatch
    | ElapsedTimeExceededThreshold
    | IpAddressesDontMatch
    | IpAddressNotFound

type ComplianceSuccess = {
    CheckDataTime: DateTime
    TimeBetweenScans: TimeSpan
    Checks: list<ComplianceCheck> 
    Errors: list<ComplianceError>
    Nodes: list<Result<NodeComplianceData, ComplianceError>>
}

type ComplianceFailure = {
    CheckDataTime: DateTime
    TimeBetweenScans: TimeSpan
    Checks: list<ComplianceCheck> 
    Errors: list<ComplianceError>
    Nodes: list<NodeComplianceData>
}

type AliveDevices = int 

type Compliance =
    | Compliant of ComplianceSuccess 
    | Failure of ComplianceFailure


module ScanChecks =

    let internal checkAliveDevices (compliance: AliveDevices, scanned: AliveDevices) =
        let itemsMatch = compliance = scanned
        match itemsMatch with 
        | true -> Ok NumberOfAliveDevicesMatch 
        | false -> Error NumberOfAliveDevicesDontMatch 

    let internal checkElapsedTime (compliance: TimeSpan, scanned: TimeSpan, threshold: TimeSpan) =
        let difference = scanned - compliance
        match difference with 
        | d when d < threshold -> Ok ElapsedTimeWithinThreshold
        | _ -> Error ElapsedTimeExceededThreshold

    let private checkIpAddressesAgainstScanned (compliance: list<NetworkNode>, scanned: list<NetworkNode>) = 
        let ips = scanned |> List.map (fun item -> item.IpAddress)
        compliance
        |> List.filter (fun node -> node.Alive)
        |> List.filter (fun item -> not (List.contains item.IpAddress ips))
        |> fun nodes -> 
            match nodes with 
            | [] -> Ok IpAddressesMatch 
            | _ -> Error IpAddressesDontMatch         
            
    let private checkIpAddressesAgainstCompliance (compliance: list<NetworkNode>, scanned: list<NetworkNode>) = 
        let ips = compliance |> List.map (fun item -> item.IpAddress)
        scanned
        |> List.filter (fun node -> node.Alive)
        |> List.filter (fun item -> not (List.contains item.IpAddress ips))
        |> fun nodes -> 
            match nodes with 
            | [] -> Ok IpAddressesMatch 
            | _ -> Error IpAddressesDontMatch
                
    let internal checkIpAddresses(compliance: list<NetworkNode>, scanned: list<NetworkNode>) = 
        let result = checkIpAddressesAgainstScanned (compliance, scanned)
        match result with 
        | Ok matched -> 
            match matched with 
            | IpAddressesMatch -> checkIpAddressesAgainstCompliance(compliance, scanned)
            | _ -> Error IpAddressesDontMatch
        | Error mismatch -> Error mismatch
    
    let internal findMatchingNode(ipAddress: IPAddress, nodes: list<NetworkNode>) =
        match nodes |> List.tryFind(fun n -> n.IpAddress = IPAddress.Any ) with
        | Option.Some node -> Ok node
        | Option.None -> Error IpAddressNotFound
       
    let internal checkNetworkNodes(compliance: list<NetworkNode>, scanned: list<NetworkNode>, options: NodeComplianceOptions) = 
        compliance
        |> List.map (fun node ->
            match findMatchingNode(node.IpAddress, scanned) with 
            | Ok scannedNode -> Ok (NodeChecks.CheckNetworkNode(node, scannedNode, options))
            | Error error -> Error error) 
               
    let CheckForCompliance (compliance: ScanResults, scanned: ScanResults, options: ComplianceOptions) =
        let checks = [
            checkAliveDevices(compliance.NumberOfAliveNodes, scanned.NumberOfAliveNodes); 
            checkElapsedTime(compliance.ElapsedTime, scanned.ElapsedTime, TimeSpan.FromMilliseconds(5000));
            checkIpAddresses(Seq.toList compliance.Nodes, Seq.toList scanned.Nodes )]
        
        let success, failure = checks |> List.partition(function | Ok _ -> true | Error _ -> false)

        Compliant { 
            CheckDataTime = DateTime.Now
            TimeBetweenScans = compliance.ElapsedTime - scanned.ElapsedTime
            Checks = success |> List.map (function | Ok check -> check | _ -> failwith "Unexpected pattern")
            Errors = failure |> List.map (function | Error err -> err | _ -> failwith "Unexpected pattern")
            Nodes = checkNetworkNodes(Seq.toList compliance.Nodes, Seq.toList scanned.Nodes, options.NodeOptions)
            }
