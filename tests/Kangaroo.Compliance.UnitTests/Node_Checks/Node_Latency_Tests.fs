namespace Kangaroo.Compliance

open System
open System.Net
open Xunit
open Kangaroo
open Kangaroo.Compliance

module ``Check network node for compliant network Latency`` = 
    let baseNode = new NetworkNode(
        IPAddress.Any,
        new MacAddress("00:AA:BB:CC:00:01"),
        null,
        "node.kangaroo",
        TimeSpan.FromMilliseconds(200),
        TimeSpan.FromMilliseconds(200),
        true)
    
    let withinNode = new NetworkNode(
        IPAddress.Any,
        new MacAddress("AA:01:BB:CC:00:01"),
        null,
        "apache.kangaroo",
        TimeSpan.FromMilliseconds(211),
        TimeSpan.FromMilliseconds(211),
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
        TimeSpan.Zero,
        TimeSpan.Zero,
        true)
    
    let nullNode = new NetworkNode(
        IPAddress.Any,
        new MacAddress("AA:01:BB:CC:00:01"),
        null,
        "apache.kangaroo",
        System.Nullable(),
        TimeSpan.FromMilliseconds(0),
        true)
    
    let options = Options.CreateOptionsWithLatency(TimeSpan.FromMilliseconds(12), TimeSpan.FromMilliseconds(12))
    
    [<Fact>]
    let ``with equal Latency Node is Compliant`` () =

        let res = NodeChecks.CheckNetworkNode(baseNode, baseNode, options.NodeOptions)
        match res.Latency with 
        | LatencyCompliance.Compliant latency -> Assert.True(true)
        | LatencyCompliance.Failure reason -> Assert.True(false)
          

    [<Fact>]
    let ``with latency within the threshold is Compliant`` () =

        let res = NodeChecks.CheckNetworkNode(baseNode, withinNode, options.NodeOptions)
        match res.Latency with 
        | LatencyCompliance.Compliant latency -> Assert.True(true)
        | LatencyCompliance.Failure reason -> Assert.True(false)
          
          
    [<Fact>]
    let ``with latency slower than threshold is not Compliant`` () =

        let res = NodeChecks.CheckNetworkNode(baseNode, slowNode, options.NodeOptions)
        match res.Latency with 
        | LatencyCompliance.Compliant latency -> Assert.True(false)
        | LatencyCompliance.Failure reason -> Assert.True(true)
          
          
    [<Fact>]
    let ``with zero latency is not Compliant`` () =
        let res = NodeChecks.CheckNetworkNode(baseNode, zeroNode, options.NodeOptions)
        match res.Latency with 
        | LatencyCompliance.Compliant latency -> Assert.True(false)
        | LatencyCompliance.Failure reason -> Assert.True(true)
          
          
    [<Fact>]
    let ``with null latency is not Compliant`` () =

        let res = NodeChecks.CheckNetworkNode(baseNode, nullNode, options.NodeOptions)
        match res.Latency with 
        | LatencyCompliance.Compliant latency -> Assert.True(false)
        | LatencyCompliance.Failure reason -> Assert.True(true)
          
    [<Fact>]
    let ``with equal Latency Node contains the difference`` () =

        let res = NodeChecks.CheckNetworkNode(baseNode, baseNode, options.NodeOptions)
        match res.Latency with 
        | LatencyCompliance.Compliant latency when latency = TimeSpan.Zero -> Assert.True(true)
        | _ -> Assert.True(false)
          
    [<Fact>]
    let ``with latency within the threshold contains the difference`` () =

        let res = NodeChecks.CheckNetworkNode(baseNode, withinNode, options.NodeOptions)
        match res.Latency with 
        | LatencyCompliance.Compliant latency when latency = (withinNode.Latency.Value - baseNode.Latency.Value) -> Assert.True(true)
        | _ -> Assert.True(false)
          
          
    [<Fact>]
    let ``with latency slower than threshold is slow failure`` () =

        let res = NodeChecks.CheckNetworkNode(baseNode, slowNode, options.NodeOptions)
        match res.Latency with 
        | LatencyCompliance.Failure reason when reason = LatencyFailure.Slow -> Assert.True(true)
        | _ -> Assert.True(false)
          
          
    [<Fact>]
    let ``with zero latency is none failure`` () =
        let res = NodeChecks.CheckNetworkNode(baseNode, zeroNode, options.NodeOptions)
        match res.Latency with 
        | LatencyCompliance.Failure reason when reason = LatencyFailure.None -> Assert.True(true)
        | _ -> Assert.True(false)
          
          
    [<Fact>]
    let ``with null latency is none Failure`` () =

        let res = NodeChecks.CheckNetworkNode(baseNode, nullNode, options.NodeOptions)
        match res.Latency with 
        | LatencyCompliance.Failure reason when reason = LatencyFailure.None -> Assert.True(true)
        | _ -> Assert.True(false)
          