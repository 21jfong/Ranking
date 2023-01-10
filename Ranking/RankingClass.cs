using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranking
{
    [JsonObject(MemberSerialization.OptIn)]
    public class RankingClass
    {
        private List<Player> ranking;
        [JsonProperty(PropertyName = "Ranking")]
        private Dictionary<string, Player> playerDictionary;
        [JsonProperty]
        public bool IsPosEnabled { get; set; }
        public bool Changed { get; set; }
        [JsonProperty]
        public int Count { get; set; }

        public RankingClass()
        {
            ranking = new();
            playerDictionary = new();
            IsPosEnabled = false;
            Changed = false;
            Count = 0;
        }

        public RankingClass(string filePath)
        {
            ranking = new();

            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.MissingMemberHandling = MissingMemberHandling.Error;

            // import the given spreadsheet and make sure it is correct
            try
            {
                string s = File.ReadAllText(filePath);
                RankingClass importedR = JsonConvert.DeserializeObject<RankingClass>(s, settings);

                if (importedR == null)
                    throw new ArgumentException("unknown path");

                IsPosEnabled = importedR.IsPosEnabled;
                playerDictionary = importedR.playerDictionary;
                Count = importedR.Count;
            }
            catch
            {
                throw new ArgumentException("failure to open");
            }

            ranking = DictToList();
        }

        private List<Player> DictToList()
        {
            List<Player> p = new();

            foreach (Player player in playerDictionary.Values)
                p.Add(player);
            return p;
        }

        public IEnumerable<Player> GetAllPlayers()
        {
            foreach (Player p in ranking)
            {
                yield return p;
            }
        }

        public IEnumerable<Player> GetAllPlayersByPos()
        {
            int pos = 0;
            int pCount = 0;
            while (pCount < ranking.Count)
            {
                List<Player> pList = ranking.FindAll(x => x.Position == pos);
                while (!pList.Any())
                {
                    pos++;
                    pList = ranking.FindAll(x => x.Position == pos);
                }
                foreach (Player player in pList)
                {
                    yield return player;
                    pCount++;
                }
                pos++;
            }
        }

        public Player GetPlayerAtRank(int rank)
        {
            ranking.Sort();
            if (ranking.Any())
                return ranking.ElementAt(rank);
            else
                throw new ArgumentException();
        }

        public void AddPlayer(Player p)
        {
            Player player = ranking.Find(x => x.Name == p.Name);
            if (player is not null)
                throw new ArgumentException();
            else
            {
                ranking.Add(p);
                Count = ranking.Count;
                playerDictionary.Add(p.Name, p);
            }
        }

        public Player GetPlayer(String name)
        {
            Player p = ranking.Find(x => x.Name == name);
            if (p is null || p.Name.Equals(""))
                throw new ArgumentException("Can't find player");
            else
                return p;
        }

        public void removePlayer(Player p)
        {
            ranking.Remove(p);
            playerDictionary.Remove(p.Name);
            Count = ranking.Count;
        }

        public void clearPlayers()
        {
            ranking.Clear();
            playerDictionary.Clear();
            Count = ranking.Count;
        }

        public void Save(string filename)
        {
            try
            {
                File.WriteAllText(filename, JsonConvert.SerializeObject(this));
                Changed = false;
            }
            catch (Exception)
            {
                throw new ArgumentException("File not found");
            }
        }

        [JsonObject]
        public class Player : IComparable
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }
            public int Position { get; set; }

            public Player()
            {
                Name = "";
                Description = "";
                Wins = 0;
                Losses = 0;
            }
            public Player(string name, string description, int wins, int losses)
            {
                Name = name;
                Description = description;
                Wins = wins;
                Losses = losses;
            }

            public Player(string name, string description, int wins, int losses, int pos)
            {
                Name = name;
                Description = description;
                Wins = wins;
                Losses = losses;
                Position = pos;
            }

            public int CompareTo(Object obj)
            {
                Player p = obj as Player;
                if (p.Wins > this.Wins)
                    return 1;
                if (p.Wins < this.Wins)
                    return -1;
                else
                    return 0;
            }
        }
    }
}
