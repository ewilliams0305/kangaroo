-- Step 1: Add the result columns to the recent scans
ALTER TABLE RecentScans ADD COLUMN ScanResult blob NULL;
ALTER TABLE RecentScans ADD COLUMN ScanResultDiscriminator TEXT NULL; 