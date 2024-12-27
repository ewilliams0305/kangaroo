namespace Kangaroo.Compliance

open System
open System.Net
open Xunit
open Kangaroo
open Kangaroo.Compliance

module ``Check network node for compliant WebServer Name`` = 
    let node1 = new NetworkNode(
        IPAddress.Any,
        new MacAddress("00:AA:BB:CC:00:01"),
        null,
        "node.kangaroo",
        TimeSpan.FromMilliseconds(200),
        TimeSpan.FromMilliseconds(200),
        true)
    
    let node2 = new NetworkNode(
        IPAddress.Any,
        new MacAddress("AA:01:BB:CC:00:01"),
        null,
        "apache.kangaroo",
        TimeSpan.FromMilliseconds(200),
        TimeSpan.FromMilliseconds(200),
        true)
    
    let options = Options.CreateOptionsWithLatency(TimeSpan.FromMilliseconds(12), TimeSpan.FromMilliseconds(12))
    
    [<Fact>]
    let ``with equal WebServer Node is Compliant`` () =

        let res = NodeChecks.CheckNetworkNode(node1, node1, options.NodeOptions)
        match res.WebServer with 
        | WebServerCompliance.Compliant server -> Assert.True(true)
        | WebServerCompliance.Failure reason -> Assert.True(false)
           
    [<Fact>]
    let ``with different WebServer Node is not Compliant`` () =

        let res = NodeChecks.CheckNetworkNode(node1, node2, options.NodeOptions)
        match res.WebServer with 
        | WebServerCompliance.Compliant server -> Assert.True(false)
        | WebServerCompliance.Failure reason -> Assert.True(true)
       
    [<Fact>]
    let ``with equal WebServer results contain MAC`` () =

        let res = NodeChecks.CheckNetworkNode(node1, node1, options.NodeOptions)
        match res.WebServer with 
        | WebServerCompliance.Compliant server ->
            match server with
            | (s) when s = "node.kangaroo" -> Assert.True(true)
            | _ -> Assert.True(false)
        | WebServerCompliance.Failure reason -> Assert.True(false)
        
