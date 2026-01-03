# Start both Slingcessories Service and MAUI app
Write-Host "Starting Slingcessories Service..." -ForegroundColor Green

# Start the service in a new window
Start-Process pwsh -ArgumentList "-NoExit", "-Command", "cd 'Slingcessories.Service'; dotnet run"

Write-Host "Service starting in new window..." -ForegroundColor Yellow
Write-Host "Wait for service to show 'Now listening on...' message" -ForegroundColor Yellow
Write-Host "Then run the MAUI project from Visual Studio" -ForegroundColor Cyan
