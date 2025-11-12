# 🗄️ Database Migration Guide - SQLite to Azure SQL

## 📊 Overview

This guide covers the migration of the WorldCup historical data from SQLite (`worldcup_clean.db`) to Azure SQL Database.

### Current Database Statistics
- **30 Tournaments** (World Cups from 1930-2022)
- **1,248 Matches**
- **3,637 Goals**
- **10,401 Players**
- **88 Teams**
- **240 Stadiums**
- **Total Database Size:** ~50 MB (SQLite)

---

## 🎯 Why Migrate to Azure SQL?

### ✅ Benefits

1. **Security**
   - Database not stored in Git repository
   - Encryption at rest and in transit
   - IP whitelisting and firewall rules
   - Automated backups with point-in-time restore

2. **Performance**
   - Optimized for read-heavy workloads (API queries)
   - Query performance insights
   - Automatic indexing recommendations
   - Lower latency for cloud-hosted API

3. **Scalability**
   - Easy to scale up/down based on demand
   - Geo-replication for high availability
   - Connection pooling

4. **Cost-Effective**
   - Basic tier: ~$5/month
   - Read-only workload is very cheap
   - Can share server with Identity database

5. **Operational**
   - Centralized management with Identity database
   - Automated backups and maintenance
   - Monitoring and alerting built-in

---

## 🏗️ Architecture Options

### Option 1: Single Server (RECOMMENDED) 💰

**Use the same Azure SQL Server for both databases:**

```
Azure SQL Server: worldcup-server.database.windows.net
├── worldcup-identity (Read/Write) - ASP.NET Identity, users, API keys
└── worldcup-data (Read-Only) - World Cup historical data
```

**Advantages:**
- Lower cost (one server, two databases)
- Simplified connection string management
- Easier to manage firewall rules
- Single point of monitoring

**Connection Strings:**
```json
{
  "ConnectionStrings": {
    "IdentityConnection": "Server=worldcup-server.database.windows.net;Database=worldcup-identity;User Id=sqladmin;Password=***;Encrypt=True;",
    "WorldCupDataConnection": "Server=worldcup-server.database.windows.net;Database=worldcup-data;User Id=sqladmin;Password=***;Encrypt=True;"
  }
}
```

---

### Option 2: Separate Servers

**Create a dedicated read-only server for WorldCup data:**

```
Server 1: worldcup-identity-server.database.windows.net
└── worldcup-identity (Read/Write)

Server 2: worldcup-data-server.database.windows.net
└── worldcup-data (Read-Only, IP restricted)
```

**Use case:** Maximum isolation, stricter IP whitelisting for data server

---

## 🚀 Migration Steps

### Prerequisites

1. **Azure CLI**
   ```bash
   # Install Azure CLI
   https://aka.ms/installazurecli
   
   # Login to Azure
   az login
   ```

2. **SQLite3 Command Line Tool**
   ```bash
   # Already available if you can query the database
   sqlite3 --version
   ```

3. **Azure Data Studio or SSMS** (Optional but recommended)
   - Download: https://aka.ms/azuredatastudio

---

### Step 1: Prepare Azure SQL Server

If you're using the same server as Identity (Option 1):

```bash
# Check your existing SQL Server name
az sql server list --resource-group worldcup-rg --query "[].name" -o tsv

# Output: worldcup-server (or similar)
```

If creating a new server (Option 2):

```bash
# Create new SQL Server
az sql server create \
  --name worldcup-data-server \
  --resource-group worldcup-rg \
  --location eastus \
  --admin-user sqladmin \
  --admin-password "YourSecurePassword123!"

# Configure firewall to allow your IP
az sql server firewall-rule create \
  --resource-group worldcup-rg \
  --server worldcup-data-server \
  --name AllowMyIP \
  --start-ip-address YOUR_IP \
  --end-ip-address YOUR_IP
```

---

### Step 2: Run Migration Script

```powershell
# Navigate to scripts folder
cd scripts

# Run migration script (using existing server - RECOMMENDED)
.\migrate-sqlite-to-azure.ps1 `
  -ServerName "worldcup-server" `
  -DatabaseName "worldcup-data" `
  -AdminUser "sqladmin" `
  -ResourceGroup "worldcup-rg"

# OR for new dedicated server
.\migrate-sqlite-to-azure.ps1 `
  -ServerName "worldcup-data-server" `
  -DatabaseName "worldcup-data" `
  -AdminUser "sqladmin" `
  -ResourceGroup "worldcup-rg"
```

