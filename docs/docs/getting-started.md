# Getting Started

Follow these steps to get HiveSync running locally.

## Requirements

- .NET 8 SDK
- PostgreSQL
- Node.js and npm (for React frontend)
- Optional: IDE like Visual Studio or VS Code

## Backend Setup

1. Clone the repository:

```bash
git clone https://github.com/your-org/HiveSync.git
cd HiveSync/hivesyncapi/src
```
2. Restore NuGet packages:

```bash 
dotnet restore
```
3. Build the solution: 

```bash
dotnet build
```
4. Run the API
```bash
dotnet run --project HiveSync.Api
```
