using Kangaroo.Compliance;

namespace Kangaroo.UI.Database;

/// <summary>
/// An executed compliance test
/// The compliance run contains all the resulting compliance data as well as the data used as the comparison.
/// </summary>
/// <param name="Id">The runs ID</param>
/// <param name="Options">The configuration options used for the run</param>
/// <param name="BaselineData">The run baseline data</param>
/// <param name="AnalysisData">The data from the scan</param>
public sealed record ComplianceRun(
    int Id,
    ComplianceOptions Options,
    ScanResults BaselineData,
    ScanResults AnalysisData);