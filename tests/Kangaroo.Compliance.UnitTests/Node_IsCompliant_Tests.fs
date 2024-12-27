namespace Kangaroo.Compliance

open System
open System.Net
open Xunit
open Kangaroo
open Kangaroo.Compliance

module ``Check network node for compliant properties`` = 
    let baseNode = new NetworkNode(
        IPAddress.Any,
        new MacAddress("00:AA:BB:CC:00:01"),
        "my-fqdns-name",
        "node.kangaroo",
        TimeSpan.FromMilliseconds(200),
        TimeSpan.FromMilliseconds(200),
        true)
    
    let options = { LatencyThreshold = TimeSpan.FromMilliseconds(12); QueryThreshold = TimeSpan.FromMilliseconds(12); }
    
    [<Fact>]
    let ``with Compliant test is Compliant should be true`` () =

        let res = NodeChecks.CheckNetworkNode(baseNode, baseNode, options)
        match res.IsCompliant with 
        | true -> Assert.True(true)
        | false -> Assert.True(false)
        
    [<Fact>]
    let ``with dead node is Compliant should be false`` () =

        let res = NodeChecks.CheckNetworkNode(
            baseNode,
            new NetworkNode(
                IPAddress.Any,
                new MacAddress("00:AA:BB:CC:00:01"),
                null,
                "node.kangaroo",
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMilliseconds(200),
                false),
            options)
        match res.IsCompliant with 
        | true -> Assert.True(false)
        | false -> Assert.True(true)
         
                 
    [<Fact>]
    let ``with different mac Address node is Compliant should be false`` () =

        let res = NodeChecks.CheckNetworkNode(
            baseNode,
            new NetworkNode(
                IPAddress.Any,
                new MacAddress("00:AA:BB:CC:00:02"),
                null,
                "node.kangaroo",
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMilliseconds(200),
                true),
            options)
        match res.IsCompliant with 
        | true -> Assert.True(false)
        | false -> Assert.True(true)
        
    [<Fact>]
    let ``with different hostname node is Compliant should be false`` () =

        let res = NodeChecks.CheckNetworkNode(
            baseNode,
            new NetworkNode(
                IPAddress.Any,
                new MacAddress("00:AA:BB:CC:00:01"),
                "notmy-fqdns-name",
                "node.kangaroo",
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMilliseconds(200),
                true),
            options)
        match res.IsCompliant with 
        | true -> Assert.True(false)
        | false -> Assert.True(true)
         

    [<Fact>]
    let ``with different webserver node is Compliant should be false`` () =

        let res = NodeChecks.CheckNetworkNode(
            baseNode,
            new NetworkNode(
                IPAddress.Any,
                new MacAddress("00:AA:BB:CC:00:01"),
                "my-fqdns-name",
                "apache.kangaroo",
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMilliseconds(200),
                true),
            options)
        match res.IsCompliant with 
        | true -> Assert.True(false)
        | false -> Assert.True(true)
         
    [<Fact>]
    let ``with faulty latency node is Compliant should be false`` () =

        let res = NodeChecks.CheckNetworkNode(
            baseNode,
            new NetworkNode(
                IPAddress.Any,
                new MacAddress("00:AA:BB:CC:00:01"),
                "my-fqdns-name",
                "node.kangaroo",
                TimeSpan.FromMilliseconds(500),
                TimeSpan.FromMilliseconds(200),
                true),
            options)
        match res.IsCompliant with 
        | true -> Assert.True(false)
        | false -> Assert.True(true)
        
    [<Fact>]
    let ``with faulty query Time node is Compliant should be false`` () =

        let res = NodeChecks.CheckNetworkNode(
            baseNode,
            new NetworkNode(
                IPAddress.Any,
                new MacAddress("00:AA:BB:CC:00:01"),
                "my-fqdns-name",
                "node.kangaroo",
                TimeSpan.FromMilliseconds(200),
                TimeSpan.FromMilliseconds(500),
                true),
            options)
        match res.IsCompliant with 
        | true -> Assert.True(false)
        | false -> Assert.True(true)
         
