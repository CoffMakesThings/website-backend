using System.Threading.Tasks;
using W3ChampionsStatisticService.PlayerStats.RaceOnMapStats;
using W3ChampionsStatisticService.PlayerStats.RaceOnMapVersusRaceStats;
using W3ChampionsStatisticService.PlayerStats.RaceVersusRaceStats;
using W3ChampionsStatisticService.Ports;
using W3ChampionsStatisticService.ReadModelBase;

namespace W3ChampionsStatisticService.PlayerStats
{
    public class PlayerStatsRepository : MongoDbRepositoryBase, IPlayerStatsRepository
    {
        public Task<RaceVersusRaceRatio> LoadRaceStat(string battleTag)
        {
            return LoadFirst<RaceVersusRaceRatio>(p => p.Id == battleTag);
        }

        public Task UpsertRaceStat(RaceVersusRaceRatio raceVersusRaceRatio)
        {
            return Upsert(raceVersusRaceRatio, p => p.Id == raceVersusRaceRatio.Id);
        }

        public Task<RaceOnMapRatio> LoadMapStat(string battleTag)
        {
            return LoadFirst<RaceOnMapRatio>(p => p.Id == battleTag);
        }

        public Task UpsertMapStat(RaceOnMapRatio raceOnMapRatio)
        {
            return Upsert(raceOnMapRatio, p => p.Id == raceOnMapRatio.Id);
        }

        public Task<RaceOnMapVersusRaceRatio> LoadMapAndRaceStat(string battleTag)
        {
            return LoadFirst<RaceOnMapVersusRaceRatio>(p => p.Id == battleTag);
        }

        public Task UpsertMapAndRaceStat(RaceOnMapVersusRaceRatio raceOnMapVersusRaceRatio)
        {
            return Upsert(raceOnMapVersusRaceRatio, p => p.Id == raceOnMapVersusRaceRatio.Id);
        }

        public PlayerStatsRepository(DbConnctionInfo connectionInfo) : base(connectionInfo)
        {
        }
    }
}