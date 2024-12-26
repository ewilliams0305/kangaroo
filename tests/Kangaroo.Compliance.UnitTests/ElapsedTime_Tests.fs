namespace Kangaroo.Compliance

open System
open System.Net
open Xunit
open Kangaroo
open Kangaroo.Compliance

module ``Check elapsed scan time tests`` = 

    [<Fact>]
    let ``with equal elapsed time is ok`` () =

        let res = ScanChecks.checkElapsedTime (TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
        match res with 
        | Ok ok -> Assert.True(true)
        | Error err -> Assert.True(false)
        
    [<Fact>]
    let ``with greater elapsed time within threashold is ok`` () =

        let res = ScanChecks.checkElapsedTime (TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(1200), TimeSpan.FromMilliseconds(500))
        match res with 
        | Ok ok -> Assert.True(true)
        | Error err -> Assert.True(false)
        

    [<Fact>]
    let ``with lesser elapsed time is always ok`` () =

        let res = ScanChecks.checkElapsedTime (TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(200), TimeSpan.FromMilliseconds(500))
        match res with 
        | Ok ok -> Assert.True(true)
        | Error err -> Assert.True(false)        

    [<Fact>]
    let ``with greater elapsed time above threashold is error`` () =

        let res = ScanChecks.checkElapsedTime (TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(11), TimeSpan.FromMilliseconds(500))
        match res with 
        | Ok ok -> Assert.True(false)
        | Error err -> Assert.True(true)

    [<Fact>]
    let ``with compliant elapsed time is ElapsedTimeWithinThreashold`` () =

        let res = ScanChecks.checkElapsedTime (TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1))
        match res with 
        | Ok ok -> 
            match ok with 
            | ElapsedTimeWithinThreshold -> Assert.True(true)
            | NumberOfAliveDevicesMatch -> Assert.True(false)
            | IpAddressesMatch -> Assert.True(false)
        | Error err -> Assert.True(false)
        

    [<Fact>]
    let ``with greater elapsed time above threashold is ElapsedTimeExceededThreashold`` () =

        let res = ScanChecks.checkElapsedTime (TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(11), TimeSpan.FromMilliseconds(500))
        match res with 
        | Ok ok -> Assert.True(false)
        | Error err ->
            match err with 
            | NumberOfAliveDevicesDontMatch -> Assert.True(false)
            | ElapsedTimeExceededThreshold -> Assert.True(true)
            | IpAddressesDontMatch -> Assert.True(false)
            | _ -> Assert.True(false)
       
   