**What the script does:**
1. ✅ Checks Azure login
2. ✅ Creates database if doesn't exist
3. ✅ Exports SQLite schema to SQL Server T-SQL
4. ✅ Exports all data to CSV files
5. ✅ Generates BCP import script
6. ✅ Provides connection string for appsettings.json

---

### Step 3: Create Schema in Azure SQL

1. **Open Azure Data Studio**
2. **Connect to Azure SQL Server**
   - Server: `worldcup-server.database.windows.net`
   - Authentication: SQL Login
   - Username: `sqladmin`
   - Password: `***`
   - Database: `worldcup-data`

3. **Execute Schema Script**
   - Open: `data/export/schema-tsql.sql`
   - Review the T-SQL (verify table names and data types)
   - Execute (F5)

---

### Step 4: Import Data

#### Option A: BCP (Command Line - FASTER)

```powershell
# Navigate to export folder
cd data/export

# Run the generated import script
.\import-data.ps1
```

#### Option B: Azure Data Studio (GUI - EASIER)

1. Right-click database → **Tasks** → **Import Flat File**
2. Select each CSV file
3. Choose table name
4. Verify column mappings
5. Import

**Recommended order:**
1. `Team.csv`
2. `Player.csv`
3. `Stadium.csv`
4. `Competition.csv`
5. `Tournament.csv`
6. `Match.csv`
7. `Goal.csv`
8. `Award.csv` and `AwardWinner.csv`
9. `Booking.csv`, `Substitution.csv`
10. `TournamentStage.csv`, `TournamentStanding.csv`

---

### Step 5: Verify Data Integrity

```sql
-- Check record counts
SELECT 'Tournament' as TableName, COUNT(*) as RecordCount FROM Tournament
UNION ALL
SELECT 'Match', COUNT(*) FROM Match
UNION ALL
SELECT 'Team', COUNT(*) FROM Team
UNION ALL
SELECT 'Player', COUNT(*) FROM Player
UNION ALL
SELECT 'Goal', COUNT(*) FROM Goal
UNION ALL
SELECT 'Stadium', COUNT(*) FROM Stadium;

-- Expected results:
-- Tournament: 30
-- Match: 1248
-- Team: 88
-- Player: 10401
-- Goal: 3637
-- Stadium: 240
```

```sql
-- Verify data quality
SELECT TOP 5 Year, Name, WinnerTeamName, HostCountry
FROM Tournament
ORDER BY Year DESC;

-- Verify relationships
SELECT 
    t.Name as Tournament,
    COUNT(DISTINCT m.Id) as TotalMatches,
    COUNT(DISTINCT g.Id) as TotalGoals
FROM Tournament t
LEFT JOIN Match m ON m.SourceTournamentId = t.SourceTournamentId
LEFT JOIN Goal g ON g.SourceTournamentId = t.SourceTournamentId
GROUP BY t.Name, t.Year
ORDER BY t.Year DESC;
```

---

### Step 6: Create Indexes (Performance Optimization)

```sql
-- Indexes for common queries
CREATE INDEX IX_Tournament_Year ON Tournament(Year);
CREATE INDEX IX_Match_SourceTournamentId ON Match(SourceTournamentId);
CREATE INDEX IX_Match_Stage ON Match(Stage);
CREATE INDEX IX_Goal_SourceMatchId ON Goal(SourceMatchId);
CREATE INDEX IX_Goal_SourcePlayerId ON Goal(SourcePlayerId);
CREATE INDEX IX_Team_Code ON Team(Code);
CREATE INDEX IX_Player_FamilyName ON Player(FamilyName);
```

---

### Step 7: Configure Read-Only User (Security Best Practice)

```sql
-- Create read-only user for API
CREATE USER worldcup_api WITH PASSWORD = 'ApiReadOnlyPassword123!';

-- Grant read-only access
ALTER ROLE db_datareader ADD MEMBER worldcup_api;

-- Deny write permissions
DENY INSERT, UPDATE, DELETE TO worldcup_api;
```

**Use this user in your API connection string:**
```json
{
  "ConnectionStrings": {
    "WorldCupDataConnection": "Server=worldcup-server.database.windows.net;Database=worldcup-data;User Id=worldcup_api;Password=ApiReadOnlyPassword123!;Encrypt=True;"
  }
}
```

---

## 🔧 Configuration in .NET

### Update appsettings.json

