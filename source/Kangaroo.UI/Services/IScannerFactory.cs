using System;
using Kangaroo.UI.Controls;

namespace Kangaroo.UI.Services;

/// <summary>
/// Creates scanners from configuration options.
/// </summary>
public interface IScannerFactory
{
    /// <summary>
    /// Provides the scan and the required contextual data to run and display a scan.
    /// </summary>
    Action<(IScanner? scanner, ScanConfiguration? configuration, bool valid)>? OnScannerCreated { get; set; }

    /// <summary>
    /// Creates the correct scanner for the data provided
    /// </summary>
    /// <param name="options">scan configuration data</param>
    void CreateScanner(ScanConfiguration options);
}