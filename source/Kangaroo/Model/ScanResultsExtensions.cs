using System.Text;

namespace Kangaroo;

/// <summary>
/// Helper extensions for the scan results.
/// </summary>
public static  class ScanResultsExtensions
{
    /// <summary>
    /// Converts the results to a string.
    /// </summary>
    /// <param name="results"></param>
    /// <param name="onlyAliveNodes"></param>
    /// <returns></returns>
    public static string Dump(this ScanResults results, bool onlyAliveNodes = false)
    {
        var builder = new StringBuilder();

        builder
            .AppendLine("----------------------------------------------------")
            .Append(">>> ITEMS SCANNED: ").AppendLine(results.NumberOfAddressesScanned.ToString())
            .Append(">>> ELAPSED TIME: ").AppendLine(results.ElapsedTime.ToString())
            .Append(">>> START IP: ").Append(results.StartAddress).Append(" END IP: ").AppendLine(results.EndAddress.ToString())
            .AppendLine("----------------------------------------------------").AppendLine();

        if (onlyAliveNodes)
        {
            foreach (var item in results.Nodes.Where(n => n.Alive))
            {
                builder.AppendLine(item.ToString());
            }

            return builder.ToString();
        }

        foreach (var item in results.Nodes)
        {
            builder.AppendLine(item.ToString());
        }

        return builder.ToString();
    }
}