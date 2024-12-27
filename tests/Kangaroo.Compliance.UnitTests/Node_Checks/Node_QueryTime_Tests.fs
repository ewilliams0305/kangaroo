namespace Kangaroo.Compliance.Node_Checks

open System
open System.Net
open Xunit
open Kangaroo
open Kangaroo.Compliance

module ``Check network node for compliant network Query Time`` = 
    let baseNode = new NetworkNode(
        IPAddress.Any,
        new MacAddress("00:AA:BB:CC:00:01"),
        null,
        "node.kangaroo",
        TimeSpan.FromMilliseconds(200),
        TimeSpan.FromMilliseconds(200),
        true)
    
    let slowNode = new NetworkNode(
        IPAddress.Any,
        new MacAddress("AA:01:BB:CC:00:01"),
        null,
        "apache.kangaroo",
        TimeSpan.FromMilliseconds(213),
        TimeSpan.FromMilliseconds(213),
        true)
    
    let zeroNode = new NetworkNode(
        IPAddress.Any,
        new MacAddress("AA:01:BB:CC:00:01"),
        null,
        "apache.kangaroo",
        TimeSpan.FromMilliseconds(0),
        TimeSpan.FromMilliseconds(0),
        true)
    
    let options = Options.CreateOptionsWithLatency(TimeSpan.FromMilliseconds(12), TimeSpan.FromMilliseconds(12))
    
    [<Fact>]
    let ``with equal QueryTime Node is Compliant`` () =

        let res = NodeChecks.CheckNetworkNode(baseNode, baseNode, options.NodeOptions)
        match res.QueryTime with 
        | QueryTimeCompliance.Compliant latency -> Assert.True(true)
        | QueryTimeCompliance.Failure reason -> Assert.True(false)
          

    [<Fact>]
    let ``with latency slower than threshold is not Compliant`` () =

        let res = NodeChecks.CheckNetworkNode(baseNode, slowNode, options.NodeOptions)
        match res.QueryTime with 
        | QueryTimeCompliance.Compliant latency -> Assert.True(false)
        | QueryTimeCompliance.Failure reason -> Assert.True(true)
          
          
    [<Fact>]
    let ``with latency zero is not Compliant`` () =

        let res = NodeChecks.CheckNetworkNode(baseNode, zeroNode, options.NodeOptions)
        match res.QueryTime with 
        | QueryTimeCompliance.Compliant latency -> Assert.True(false)
        | QueryTimeCompliance.Failure reason -> Assert.True(true)
          
