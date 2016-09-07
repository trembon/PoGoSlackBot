using PoGoSlackBot.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoGoSlackBot.DAL
{
    public class PogoDBContext : DbContext
    {
        public DbSet<SpawnedPokemon> PokemonSpawns { get; set; }

        public DbSet<NearbyPokemon> NearbyPokemons { get; set; }

        public DbSet<GymDetails> GymDetails { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder(this.Database.Connection.ConnectionString);

            string connString = builder.DataSource;
            if (connString.StartsWith("|DataDirectory|", StringComparison.InvariantCultureIgnoreCase))
                connString = GetDataDirectory() + connString.Substring("|DataDirectory|".Length);

            if (!File.Exists(connString))
                SQLiteConnection.CreateFile(connString);

            if (this.Database.Connection.State == ConnectionState.Open)
            {
                CreateTables(this.Database.Connection);
            }
            else
            {
                using (var conn = new SQLiteConnection(this.Database.Connection.ConnectionString))
                {
                    conn.Open();
                    CreateTables(conn);
                }
            }
        }

        private void CreateTables(DbConnection conn)
        {
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='pokemon_spawns'";
                if (cmd.ExecuteScalar() == null)
                {
                    cmd.CommandText = "create table pokemon_spawns(instance VARCHAR(100), pokemon_id int, encounter_id VARCHAR(100), spawn_point_id VARCHAR(100), latitude float, longitude float, despawn datetime, encountered datetime)";
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='gyms'";
                if (cmd.ExecuteScalar() == null)
                {
                    cmd.CommandText = "create table gyms(instance VARCHAR(100), gym_id VARCHAR(100), name VARCHAR(200), image_url VARCHAR(500), owner int, strongest_pokemon int, is_in_battle int, latitude float, longitude float, last_update datetime)";
                    cmd.ExecuteNonQuery();
                }

                cmd.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='pokemon_nearby'";
                if (cmd.ExecuteScalar() == null)
                {
                    cmd.CommandText = "create table pokemon_nearby(instance VARCHAR(100), pokemon_id int, encounter_id VARCHAR(100), scan_latitude float, scan_longitude float, encountered datetime)";
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static string GetDataDirectory()
        {
            string text = AppDomain.CurrentDomain.GetData("DataDirectory") as string;
            if (string.IsNullOrEmpty(text))
            {
                text = AppDomain.CurrentDomain.BaseDirectory;
            }
            return text;
        }
    }
}
