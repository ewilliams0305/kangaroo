using System;
using System.Text.Json;
using Kangaroo.Compliance;

namespace Kangaroo.UI.Database;

// ReSharper disable once ClassNeverInstantiated.Global
/// <summary>
/// Executed by the static constructor in the <see cref="SqliteDbInitializer"/>
/// </summary>
internal class ComplianceOptionsConfiguration : IConfigureRepository
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

    public static object CreateComplianceOptionsParameters(ComplianceOptions options, int id, int runId)
    {
        var parameters = new
        {
            Id = id,
            RunId = runId,
            options.TotalTimeThreshold,
            options.NodeOptions.LatencyThreshold,
            options.NodeOptions.QueryThreshold,
            options.NodeOptions.UseStrictHostname,
            options.NodeOptions.UseStrictMacAddress,
            options.NodeOptions.UseStrictWebServers,
            AllowExtraNodes = false,
        };
        return parameters;
    }
}