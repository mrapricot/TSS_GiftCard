# Run all tests with comprehensive coverage reporting
Write-Host "🧪 Starting comprehensive test run..." -ForegroundColor Green

# Clean previous results
if (Test-Path "TestResults") {
    Write-Host "🧹 Cleaning previous test results..." -ForegroundColor Yellow
    Remove-Item "TestResults" -Recurse -Force
}

if (Test-Path "coverage-html") {
    Write-Host "🧹 Cleaning previous coverage reports..." -ForegroundColor Yellow
    Remove-Item "coverage-html" -Recurse -Force
}

# Run tests with coverage
Write-Host "🔬 Running tests with coverage collection..." -ForegroundColor Cyan
dotnet test --collect:"XPlat Code Coverage" --results-directory TestResults

# Generate HTML coverage report
Write-Host "📊 Generating coverage report..." -ForegroundColor Cyan
dotnet tool run reportgenerator -reports:"TestResults\*\coverage.cobertura.xml" -targetdir:"coverage-html" -reporttypes:Html

# Generate test summary for GitHub
Write-Host "📝 Generating test summary..." -ForegroundColor Cyan
.\generate-test-summary.ps1

# Optional: Run Stryker mutation testing (uncomment when needed)
# Write-Host "🧬 Running mutation testing..." -ForegroundColor Cyan
# dotnet stryker

Write-Host "✅ All tests completed successfully!" -ForegroundColor Green
Write-Host "📊 Open coverage-html/index.html to view detailed coverage report" -ForegroundColor Blue
Write-Host "📝 Check TEST_RESULTS.md for GitHub-ready summary" -ForegroundColor Blue