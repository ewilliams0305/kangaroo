namespace Kangaroo.Compliance

open System
open System.Net
open Xunit
open Kangaroo
open Kangaroo.Compliance

module ``Check network node for compliant Hostname`` = 
    let node1 = new NetworkNode(
        IPAddress.Any,
        new MacAddress("00:AA:BB:CC:00:01"),
        "kangaroo_rules",
        null,
        TimeSpan.FromMilliseconds(200),
        TimeSpan.FromMilliseconds(200),
        true)
    
    let node2 = new NetworkNode(
        IPAddress.Any,
        new MacAddress("AA:01:BB:CC:00:01"),
        "kangaroo_jumps",
        null,
        TimeSpan.FromMilliseconds(200),
        TimeSpan.FromMilliseconds(200),
        true)
    
    let options = Options.CreateOptionsWithLatency(TimeSpan.FromMilliseconds(12), TimeSpan.FromMilliseconds(12))
    
    [<Fact>]
    let ``with equal Hostname Address Node is Compliant`` () =

        let res = NodeChecks.CheckNetworkNode(node1, node1, options.NodeOptions)
        match res.DnsName with 
        | HostnameCompliance.Compliant dnsName -> Assert.True(true)
        | HostnameCompliance.Failure reason -> Assert.True(false)
           
    [<Fact>]
    let ``with different Hostname Node is not Compliant`` () =

        let res = NodeChecks.CheckNetworkNode(node1, node2, options.NodeOptions)
        match res.DnsName with 
        | HostnameCompliance.Compliant dnsName -> Assert.True(false)
        | HostnameCompliance.Failure reason -> Assert.True(true)
       
    [<Fact>]
    let ``with equal Hostname results contain Hostname`` () =

        let res = NodeChecks.CheckNetworkNode(node1, node1, options.NodeOptions)
        match res.DnsName with 
        | HostnameCompliance.Compliant dnsName ->
            match dnsName with
            | (m) when m = "kangaroo_rules" -> Assert.True(true)
            | _ -> Assert.True(false)
        | HostnameCompliance.Failure reason -> Assert.True(false)
        
