using System;
using Dapper;

namespace Kangaroo.UI.Services.Database;

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
            CreatedDateTime = timeProvider.GetLocalNow().DateTime
        };
        return parameters;
    }
}