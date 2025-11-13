WorldCup API — Backend Requirements Specification

Version: 1.0
Author: Antonio Felipe Vieira
Audience: Backend Engineering Team

🎯 1. Project Overview

The purpose of the WorldCup API is to provide a modern, scalable and feature-rich football statistics service, covering:

FIFA World Cup (Men)

FIFA Women’s World Cup

The API must expose tournament data, matches, players, teams, stadiums and all in-game events (goals, bookings, substitutions), with optional expanded views via include= and strong filtering capabilities.

The API will adopt:

.NET 9 REST API

Header-based API key authentication

Azure Services (App Service, SQL Database, Cache for Redis)

Database: Azure SQL (read-only)

The API supports free, premium and pro access tiers, each with request limits.

🏗 2. Design Principles
✔ REST-first approach

Clean RESTful API with consistent response formats.

✔ Include system

Optional expansions via ?include=:

include=goals,bookings,substitutions

include=matches,stages,standings

✔ Filtering system

Filters must be chainable:

GET ?teamCode=BRA&stage=Final&gender=male

✔ Pagination

Mandatory for high-volume tables:

Player

Goal

Booking

Substitution

Match

✔ API Versioning

Prefix all endpoints with version:

/api/v1/...

🧩 3. Global Requirements
3.1 Pagination
?page=1
&pageSize=20


Default: page=1, pageSize=10

Max pageSize: 100

Min pageSize: 1

Response includes: totalCount, totalPages, currentPage, pageSize, hasNextPage, hasPreviousPage

3.2 Sorting
?sort=year:desc,name:asc


Supports multiple fields

Values: asc or desc

Default sorting varies by endpoint (documented per endpoint)

3.3 Include Expansion
?include=goals,bookings,substitutions


Comma-separated list of relationships to include

Invalid includes are ignored (no error)

3.4 Gender Filter

Mandatory support for:

gender=male

gender=female

Valid values: male, female (case-insensitive)

Invalid values return 400 Bad Request

3.5 Response Format

All successful responses follow this structure:

{
  "data": { /* actual response object or array */ },
  "meta": {
    "timestamp": "2025-11-12T10:30:00Z",
    "page": 1,
    "pageSize": 10,
    "totalCount": 100,
    "totalPages": 10
  },
  "links": {
    "self": "/api/v1/tournaments?page=1",
    "next": "/api/v1/tournaments?page=2",
    "prev": null,
    "first": "/api/v1/tournaments?page=1",
    "last": "/api/v1/tournaments?page=10"
  }
}


Note: meta.page/pageSize/totalCount only present for paginated responses

links only present for paginated responses

3.6 Error Response Format

All error responses follow this structure:

{
  "error": {
    "code": "VALIDATION_ERROR",
    "message": "Invalid gender value. Must be 'male' or 'female'.",
    "statusCode": 400,
    "details": [
      {
        "field": "gender",
        "issue": "Invalid value 'xyz'"
      }
    ]
  }
}


Error codes:

VALIDATION_ERROR (400)

NOT_FOUND (404)

UNAUTHORIZED (401)

RATE_LIMIT_EXCEEDED (429)

INTERNAL_ERROR (500)

3.7 Query Parameter Validations
Parameter	Validation	Error if invalid
page	Integer >= 1	400 Bad Request
pageSize	1-100	400 Bad Request
year	1930-2026	400 Bad Request
gender	male or female	400 Bad Request
sort	field:direction format	400 Bad Request
include	Valid relationship names	Ignored (no error)
3.8 Caching Strategy
Endpoint Type	Cache-Control	TTL
Static (competitions, teams, stadiums)	public, max-age=86400	24 hours
Tournaments	public, max-age=3600	1 hour
Matches	public, max-age=900	15 minutes
Statistics	public, max-age=300	5 minutes
3.9 Rate Limiting

All responses include rate limit headers:

X-RateLimit-Limit: 100
X-RateLimit-Remaining: 87
X-RateLimit-Reset: 1699876800


When limit exceeded, return 429 with Retry-After header

3.10 API Access Tiers
Tier	Limits	Notes
Free	100 req/day	No /stats or /full endpoints
Premium	10,000 req/day	Full API access
Pro	Uncapped (fair use)	All features enabled
🔥 4. Complete Endpoint Specification

Below are all required REST endpoints, including filters, includes and detailed behavior.

🏆 5. Competition Endpoints
5.1 GET /api/v1/competitions
Requirements

Return list of competitions (2 total)

Fully cached

Includes

None

Filters

gender (optional)

5.2 GET /api/v1/competitions/{id}
Includes

tournaments

Requirements

Return competition info + optional list of tournaments

🏟 6. Tournament Endpoints
6.1 GET /api/v1/tournaments
Filters
Filter	Description
year	Year of tournament
hostCountry	Host country
winner	Winner team
competitionCode	WC / WCW
gender	male / female
Includes

None

6.2 GET /api/v1/tournaments/{id}
Includes
Include	Description
matches	All matches of the tournament
teams	Participating teams
stages	Group & knockout structure
standings	Final positions
hosts	Host countries
stats	Aggregated statistics (premium)
6.3 GET /api/v1/tournaments/{id}/summary
Returns

total matches

total goals

avg goals per match

total bookings

discipline summary

champion + runner-up

top scorers

best attack / best defense

6.4 GET /api/v1/tournaments/{id}/bracket
Returns

Round of 16

Quarter-finals

Semi-finals

Third-place

Final

6.5 GET /api/v1/tournaments/{id}/topscorers
Returns

List of top scorers for the tournament

