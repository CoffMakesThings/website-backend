﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Serilog;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using W3C.Contracts.GameObjects;
using W3C.Contracts.Matchmaking;

namespace W3C.Domain.MatchmakingService;

[BsonIgnoreExtraElements]
public class Mmr
{
    public double rating { get; set; }
    public double rd { get; set; }
    public double vol { get; set; }
    public double rating_lower_bound { get; set; }
}

[BsonIgnoreExtraElements]
[BsonNoId]
public class PlayerMMrChange : UnfinishedMatchPlayer
{
    public bool won { get; set; }
    public Mmr updatedMmr { get; set; }
    public Ranking updatedRanking { get; set; }
    public string atTeamId { get; set; }
    public Race? rndRace { get; set; }

    public bool IsAt
    {
        get
        {
            return !string.IsNullOrEmpty(atTeamId);
        }
    }
}

[BsonIgnoreExtraElements]
[BsonNoId]
public class UnfinishedMatchPlayer : IMatchPlayerServerInfo
{
    public int team { get; set; }
    public string id { get; set; }
    public string battleTag { get; set; }
    public string inviteName { get; set; }
    public Race race { get; set; }
    public Mmr mmr { get; set; }
    public Ranking ranking { get; set; }
    public string country { get; set; }
    public FloPing[] floPings { get; set; }
}

[BsonIgnoreExtraElements]
public class Ranking
{
    public double rp { get; set; }
    public int rank { get; set; }
    public int? leagueId { get; set; }
    public int? leagueOrder { get; set; }
}

[BsonIgnoreExtraElements]
public class PlayerBlizzard
{
    public int id { get; set; }
    public int raceId { get; set; }
    public int gamePlayerId { get; set; }
    public int playerColor { get; set; }
    public int teamIndex { get; set; }
    public string toonName { get; set; }
    public bool won { get; set; }
    public bool isAi { get; set; }
    public string battleTag { get; set; }
    public string clanName { get; set; }
    public string avatarId { get; set; }
    public OverallScore overallScore { get; set; }
    public UnitScore unitScore { get; set; }
    public List<Hero> heroes { get; set; }
    public HeroScore heroScore { get; set; }
    public ResourceScore resourceScore { get; set; }
}

// Sync with src\app\data\models\match.ts:EMatchState
// https://github.com/w3champions/matchmaking-service/blob/2191b33b05dacc3e4deb7f6b01d411d158732eae/src/app/data/models/match.ts#L36
public enum EMatchState : int
{
    INIT = 0,
    STARTED = 1,
    FINISHED = 2,
    CANCELED = 3,
}

[BsonIgnoreExtraElements]
[BsonNoId]
public class Match : IMatchServerInfo
{
    public string serverProvider { get; set; }
    public string type { get; set; }
    public EMatchState state { get; set; }
    public long startTime { get; set; }
    public List<PlayerMMrChange> players { get; set; }
    public GameMode gameMode { get; set; }
    public GateWay gateway { get; set; }
    public int season { get; set; }
    public FloNode floNode { get; set; }
    public string host { get; set; }
    public string map { get; set; }
    public int? mapId { get; set; }
    public string mapName { get; set; }
    public bool publicGame { get; set; }
    public string gamename { get; set; }

    [BsonElement("_id")]
    public string id { get; set; }
    public int? floGameId { get; set; }
    public long endTime { get; set; }
    public List<IMatchPlayerServerInfo> PlayersServerInfo
    {
        get
        {
            return players?.Cast<IMatchPlayerServerInfo>().ToList();
        }
    }

    public bool HasFloGameId()
    {
        if (floGameId.HasValue) return true;
        return false;
    }
}

[BsonIgnoreExtraElements]
[BsonNoId]
public class UnfinishedMatch : IMatchServerInfo
{
    public int season { get; set; }
    public EMatchState state { get; set; }
    public long startTime { get; set; }
    public GameMode gameMode { get; set; }
    public GateWay gateway { get; set; }
    public string host { get; set; }
    [BsonElement("_id")]
    public string id { get; set; }
    public int? mapId { get; set; }
    public string map { get; set; }
    public string mapName { get; set; }
    public List<UnfinishedMatchPlayer> players { get; set; }
    public FloNode floNode { get; set; }
    public string serverProvider { get; set; }

    public List<IMatchPlayerServerInfo> PlayersServerInfo
    {
        get
        {
            return players?.Cast<IMatchPlayerServerInfo>().ToList();
        }
    }
}

[BsonIgnoreExtraElements]
public class OverallScore
{
    [JsonPropertyName("unitScore")]
    public int UNIT_SCORE { get; set; }
    [JsonPropertyName("heroScore")]
    public int HERO_SCORE { get; set; }
    [JsonPropertyName("resourceScore")]
    public int RESOURCE_SCORE { get; set; }
    [JsonPropertyName("totalScore")]
    public int TOTAL_SCORE { get; set; }
}

