namespace Kangaroo.Compliance

open System
open System.Net
open Xunit
open Kangaroo
open Kangaroo.Compliance

module ``Check network node for compliant Mac Address`` = 
    let node1 = new NetworkNode(
        IPAddress.Any,
        new MacAddress("00:AA:BB:CC:00:01"),
        null,
        null,
        TimeSpan.FromMilliseconds(200),
        TimeSpan.FromMilliseconds(200),
        true)
    
    let node2 = new NetworkNode(
        IPAddress.Any,
        new MacAddress("AA:01:BB:CC:00:01"),
        null,
        null,
        TimeSpan.FromMilliseconds(200),
        TimeSpan.FromMilliseconds(200),
        true)
    
    let options = { LatencyThreshold = TimeSpan.FromMilliseconds(12); QueryThreshold = TimeSpan.FromMilliseconds(12); }
    
    [<Fact>]
    let ``with equal MAC Address Node is Compliant`` () =

        let res = NodeChecks.CheckNetworkNode(node1, node1, options)
        match res.MacAddress with 
        | NodeMacAddressCheck.Compliant mac -> Assert.True(true)
        | NodeMacAddressCheck.Failure reason -> Assert.True(false)
           
    [<Fact>]
    let ``with different MAC Address Node is not Compliant`` () =

        let res = NodeChecks.CheckNetworkNode(node1, node2, options)
        match res.MacAddress with 
        | NodeMacAddressCheck.Compliant mac -> Assert.True(false)
        | NodeMacAddressCheck.Failure reason -> Assert.True(true)
       
    [<Fact>]
    let ``with equal MAC Address results contain MAC`` () =

        let res = NodeChecks.CheckNetworkNode(node1, node1, options)
        match res.MacAddress with 
        | NodeMacAddressCheck.Compliant mac ->
            match mac with
            | (m) when m = new MacAddress("00:AA:BB:CC:00:01") -> Assert.True(true)
            | _ -> Assert.True(false)
        | NodeMacAddressCheck.Failure reason -> Assert.True(false)
        