Includes: playerId, playerName, teamCode, goals, penalties, minutes

Pagination: Yes

Default sort: goals desc, penalties asc

6.6 GET /api/v1/tournaments/{id}/fairplay
Returns

Fair play rankings by team

Includes: teamCode, teamName, yellowCards, redCards, points

Points calculation: yellow=-1, red=-3

Sort: points desc (best discipline first)

6.7 GET /api/v1/tournaments/{id}/awards
Returns

All award winners for the tournament

Includes: awardCode, awardName, winnerName, teamCode, shared

Examples: Golden Ball, Golden Boot, Golden Glove

6.8 GET /api/v1/tournaments/{id}/full
Premium Endpoint

Returns complete tournament data with all relationships included

Equivalent to: ?include=matches,teams,stages,standings,hosts,stats

Cache: 1 hour

📌 7. Matches

standings

stages

all matches

goals

bookings

substitutions

aggregated stats

team summaries

📌 7. Matches
7.1 GET /api/v1/matches
Filters
Filter	Description
tournamentId	Filter by tournament
homeTeamCode	Filter by home team
awayTeamCode	Filter by away team
teamCode	Matches involving team (home OR away)
stage	Tournament stage
date	YYYY-MM-DD
dateFrom	Matches from this date
dateTo	Matches until this date
result	homewin / awaywin / draw
hasExtraTime	true / false
hasPenalties	true / false
minGoals	Minimum total goals
gender	via tournament
Includes
Include	Description
goals	Match goals
bookings	Cards
substitutions	Player substitutions
tournament	Tournament details
Default sort: matchDateTime desc

7.2 GET /api/v1/matches/{id}
Includes
Include	Description
goals	Match goals
bookings	Cards
substitutions	Player substitutions
players	All players appearing in match
tournament	Tournament details
homeTeam	Home team details
awayTeam	Away team details
👤 8. Player Endpoints
8.1 GET /api/v1/players
Filters
Filter	Description
team	Team name
position	GK / DF / MF / FW
gender	male / female
tournamentId	Appeared in tournament
search	text search
Includes

None

Pagination: Yes (high-volume table: 10,401 players)

Default sort: familyName asc, givenName asc

8.2 GET /api/v1/players/{id}
Includes
Include	Description
goals	All goals
bookings	Cards
substitutions	Substitutions
awards	Awards won
matches	Matches the player played
8.3 GET /api/v1/players/search?name={query}
Requirements

Fuzzy search on givenName + familyName

Return minimal information

Pagination: Yes

8.4 GET /api/v1/players/{id}/tournaments
Returns

List of all tournaments the player participated in

Includes: tournamentId, year, competitionCode, teamCode

Pagination: Yes

8.5 GET /api/v1/players/{id}/teams
Returns

List of all teams the player played for

Includes: teamCode, teamName, tournamentCount

Note: Players can represent different teams across tournaments

🏁 9. Team Endpoints
9.1 GET /api/v1/teams
Filters
Filter	Description
region	Geographic region
confederation	UEFA / CONMEBOL / AFC...
gender	men / women
Includes

None

9.2 GET /api/v1/teams/{id}
Includes
Include	Description
matches	Home + away matches
goals	Goals scored
bookings	Cards
players	All players
standings	Final positions
tournaments	Tournament appearances
9.3 GET /api/v1/teams/{id}/stats

Premium endpoint.

Returns

matches played

wins/draws/losses

goals scored/conceded

historical tournament performance

🏟 10. Stadium Endpoints
10.1 GET /api/v1/stadiums
Filters
Filter	Description
country	Filter country
capacity_gt	min capacity
capacity_lt	max capacity
10.2 GET /api/v1/stadiums/{id}

No includes required

🏅 11. Awards Endpoints
11.1 GET /api/v1/awards

Return all award types

11.2 GET /api/v1/awards/{id}
11.3 GET /api/v1/awards/{id}/winners
Returns

winners per tournament

playerName

teamName

📊 12. Statistics Endpoints (Premium/Pro)
12.1 GET /api/v1/stats/tournaments/{id}

Return:

goal totals

average goals

card totals

substitution numbers

top scorers

discipline stats

12.2 GET /api/v1/stats/teams/{id}

Return:

goals scored/conceded

win/draw/loss ratios

best/worst performances

12.3 GET /api/v1/stats/players/{id}

Return:

goals

cards

substitution stats

tournament performance summary

12.4 GET /api/v1/stats/headtohead?team1=BRA&team2=ARG

Return:

total matches

win/loss count

goals scored/conceded

last 5 matches

12.5 GET /api/v1/stats/fairplay?tournamentId={id}

Rankings by disciplinary points.

12.6 GET /api/v1/stats/topscorers?tournamentId={id}

Return:

ordered list of scorers

penalties vs open-play split

🔍 13. Global Search Endpoint
13.1 GET /api/v1/search?q=ronaldo&type=player,team,match,tournament
Requirements

Multi-type search

Lightweight result items

Must support fuzzy matching

🛡 14. Authentication Requirements

The entire API must be protected with:

API Key header: X-Api-Key

Key must map to user tier (Free/Premium/Pro)

Rate limits per tier must be enforced.

🚀 15. Performance Requirements

Cache static data (competitions, teams, stadiums, awards)

Cache tournament lookups for 1–24h

Heavy endpoints require Premium/Pro

List endpoints must always paginate

Indexing required on:

SourceTournamentId

SourceMatchId

SourcePlayerId

SourceTeamId

📎 16. Future Enhancements

Player photos

Stadium images

Team flags

GraphQL subscriptions for real-time extensions