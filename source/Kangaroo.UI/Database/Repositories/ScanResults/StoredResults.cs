using System;

namespace Kangaroo.UI.Database;

public record StoredResults(
    Guid Id,
    byte[] ScanResults,
    string ScanResultDescription);