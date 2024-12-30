using System;
using System.Text.Json;

namespace Kangaroo.UI.Database;

// ReSharper disable once ClassNeverInstantiated.Global
/// <summary>
/// Executed by the static constructor in the <see cref="SqliteDbInitializer"/>
/// </summary>
internal class RecentScansConfiguration : IConfigureRepository
{
    public bool Configure()
    {
        try
        {
            return true;

        }
        catch (Exception)
        {
            return false;
        }
    }

    public static object CreateRecentScansParameters(RecentScan recentScan, TimeProvider timeProvider)
    {
        var parameters = new
        {
            recentScan.Id,
            recentScan.ScanMode,
            recentScan.StartAddress,
            recentScan.EndAddress,
            recentScan.SubnetMask,
            recentScan.SpecifiedAddresses,
            recentScan.Adapter,
            CreatedDateTime = timeProvider.GetLocalNow().DateTime,
            recentScan.ElapsedTime,
            recentScan.OnlineDevices
        };
        return parameters;
    }
}