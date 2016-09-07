using NLog;
using PoGoSlackBot.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.DAL
{
    public class PogoDB
    {
        private readonly Logger log;

        private static object dbLock = new object();

        private string instanceName;

        public PogoDB(string instanceName)
        {
            log = LogManager.GetLogger($"PogoDB ({instanceName})");

            this.instanceName = instanceName;
        }

        public void AddPokemonSpawn(SpawnedPokemon spawnedPokemon)
        {
            try
            {
                lock (dbLock)
                {
                    using (var context = new PogoDBContext())
                    {
                        context.PokemonSpawns.Add(spawnedPokemon);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed to add pokemon spawn to database.");
            }
        }

        public Dictionary<string, SpawnedPokemon> GetLatestSpawnData()
        {
            try
            {
                lock (dbLock)
                {
                    using (var context = new PogoDBContext())
                    {
                        return context
                            .PokemonSpawns
                            .Where(p => p.InstanceName == instanceName)
                            .GroupBy(p => p.SpawnPointID)
                            .ToDictionary(k => k.Key, v => v.OrderByDescending(p => p.Encountered).FirstOrDefault());
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed to get latest spawn data.");
                return new Dictionary<string, SpawnedPokemon>();
            }
        }

        public void AddNearbyPokemon(NearbyPokemon nearbyPokemon)
        {
            try
            {
                lock (dbLock)
                {
                    using (var context = new PogoDBContext())
                    {
                        context.NearbyPokemons.Add(nearbyPokemon);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed to add nearby pokemon to database.");
            }
        }

        public Dictionary<string, DateTime> GetLatestNearbyData()
        {
            try
            {
                lock (dbLock)
                {
                    DateTime fourHoursAgo = DateTime.Now.AddHours(-Handlers.PokemonHandler.KEEP_NEARBY_HOURS);

                    using (var context = new PogoDBContext())
                    {
                        return context
                            .NearbyPokemons
                            .Where(p => p.InstanceName == instanceName)
                            .Where(p => p.Encountered > fourHoursAgo)
                            .ToDictionary(k => k.EncounterID, v => v.Encountered);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed to get latest nearby data.");
                return new Dictionary<string, DateTime>();
            }
        }

        public void AddGymDetails(GymDetails gymDetails)
        {
            try
            {
                lock (dbLock)
                {
                    using (var context = new PogoDBContext())
                    {
                        context.GymDetails.Add(gymDetails);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed to add gym details to database.");
            }
        }

        public void UpdateGymDetails(GymDetails gymDetails)
        {
            try
            {
                lock (dbLock)
                {
                    using (var context = new PogoDBContext())
                    {
                        var oldGymDetails = context.GymDetails.FirstOrDefault(g => g.GymID == gymDetails.GymID && g.InstanceName == gymDetails.InstanceName);
                        oldGymDetails.IsInBattle = gymDetails.IsInBattle;
                        oldGymDetails.Owner = gymDetails.Owner;
                        oldGymDetails.StrongestPokemon = gymDetails.StrongestPokemon;
                        oldGymDetails.LastUpdate = gymDetails.LastUpdate;
                        
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed to update gym details in database.");
            }
        }

        public Dictionary<string, GymDetails> GetLatestGymDetails()
        {
            try
            {
                lock (dbLock)
                {
                    using (var context = new PogoDBContext())
                    {
                        return context
                            .GymDetails
                            .Where(p => p.InstanceName == instanceName)
                            .ToDictionary(k => k.GymID, v => v);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "Failed to get latest spawn data.");
                return new Dictionary<string, GymDetails>();
            }
        }
    }
}
