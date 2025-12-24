# PowerShell script to copy data from Slinsessories to Slingcessories database
# Requires SqlServer PowerShell module: Install-Module -Name SqlServer

$serverInstance = "TRUNKS\SQLEXPRESS"
$oldDatabase = "Slinsessories"
$newDatabase = "Slingcessories"

# Tables to copy (in order of dependencies)
$tables = @(
    "Categories",
    "Accessories",
    "Slingshots",
    "AccessorySlingshots"
)

Write-Host "Starting data copy from $oldDatabase to $newDatabase..." -ForegroundColor Green

foreach ($table in $tables) {
    Write-Host "`nProcessing table: $table" -ForegroundColor Yellow
    
    # Check if table exists in source database
    $checkTableQuery = "SELECT COUNT(*) FROM $oldDatabase.INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = '$table'"
    $tableExists = Invoke-Sqlcmd -ServerInstance $serverInstance -Query $checkTableQuery -TrustServerCertificate
    
    if ($tableExists.Column1 -eq 0) {
        Write-Host "  Table $table does not exist in source database. Skipping..." -ForegroundColor Gray
        continue
    }
    
    # Get row count
    $countQuery = "SELECT COUNT(*) as RowCount FROM $oldDatabase.dbo.$table"
    $rowCount = Invoke-Sqlcmd -ServerInstance $serverInstance -Query $countQuery -TrustServerCertificate
    
    if ($rowCount.RowCount -eq 0) {
        Write-Host "  Table $table is empty. Skipping..." -ForegroundColor Gray
        continue
    }
    
    Write-Host "  Found $($rowCount.RowCount) rows to copy" -ForegroundColor Cyan
    
    # Get data from old database
    $selectQuery = "SELECT * FROM $oldDatabase.dbo.$table"
    $data = Invoke-Sqlcmd -ServerInstance $serverInstance -Query $selectQuery -TrustServerCertificate
    
    # Check if table has identity column
    $identityQuery = @"
SELECT COLUMNPROPERTY(OBJECT_ID('$table'), COLUMN_NAME, 'IsIdentity') as IsIdentity
FROM $newDatabase.INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = '$table'
AND COLUMNPROPERTY(OBJECT_ID('$table'), COLUMN_NAME, 'IsIdentity') = 1
"@
    $hasIdentity = Invoke-Sqlcmd -ServerInstance $serverInstance -Query $identityQuery -TrustServerCertificate
    
    # Enable IDENTITY_INSERT if needed
    if ($hasIdentity) {
        $enableIdentity = "SET IDENTITY_INSERT $newDatabase.dbo.$table ON;"
        Invoke-Sqlcmd -ServerInstance $serverInstance -Query $enableIdentity -Database $newDatabase -TrustServerCertificate
    }
    
    # Insert data (using bulk copy would be better for large datasets)
    $insertedCount = 0
    foreach ($row in $data) {
        try {
            $columns = ($row.PSObject.Properties | Where-Object { $_.Name -ne "RowError" -and $_.Name -ne "RowState" -and $_.Name -ne "Table" -and $_.Name -ne "ItemArray" -and $_.Name -ne "HasErrors" }).Name -join ", "
            $values = ($row.PSObject.Properties | Where-Object { $_.Name -ne "RowError" -and $_.Name -ne "RowState" -and $_.Name -ne "Table" -and $_.Name -ne "ItemArray" -and $_.Name -ne "HasErrors" } | ForEach-Object {
                if ($_.Value -eq $null) { "NULL" }
                elseif ($_.Value -is [string]) { "'$($_.Value.Replace("'", "''"))'" }
                elseif ($_.Value -is [bool]) { if ($_.Value) { "1" } else { "0" } }
                elseif ($_.Value -is [DateTime]) { "'$($_.Value.ToString("yyyy-MM-dd HH:mm:ss"))'" }
                else { $_.Value }
            }) -join ", "
            
            $insertQuery = "INSERT INTO $newDatabase.dbo.$table ($columns) VALUES ($values)"
            Invoke-Sqlcmd -ServerInstance $serverInstance -Query $insertQuery -TrustServerCertificate -ErrorAction Stop
            $insertedCount++
        }
        catch {
            Write-Host "    Error inserting row: $($_.Exception.Message)" -ForegroundColor Red
        }
    }
    
    # Disable IDENTITY_INSERT if it was enabled
    if ($hasIdentity) {
        $disableIdentity = "SET IDENTITY_INSERT $newDatabase.dbo.$table OFF;"
        Invoke-Sqlcmd -ServerInstance $serverInstance -Query $disableIdentity -Database $newDatabase -TrustServerCertificate
    }
    
    Write-Host "  Successfully copied $insertedCount rows" -ForegroundColor Green
}

Write-Host "`nData copy completed!" -ForegroundColor Green
