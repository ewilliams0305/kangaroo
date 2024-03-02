using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kangaroo
{
    public static  class ScanResultsExtensions
    {
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
}
