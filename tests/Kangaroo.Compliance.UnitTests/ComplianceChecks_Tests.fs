namespace Kangaroo.ComplianceChecks

open System
open System.Net
open Xunit
open Kangaroo
open Kangaroo.Compliance

module ``Check compliance on full scan results`` = 

    let result1 = ScanResults(
        [NetworkNode(IPAddress.Parse("192.168.1.1"), MacAddress.Empty, "", "", TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(3), true)],
        TimeSpan.FromMilliseconds(12000),
        125,
        72,
        IPAddress.Parse("192.168.1.1"),
        IPAddress.Parse("192.168.1.254"))

    [<Fact>]
    let ``with the identical data is ok`` () =

        let res = ScanChecks.CheckForCompliance (result1, result1)
        match res with 
        | Compliance.Compliant data -> Assert.True(true)
        | Compliance.Failure data -> Assert.True(false)
   
