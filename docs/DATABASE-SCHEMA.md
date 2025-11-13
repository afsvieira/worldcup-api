# WorldCup Database Schema

## 📊 Database Overview
- **Total Tables**: 14
- **Total Records**: 29,537
- **Database Type**: Azure SQL Database (read-only)
- **Relationships**: String-based (Source*Id fields instead of FK constraints)

---

## 🏆 Core Tables

### 1. Competition (2 records)
FIFA competitions (World Cup, Women's World Cup)

| Column | Type | Description |
|--------|------|-------------|
| **Id** | int | Primary Key |
| Code | string | Competition code (e.g., "WC") |
| Name | string | Full name |
| Gender | string | Male/Female/Mixed |

**Relationships**:
- Has many: `Tournament`

---

### 2. Tournament (29 records)
Specific World Cup editions by year

| Column | Type | Description |
|--------|------|-------------|
| **Id** | int | Primary Key |
| SourceTournamentId | string | External ID |
| SourceCompetitionId | string | Links to Competition |
| Name | string | e.g., "2022 FIFA World Cup" |
| Year | int | Tournament year |
| StartDate | string | Start date |
| EndDate | string | End date |
| HostCountry | string | Host country name |
| WinnerTeamName | string | Winning team |
| HostWon | bool | Did host win? |
| TeamCount | int | Number of teams |
| HasGroupStage | bool | Has group stage? |
| HasSecondGroupStage | bool | Has second group stage? |
| HasFinalRound | bool | Has final round? |
| HasRoundOf16 | bool | Has round of 16? |
| HasQuarterFinals | bool | Has quarter-finals? |
| HasSemiFinals | bool | Has semi-finals? |
| HasThirdPlaceMatch | bool | Has third place match? |
| HasFinal | bool | Has final? |

**Relationships**:
- Belongs to: `Competition` (via SourceCompetitionId)
- Has many: `Match`, `Goal`, `Booking`, `Substitution`, `TournamentStage`, `TournamentStanding`, `HostCountry`, `AwardWinner`

---

### 3. Team (68 records)
National football teams

| Column | Type | Description |
|--------|------|-------------|
| **Id** | int | Primary Key |
| SourceTeamId | string | External ID |
| Name | string | Team name |
| Code | string | FIFA code (BRA, GER, etc.) |
| IsMenTeam | bool | Is men's team? |
| IsWomenTeam | bool | Is women's team? |
| Federation | string | Football federation |
| Region | string | Geographic region |
| ConfederationId | string | Confederation ID |
| Confederation | string | Confederation name (UEFA, CONMEBOL, etc.) |
| ConfederationCode | string | Confederation code |
| FederationWiki | string | Wikipedia link |

**Relationships**:
- Has many: `Match` (as home/away), `Goal`, `Booking`, `Substitution`, `TournamentStanding`, `AwardWinner`

---

### 4. Stadium (238 records)
Match venues

| Column | Type | Description |
|--------|------|-------------|
| **Id** | int | Primary Key |
| SourceStadiumId | string | External ID |
| Name | string | Stadium name |
| City | string | City location |
| Country | string | Country |
| Capacity | int | Seating capacity |
| StadiumWiki | string | Wikipedia link |
| CityWiki | string | City Wikipedia link |

**Relationships**:
- Has many: `Match`

---

## ⚽ Match & Events Tables

### 5. Match (1,248 records)
Individual matches between teams

| Column | Type | Description |
|--------|------|-------------|
| **Id** | int | Primary Key |
| SourceMatchId | string | External ID |
| SourceTournamentId | string | Links to Tournament |
| SourceStadiumId | string | Links to Stadium |
| SourceHomeTeamId | string | Links to Team (home) |
| SourceAwayTeamId | string | Links to Team (away) |
| Name | string | Match name/description |
| Stage | string | Tournament stage |
| GroupName | string | Group name (if applicable) |
| MatchDateTime | string | Match date/time |
| HomeScore | int | Home team score |
| AwayScore | int | Away team score |
| HomePenalties | int | Home penalties scored |
| AwayPenalties | int | Away penalties scored |
| HasExtraTime | bool | Went to extra time? |
| HasPenaltyShootout | bool | Went to penalties? |
| Result | string | Match result code |
| HomeWin | bool | Home team won? |
| AwayWin | bool | Away team won? |
| IsDraw | bool | Match drawn? |

**Relationships**:
- Belongs to: `Tournament`, `Stadium`, `Team` (home/away)
- Has many: `Goal`, `Booking`, `Substitution`

---

### 6. Goal (3,637 records) ⭐ **HIGH VOLUME**
Goals scored in matches

| Column | Type | Description |
|--------|------|-------------|
| **Id** | int | Primary Key |
| SourceGoalId | string | External ID |
| SourceTournamentId | string | Links to Tournament |
| SourceMatchId | string | Links to Match |
| SourceTeamId | string | Links to Team |
| SourcePlayerId | string | Links to Player |
| PlayerName | string | Player name (denormalized) |
| TeamName | string | Team name (denormalized) |
| MinuteLabel | string | Display label (e.g., "45+2") |
| MinuteRegulation | int | Regulation minute |
| MinuteStoppage | int | Stoppage time |
| MatchPeriod | string | Period (FirstHalf, SecondHalf, ExtraTime, etc.) |
| IsOwnGoal | bool | Own goal? |
| IsPenalty | bool | Penalty kick? |

**Key Queries**:
- Top scorers by tournament
- Goals per match
- Goals by player
- Goals by team
- Penalty statistics

**Relationships**:
- Belongs to: `Tournament`, `Match`, `Team`, `Player`

---

### 7. Booking (3,178 records) ⭐ **HIGH VOLUME**
Yellow and red cards

| Column | Type | Description |
|--------|------|-------------|
| **Id** | int | Primary Key |
| SourceBookingId | string | External ID |
| SourceTournamentId | string | Links to Tournament |
| SourceMatchId | string | Links to Match |
| SourceTeamId | string | Links to Team |
| SourcePlayerId | string | Links to Player |
| PlayerName | string | Player name (denormalized) |
| TeamName | string | Team name (denormalized) |
| MinuteLabel | string | Display label |
| MinuteRegulation | int | Regulation minute |
| MinuteStoppage | int | Stoppage time |
| MatchPeriod | string | Period |
| IsYellowCard | bool | Yellow card? |
| IsRedCard | bool | Red card? |
| IsSecondYellow | bool | Second yellow (red)? |
| IsSendingOff | bool | Sending off? |

**Key Queries**:
- Cards per match
- Most disciplined/undisciplined teams
- Red card incidents
- Fair play statistics

**Relationships**:
- Belongs to: `Tournament`, `Match`, `Team`, `Player`

---

### 8. Substitution (10,222 records) ⭐⭐ **HIGHEST VOLUME**
Player substitutions

| Column | Type | Description |
|--------|------|-------------|
| **Id** | int | Primary Key |
| SourceSubstitutionId | string | External ID |
| SourceTournamentId | string | Links to Tournament |
| SourceMatchId | string | Links to Match |
| SourceTeamId | string | Links to Team |
| SourcePlayerId | string | Links to Player |
| PlayerName | string | Player name (denormalized) |
| TeamName | string | Team name (denormalized) |
| MinuteLabel | string | Display label |
| MinuteRegulation | int | Regulation minute |
| MinuteStoppage | int | Stoppage time |
| MatchPeriod | string | Period |
| IsGoingOff | bool | Player going off? |
| IsComingOn | bool | Player coming on? |

**Key Queries**:
- Substitutions per match
- Substitution timing patterns
- Most substituted players
- Team substitution strategies

**Relationships**:
- Belongs to: `Tournament`, `Match`, `Team`, `Player`

---

## 👤 Player Table

### 9. Player (10,401 records) ⭐⭐ **HIGH VOLUME**
Football players who participated

| Column | Type | Description |
|--------|------|-------------|
| **Id** | int | Primary Key |
| SourcePlayerId | string | External ID |
| FamilyName | string | Last name |
| GivenName | string | First name |
| TeamName | string | Team name (denormalized) |
| Position | string | Position (GK, DF, MF, FW) |
| BirthDate | string | Date of birth |
| IsFemale | bool | Female player? |
| IsGoalkeeper | bool | Goalkeeper? |
| IsDefender | bool | Defender? |
| IsMidfielder | bool | Midfielder? |
| IsForward | bool | Forward? |
| TournamentCount | int | Number of tournaments |
| TournamentList | string | List of tournaments |
| WikiLink | string | Wikipedia link |

**Key Queries**:
- Player search by name
- Players by team
- Players by position
- Career statistics
- Tournament appearances

**Relationships**:
- Has many: `Goal`, `Booking`, `Substitution`, `AwardWinner`

---

## 🏅 Awards Tables

### 10. Award (8 records)
Award types

| Column | Type | Description |
|--------|------|-------------|
| **Id** | int | Primary Key |
| SourceAwardId | string | External ID |
| Name | string | Award name (Golden Ball, Golden Boot, Golden Glove, etc.) |
| Description | string | Award description |

**Relationships**:
- Has many: `AwardWinner`

---

### 11. AwardWinner (200 records)
Award recipients by tournament

| Column | Type | Description |
|--------|------|-------------|
| **Id** | int | Primary Key |
| SourceTournamentId | string | Links to Tournament |
| SourceAwardId | string | Links to Award |
| SourcePlayerId | string | Links to Player |
| SourceTeamId | string | Links to Team |
| PlayerName | string | Player name (denormalized) |
| TeamName | string | Team name (denormalized) |
| Shared | bool | Shared award? |

**Relationships**:
- Belongs to: `Tournament`, `Award`, `Player`, `Team`

---

## 📍 Tournament Structure Tables

### 12. HostCountry (31 records)
Host countries (supports multi-host tournaments)

| Column | Type | Description |
|--------|------|-------------|
| **Id** | int | Primary Key |
| SourceHostCountryId | string | External ID |
| SourceTournamentId | string | Links to Tournament |
| CountryName | string | Host country name |

**Relationships**:
- Belongs to: `Tournament`

---

### 13. TournamentStage (155 records)
Tournament phases

| Column | Type | Description |
|--------|------|-------------|
| **Id** | int | Primary Key |
| SourceTournamentId | string | Links to Tournament |
| StageNumber | int | Stage number |
| StageName | string | Stage name (Group Stage, Round of 16, etc.) |
| IsGroupStage | bool | Is group stage? |
| IsKnockoutStage | bool | Is knockout stage? |
| HasUnbalancedGroups | bool | Has unbalanced groups? |
| StartDate | string | Stage start date |
| EndDate | string | Stage end date |
| MatchCount | int | Number of matches |
| TeamCount | int | Number of teams |
| ScheduledCount | int | Scheduled matches |
| ReplayCount | int | Replay matches |
| PlayoffCount | int | Playoff matches |
| WalkoverCount | int | Walkovers |

**Relationships**:
- Belongs to: `Tournament`

---

### 14. TournamentStanding (120 records)
Final team positions/rankings

| Column | Type | Description |
|--------|------|-------------|
| **Id** | int | Primary Key |
| SourceTournamentId | string | Links to Tournament |
| SourceTeamId | string | Links to Team |
| TeamName | string | Team name (denormalized) |
| Position | int | Final position (1 = Winner) |

**Relationships**:
- Belongs to: `Tournament`, `Team`

---

## 🔑 Important Notes

### String-Based Relationships
O banco **não usa Foreign Keys tradicionais**. As relações são feitas através de campos `Source*Id` que são strings:
- `SourceTournamentId` → liga a `Tournament.Id.ToString()`
- `SourceMatchId` → liga a `Match.Id.ToString()`
- `SourceTeamId` → liga a `Team.Id.ToString()`
- `SourcePlayerId` → liga a `Player.Id.ToString()`

### Denormalization
Vários campos estão **denormalizados** para facilitar queries:
- `PlayerName` em `Goal`, `Booking`, `Substitution`, `AwardWinner`
- `TeamName` em `Goal`, `Booking`, `Substitution`, `Player`, `AwardWinner`, `TournamentStanding`

### High Volume Tables (Require Pagination)
1. **Substitution** - 10,222 registros
2. **Player** - 10,401 registros
3. **Goal** - 3,637 registros
4. **Booking** - 3,178 registros

---

## 📈 Common Query Patterns

### 1. Match Details with Events
```
Match → Goals + Bookings + Substitutions
(1 match can have ~3 goals, ~2.5 bookings, ~8 substitutions)
```

### 2. Tournament Overview
```
Tournament → Matches → Goals/Bookings/Substitutions
Tournament → Stages
Tournament → Standings
Tournament → HostCountry
```

### 3. Team Statistics
```
Team → Matches (home + away)
Team → Goals (scored)
Team → Bookings
Team → Substitutions
Team → TournamentStandings
```

### 4. Player Career
```
Player → Goals
Player → Bookings
Player → Substitutions
Player → Awards
```

---

## 💡 Recommendations for API Design

### Pagination Strategy
- **Default Page Size**: 10-20 items
- **Max Page Size**: 100 items
- **Tables requiring pagination**: Goal, Booking, Substitution, Player, Match

### Include/Expand Pattern
Suportar query params como `?include=goals,bookings` para enriquecer respostas:
```
GET /api/v1/matches/{id}?include=goals,bookings,substitutions
GET /api/v1/tournaments/{id}?include=stages,standings
```

### Performance Considerations
- Índices em: `SourceTournamentId`, `SourceMatchId`, `SourceTeamId`, `SourcePlayerId`
- Caching para: Teams, Stadiums, Competitions, Awards
- Paginação obrigatória para: Goals, Bookings, Substitutions, Players