[BsonIgnoreExtraElements]
public class UnitScore
{
    [JsonPropertyName("unitsProduced")]
    public int UNITS_PRODUCED { get; set; }
    [JsonPropertyName("unitsKilled")]
    public int UNITS_KILLED { get; set; }
    [JsonIgnore]
    public int STRUCTURES_PRODUCED { get; set; }
    [JsonIgnore]
    public int STRUCTURES_RAZED { get; set; }
    [JsonPropertyName("largestArmy")]
    public int LARGEST_ARMY { get; set; }
}

[BsonIgnoreExtraElements]
public class Hero
{
    private string _icon;

    public string icon
    {
        get
        {
            var path = _icon.Replace('\\', '/');
            if (!path.Contains("UI/Glues/ScoreScreen/")) return path;
            var strings = path.Replace(".blp", "").Replace(".png", "").Split("-");
            if (strings.Length < 3)
            {
                Log.Warning("Icon was split but didn't have enough elements! {icon}", path);
                return path;
            }
            return strings[2];
        }
        set => _icon = value;
    }

    public int level { get; set; }

    // Only used for accessing icon field during Matchup creation without having parsing.
    // FIXME: Remove default parsing of icon, atm other things depend on it existing
    [JsonIgnore]
    [BsonIgnore]
    public string iconPath
    {
        get { return _icon; }
    }
}

[BsonIgnoreExtraElements]
public class HeroScore
{
    [JsonPropertyName("heroesKilled")]
    public int HEROES_KILLED { get; set; }
    [JsonPropertyName("itemsObtained")]
    public int ITEMS_OBTAINED { get; set; }
    [JsonPropertyName("mercsHired")]
    public int MERCS_HIRED { get; set; }
    [JsonPropertyName("expGained")]
    public int EXP_GAINED { get; set; }
    [JsonIgnore]
    public int STRONGER_HEROES { get; set; }
}

[BsonIgnoreExtraElements]
public class ResourceScore
{
    [JsonPropertyName("goldCollected")]
    public int GOLD_COLLECTED { get; set; }
    [JsonPropertyName("lumberCollected")]
    public int LUMBER_COLLECTED { get; set; }
    [JsonIgnore]
    public int RESOURCES_RECVD { get; set; }
    [JsonIgnore]
    public int RESOURCES_SENT { get; set; }
    [JsonIgnore]
    public int TECH_PERCENTAGE { get; set; }
    [JsonPropertyName("goldUpkeepLost")]
    public int GOLD_UPKEEP_LOST { get; set; }
}

[BsonIgnoreExtraElements]
public class MapInfo
{
    public int elapsedGameTimeTotalSeconds { get; set; }
    public int elapsedGameTimeTotalHours { get; set; }
    public int elapsedGameTimeMinutes { get; set; }
    public int elapsedGameTimeSeconds { get; set; }
    public string elapsedSec { get; set; }
    public string name { get; set; }
    public string mapFile { get; set; }
    public bool isReplay { get; set; }
    public string replayFile { get; set; }
    public int difficulty { get; set; }
    public int campaignIndex { get; set; }
    public int missionIndex { get; set; }
    public string gameType { get; set; }
}

[BsonIgnoreExtraElements]
public class Result
{
    public bool localPlayerWon { get; set; }
    public bool isHDModeEnabled { get; set; }
    public int localPlayerRace { get; set; }
    public string gameName { get; set; }
    public string gameId { get; set; }
    public List<PlayerBlizzard> players { get; set; }
    public MapInfo mapInfo { get; set; }
    public long id { get; set; }
}

[BsonIgnoreExtraElements]
public class MatchFinishedEvent : MatchmakingEvent
{
    public Match match { get; set; }
    public Result result { get; set; }
    public bool WasFromSync { get; set; }
    public bool WasFakeEvent { get; set; }
    public string MapName => match?.mapName;
}

[BsonIgnoreExtraElements]
public class MatchStartedEvent : MatchmakingEvent
{
    public UnfinishedMatch match { get; set; }
}

[BsonIgnoreExtraElements]
public class MatchCanceledEvent : MatchmakingEvent
{
    public Match match { get; set; }
}

[BsonIgnoreExtraElements]
public class MatchmakingEvent
{
    public ObjectId Id { get; set; }
}

[BsonIgnoreExtraElements]
[BsonNoId]
public class FloNode
{
    [BsonElement("country_id")]
    public string countryId { get; set; }
    public int id { get; set; }
    public string location { get; set; }
    public string name { get; set; }
}

[BsonIgnoreExtraElements]
[BsonNoId]
public class FloPing
{
    public int nodeId { get; set; }
    public int currentPing { get; set; }
    public int avgPing { get; set; }
}

public interface IMatchPlayerServerInfo
{
    string battleTag { get; }
    FloPing[] floPings { get; }
}

public interface IMatchServerInfo
{
    FloNode floNode { get; }
    string serverProvider { get; }
    List<IMatchPlayerServerInfo> PlayersServerInfo { get; }
}