```json
{
  "ConnectionStrings": {
    "IdentityConnection": "Server=worldcup-server.database.windows.net;Database=worldcup-identity;User Id=sqladmin;Password=***;Encrypt=True;TrustServerCertificate=False;",
    "WorldCupDataConnection": "Server=worldcup-server.database.windows.net;Database=worldcup-data;User Id=worldcup_api;Password=ApiReadOnlyPassword123!;Encrypt=True;TrustServerCertificate=False;"
  }
}
```

### Add to User Secrets (Development)

```bash
dotnet user-secrets set "ConnectionStrings:WorldCupDataConnection" "Server=worldcup-server.database.windows.net;Database=worldcup-data;User Id=worldcup_api;Password=***;Encrypt=True;"
```

---

## 💰 Cost Estimation

### Basic Tier (RECOMMENDED)
- **Service Tier:** Basic
- **Storage:** 2 GB
- **DTUs:** 5
- **Cost:** ~$5 USD/month
- **Best for:** Read-only historical data, low-medium traffic

### Standard Tier (If traffic grows)
- **Service Tier:** S0
- **Storage:** 250 GB
- **DTUs:** 10
- **Cost:** ~$15 USD/month

---

## 🛡️ Security Checklist

- ✅ Database not in Git repository (added to .gitignore)
- ✅ Connection strings in User Secrets / Azure Key Vault
- ✅ Firewall rules configured (allow API server IP only)
- ✅ Read-only user for API (worldcup_api)
- ✅ SSL/TLS encryption enabled (Encrypt=True)
- ✅ Automated backups enabled (default 7 days retention)

---

## 📝 Backup Strategy

### Automatic Backups (Azure SQL Built-in)
- **Point-in-time restore:** 7 days (Basic tier)
- **Long-term retention:** Configure up to 10 years
- **Geo-redundant backup:** Optional (additional cost)

### Manual Backup (Optional)
```bash
# Export database to BACPAC
az sql db export \
  --resource-group worldcup-rg \
  --server worldcup-server \
  --name worldcup-data \
  --admin-user sqladmin \
  --admin-password *** \
  --storage-key-type StorageAccessKey \
  --storage-key *** \
  --storage-uri "https://yourstorageaccount.blob.core.windows.net/backups/worldcup-data.bacpac"
```

---

## 🧪 Testing Connection

### Test Connection from Azure Portal
1. Navigate to SQL Database → **Query editor**
2. Login with credentials
3. Run: `SELECT COUNT(*) FROM Tournament`
4. Should return: `30`

### Test Connection from .NET
```csharp
var connectionString = Configuration.GetConnectionString("WorldCupDataConnection");
using var connection = new SqlConnection(connectionString);
await connection.OpenAsync();
var result = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM Tournament");
Console.WriteLine($"Connected! Found {result} tournaments.");
```

---

## 📚 Next Steps

1. ✅ **Update DbContext** in WorldCup.Infrastructure
2. ✅ **Create Entity Models** for all tables
3. ✅ **Create Repository Pattern** for data access
4. ✅ **Implement API Endpoints** for World Cup data
5. ✅ **Add Caching** (Redis or Memory Cache) for frequently accessed data
6. ✅ **Configure Query Performance** monitoring

---

## ⚠️ Important Notes

- **Keep `worldcup_clean.db` as backup** - Don't delete until migration is fully tested
- **Export folder is excluded from Git** - Contains sensitive data
- **Use managed identity** for production deployments (no passwords in config)
- **Monitor database performance** with Azure SQL Insights

---

## 🆘 Troubleshooting

### Issue: Cannot connect to Azure SQL
**Solution:**
```bash
# Add your IP to firewall
az sql server firewall-rule create \
  --resource-group worldcup-rg \
  --server worldcup-server \
  --name AllowMyIP \
  --start-ip-address YOUR_IP \
  --end-ip-address YOUR_IP
```

### Issue: Import fails with encoding errors
**Solution:** CSV files must be UTF-8 encoded
```powershell
# Re-export with UTF-8
sqlite3 worldcup_clean.db ".mode csv" ".output file.csv" "SELECT * FROM Table"
```

### Issue: Performance is slow
**Solution:** Add missing indexes (see Step 6)

---

## 📞 Support

- Azure SQL Documentation: https://docs.microsoft.com/azure/sql-database/
- Migration Best Practices: https://docs.microsoft.com/azure/dms/
- Pricing Calculator: https://azure.microsoft.com/pricing/calculator/
