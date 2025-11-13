# Database Data Quality Issues

## 📋 Issues Identified - November 12, 2025

### 1. Character Encoding Problems

**Affected Tables**: Stadium, Player, Team (any table with names/text in non-English)

**Examples Found**:
```json
// WRONG (Current state):
{
  "name": "\"Estadio Jos├⌐ Amalfitani\"",
  "city": "\"Buenos Aires\"",
  "stadiumWiki": "\"https://en.wikipedia.org/wiki/Jos├⌐_Amalfitani_Stadium\""
}

// CORRECT (Expected):
{
  "name": "Estadio José Amalfitani",
  "city": "Buenos Aires",
  "stadiumWiki": "https://en.wikipedia.org/wiki/José_Amalfitani_Stadium"
}
```

**Symptoms**:
- ✗ Characters like `é`, `ã`, `ñ`, `ü` appearing as `├⌐`, `├ú`, `├▒`, `├╝`
- ✗ Extra escaped quotes: `"\"text\""` instead of `"text"`
- ✗ URLs with corrupted characters

### 2. Double Quote Escaping

**Pattern**: Strings wrapped with extra escaped quotes
- Database stores: `"\"value\""`
- Should be: `"value"`

**Impact**: All text fields with special characters (names, cities, URLs)

---

## 🔧 Root Causes

### Encoding Issues
1. **SQLite → Azure SQL migration** without proper UTF-8 handling
2. Collation mismatch:
   - SQLite default: UTF-8
   - SQL Server default: Latin1_General_CI_AS (should be UTF8)
3. Import tool not preserving character encoding

### Quote Escaping
1. Original CSV/JSON data had pre-escaped quotes
2. Import process added another layer of escaping
3. No cleanup/normalization during migration

---

## ✅ Solutions to Implement Tomorrow

### Option 1: Fix at Database Level (RECOMMENDED)

```sql
-- 1. Create collation-fixed columns
ALTER TABLE Stadium ALTER COLUMN Name NVARCHAR(255) COLLATE Latin1_General_100_CI_AS_SC_UTF8;
ALTER TABLE Stadium ALTER COLUMN City NVARCHAR(255) COLLATE Latin1_General_100_CI_AS_SC_UTF8;
-- Repeat for all text columns in all tables

-- 2. Clean escaped quotes
UPDATE Stadium 
SET Name = TRIM('"' FROM Name),
    City = TRIM('"' FROM City),
    StadiumWiki = TRIM('"' FROM StadiumWiki);

-- 3. Fix encoding (if collation change doesn't auto-fix)
-- Re-import from source with proper UTF-8 encoding
```

### Option 2: Fix in Application Layer (TEMPORARY WORKAROUND)

Create a mapper helper to clean data on-the-fly:

```csharp
public static class DataCleanupHelper
{
    public static string CleanText(string? value)
    {
        if (string.IsNullOrEmpty(value)) return string.Empty;
        
        // Remove escaped quotes
        value = value.Trim('"');
        
        // Fix common encoding issues
        value = value.Replace("├⌐", "é")
                     .Replace("├í", "á")
                     .Replace("├│", "ó")
                     .Replace("├║", "ú")
                     .Replace("├▒", "ñ")
                     .Replace("├ú", "ã")
                     .Replace("├º", "ç");
        
        return value;
    }
}
```

**Note**: This is NOT recommended as a long-term solution!

### Option 3: Re-import Data with Correct Settings (BEST)

```bash
# SQLite export with UTF-8
sqlite3 worldcup.db ".mode csv" ".headers on" ".encoding UTF-8" ".output stadiums.csv" "SELECT * FROM Stadium;"

# Import to SQL Server with UTF-8
bcp Stadium in stadiums.csv -S worldcup-server.database.windows.net -d worldcup-data -U worldcup_admin -P <password> -c -C 65001 -t ","
```

---

## 📊 Affected Data Estimate

Based on 238 stadiums checked:
- **~40%** have encoding issues (stadiums with non-English names)
- **~60%** have escaped quotes
- **ALL URLs** with special characters are broken

**Other tables likely affected**:
- Player (10,401 records) - Names like "José", "João", etc.
- Team (68 records) - Team names with accents
- Tournament (29 records) - Host country names
- Goal, Booking, Substitution - Player/Team names denormalized

---

## 🎯 Action Items for Tomorrow

1. [ ] Export original SQLite data with UTF-8 encoding
2. [ ] Verify Azure SQL collation is UTF-8 compatible
3. [ ] Run cleanup scripts to remove escaped quotes
4. [ ] Re-import data with proper encoding
5. [ ] Verify all special characters display correctly
6. [ ] Test API responses after fix
7. [ ] Update database backup after cleanup

---

## 🚨 Priority: HIGH

**Why**: 
- Affects user-facing data quality
- Breaks Wikipedia links
- Corrupts player/team names
- Professional API cannot serve corrupted data

**Impact**:
- User Experience: ⭐ (1/5) - Names unreadable
- Data Integrity: ⭐ (1/5) - URLs broken
- SEO/Links: ⭐ (1/5) - Wikipedia links don't work

---

## 📝 Testing After Fix

```bash
# Test endpoints
GET /api/v1/stadiums?country=Argentina
GET /api/v1/players?search=José
GET /api/v1/teams?name=São

# Verify:
✓ No escaped quotes in responses
✓ Accents display correctly (é, ã, ñ, ü, ç)
✓ Wikipedia URLs work (click and test)
✓ Names are human-readable
```

---

**Created**: November 12, 2025  
**Status**: PENDING FIX  
**Assigned**: Database maintenance task
