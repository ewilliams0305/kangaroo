-- Step 1: Create the Compliance Run table
CREATE TABLE IF NOT EXISTS ComplianceRun (
                                             Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                             BaseLineScanId UUID NOT NULL,
                                             ScanMode INTEGER NOT NULL,
                                             StartAddress TEXT,
                                             EndAddress TEXT,
                                             SubnetMask TEXT,
                                             SpecifiedAddresses TEXT,
                                             Adapter TEXT,
                                             CreatedDateTime TEXT NOT NULL,
                                             ElapsedTime TEXT NOT NULL,
                                             OnlineDevices INTEGER NOT NULL,
                                             FOREIGN KEY (BaseLineScanId) REFERENCES ScanResults(Id) ON DELETE CASCADE
);

-- Step 2: Create the Compliance Option table
CREATE TABLE IF NOT EXISTS ComplianceOptions(
                                                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                                RunId INTEGER NOT NULL,
                                                UseStrictHostname BIT NOT NULL,
                                                UseStrictMacAddress BIT NOT NULL,
                                                UseStrictWebServers BIT NOT NULL,
                                                AllowExtraNodes BIT NOT NULL,
                                                FOREIGN KEY (RunId) REFERENCES ComplianceRun(Id) ON DELETE CASCADE
);

-- Step 2: Create the Compliance Option table
CREATE TABLE IF NOT EXISTS WhiteList(
                                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                                        RunId INTEGER NOT NULL,
                                        IpAddress TEXT NOT NULL,
                                        FOREIGN KEY (RunId) REFERENCES ComplianceOptions(Id) ON DELETE CASCADE
);