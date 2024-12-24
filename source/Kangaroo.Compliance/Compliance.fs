namespace Kangaroo.Compliance

open System
open Kangaroo

type ComplianceCheck =
    | NumberOfAliveDevicesMatch
    | ElapsedTimeWithinThreshold
    | IpAddressesMatch

type ComplianceError =
    | NumberOfAliveDevicesDontMatch
    | ElapsedTimeExceededThreshold
    | IpAddressesDontMatch


type ComplianceSuccess = {
    CheckDataTime: DateTime
    TimeBetweenScans: TimeSpan
    Checks: list<ComplianceCheck> 
    Errors: list<ComplianceError> 
}

type ComplianceFailure = {
    CheckDataTime: DateTime
    TimeBetweenScans: TimeSpan
    Checks: list<ComplianceCheck> 
    Errors: list<ComplianceError> 
}

type AliveDevices = int 

type Compliance =
    | Compliant of ComplianceSuccess 
    | Failure of ComplianceFailure


module Checks =

    let internal checkAliveDevices (compliance: AliveDevices, scanned: AliveDevices) =
        let itemsMatch = compliance = scanned
        match itemsMatch with 
        | true -> Ok NumberOfAliveDevicesMatch 
        | false -> Error NumberOfAliveDevicesDontMatch 

    let internal checkElapsedTime (compliance: TimeSpan, scanned: TimeSpan, threashold: TimeSpan) =
        let difference = scanned - compliance
        match difference with 
        | d when d < threashold -> Ok ElapsedTimeWithinThreshold
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
            
    let CheckForCompliance (compliance: ScanResults, scanned: ScanResults) =
        let checks = [
            checkAliveDevices(compliance.NumberOfAliveNodes, scanned.NumberOfAliveNodes); 
            checkElapsedTime(compliance.ElapsedTime, scanned.ElapsedTime, TimeSpan.FromMilliseconds(5000));
            checkIpAddresses(Seq.toList compliance.Nodes, Seq.toList scanned.Nodes )]
        
        let success, failure = checks |> List.partition(function | Ok _ -> true | Error _ -> false)

        let result = Compliant { 
            CheckDataTime = DateTime.Now
            TimeBetweenScans = compliance.ElapsedTime - scanned.ElapsedTime
            Checks = success |> List.map (function | Ok check -> check | _ -> failwith "Unexpected pattern")
            Errors = failure |> List.map (function | Error err -> err | _ -> failwith "Unexpected pattern")}
        
        result
