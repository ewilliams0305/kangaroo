namespace Kangaroo.Compliance

open System
open System.Net
open Xunit
open Kangaroo
open Kangaroo.ComplianceChecks



module ``Check IP addresses are matched tests`` = 

    let singleAddressList = [NetworkNode(IPAddress.Parse("192.168.1.1"), MacAddress.Empty, "", "", TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(3), true)]
    let multiAddressList = [
        NetworkNode(IPAddress.Parse("192.168.1.1"), MacAddress.Empty, "", "", TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(3), true);
        NetworkNode(IPAddress.Parse("192.168.1.2"), MacAddress.Empty, "", "", TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(3), true)]


    [<Fact>]
    let ``with the same address is ok`` () =

        let res = Checks.checkIpAddresses (singleAddressList, singleAddressList)
        match res with 
        | Ok ok -> Assert.True(true)
        | Error err -> Assert.True(false)

    [<Fact>]
    let ``with the same addresses is ok`` () =

        let res = Checks.checkIpAddresses (multiAddressList, multiAddressList)
        match res with 
        | Ok ok -> Assert.True(true)
        | Error err -> Assert.True(false)

    [<Fact>]
    let ``with a different addresses is error`` () =

        let res = Checks.checkIpAddresses (singleAddressList, [NetworkNode(IPAddress.Parse("192.168.1.5"), MacAddress.Empty, "", "", TimeSpan.FromSeconds(20), TimeSpan.FromSeconds(3), true)])
        match res with 
        | Ok ok -> Assert.True(false)
        | Error err -> Assert.True(true)


    [<Fact>]
    let ``with more located addresses is error`` () =

        let res = Checks.checkIpAddresses (singleAddressList, multiAddressList)
        match res with 
        | Ok ok -> Assert.True(false)
        | Error err -> Assert.True(true)


    [<Fact>]
    let ``with less located addresses is error`` () =

        let res = Checks.checkIpAddresses (multiAddressList, singleAddressList)
        match res with 
        | Ok ok -> Assert.True(false)
        | Error err -> Assert.True(true)

    [<Fact>]
    let ``with compliant addresses is IpAddressesMatch`` () =

        let res = Checks.checkIpAddresses (singleAddressList, singleAddressList)
        match res with 
        | Ok ok -> 
            match ok with 
            | ElapsedTimeWithinThreshold -> Assert.True(false)
            | NumberOfAliveDevicesMatch -> Assert.True(false)
            | IpAddressesMatch -> Assert.True(true)
        | Error err -> Assert.True(false)
        

    [<Fact>]
    let ``with mismatched addresses is IpAddressesDontMatch`` () =

        let res = Checks.checkIpAddresses (singleAddressList, multiAddressList)
        match res with 
        | Ok ok -> Assert.True(false)
        | Error err ->
            match err with 
            | NumberOfAliveDevicesDontMatch -> Assert.True(false)
            | ElapsedTimeExceededThreshold -> Assert.True(false)
            | IpAddressesDontMatch -> Assert.True(true)
            | _ -> Assert.True(false)
       
   
