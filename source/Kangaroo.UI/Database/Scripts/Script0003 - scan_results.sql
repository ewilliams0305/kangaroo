-- Step 1: Create the ScanResults table
CREATE TABLE IF NOT EXISTS ScanResults (
                                           Id UUID NOT NULL PRIMARY KEY,         
                                           ScanResult BLOB NOT NULL,             
                                           ScanResultDiscriminator TEXT NOT NULL 
);

-- Step 2: Create the ScanResultMappings table
CREATE TABLE IF NOT EXISTS ScanResultMappings (
                                                  Id UUID NOT NULL PRIMARY KEY, 
                                                  ScanResultId UUID NOT NULL,       
                                                  OwnerId UUID NOT NULL,           
                                                  OwnerType TEXT NOT NULL,           
                                                  FOREIGN KEY (ScanResultId) REFERENCES ScanResults(Id) ON DELETE CASCADE
);

-- Step 4: Recreate the RecentScans table without ScanResult and ScanResultDiscriminator
CREATE TABLE IF NOT EXISTS RecentScans_New (
                                               Id UUID NOT NULL PRIMARY KEY,
                                               ScanMode INTEGER NOT NULL,
                                               StartAddress TEXT,
                                               EndAddress TEXT,
                                               SubnetMask TEXT,
                                               SpecifiedAddresses TEXT,
                                               Adapter TEXT,
                                               CreatedDateTime TEXT NOT NULL,
                                               ElapsedTime TEXT NOT NULL,
                                               OnlineDevices INTEGER NOT NULL
);

-- Copy data to the new RecentScans table
INSERT INTO RecentScans_New (
    Id, ScanMode, StartAddress, EndAddress, SubnetMask,
    SpecifiedAddresses, Adapter, CreatedDateTime,
    ElapsedTime, OnlineDevices
)
SELECT
    Id, ScanMode, StartAddress, EndAddress, SubnetMask,
    SpecifiedAddresses, Adapter, CreatedDateTime,
    ElapsedTime, OnlineDevices
    FROM RecentScans;

-- Drop the old RecentScans table
DROP TABLE RecentScans;

-- Rename RecentScans_New to RecentScans
ALTER TABLE RecentScans_New RENAME TO RecentScans;
