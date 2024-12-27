-- Step 1: Create the RecentScans table
CREATE TABLE IF NOT EXISTS RecentScans (
    Id UUID NOT NULL,
    ScanMode INTEGER NOT NULL,
    StartAddress TEXT,
    EndAddress TEXT,
    SubnetMask TEXT,
    SpecifiedAddresses TEXT,
    Adapter TEXT,
    CreatedDateTime TEXT NOT NULL,
    ElapsedTime TEXT NOT NULL,
    OnlineDevices INTEGER NOT NULL)