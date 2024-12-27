namespace Kangaroo.Compliance

open System
open System.Net
open Xunit
open Kangaroo
open Kangaroo.Compliance

module ``Check number of alive devices tests`` = 

    [<Fact>]
    let ``with equal number of alive devices is ok`` () =

        let res = ScanChecks.checkAliveDevices((12, 12))
        match res with 
        | Ok ok -> Assert.True(true)
        | Error err -> Assert.True(false)
       
    [<Fact>]
    let ``with differnt number of alive devices is NumberOfAliveDevicesMatch`` () =

        let res = ScanChecks.checkAliveDevices (12, 12)
        match res with 
        | Ok ok -> 
            match ok with 
            | NumberOfAliveDevicesMatch -> Assert.True(true)
            | ElapsedTimeWithinThreshold -> Assert.True(false)
            | IpAddressesMatch -> Assert.True(false)
        | Error err -> Assert.True(false)
          
    [<Fact>]
    let ``with differnt number of alive devices is error`` () =

        let res = ScanChecks.checkAliveDevices (20, 12) 
        match res with 
        | Ok ok -> Assert.True(false)
        | Error err -> Assert.True(true)
        
    [<Fact>]
    let ``with greater number of alive devices is error`` () =

        let res = ScanChecks.checkAliveDevices((20, 21))
        match res with 
        | Ok ok -> Assert.True(false)
        | Error err -> Assert.True(true)
        
    [<Fact>]
    let ``with lesser number of alive devices is error`` () =

        let res = ScanChecks.checkAliveDevices((20, 19))
        match res with 
        | Ok ok -> Assert.True(false)
        | Error err -> Assert.True(true)
        
    [<Fact>]
    let ``with different number of alive devices is NumberOfAliveDevicesDontMatch`` () =

        let res = ScanChecks.checkAliveDevices((20, 19))
        match res with 
        | Ok ok -> Assert.True(false)
        | Error err -> 
            match err with 
            | NumberOfAliveDevicesDontMatch -> Assert.True(true)
            | ElapsedTimeExceededThreshold -> Assert.True(false)
            | IpAddressesDontMatch -> Assert.True(false)
            | _ -> Assert.True(false)

