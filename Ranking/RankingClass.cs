using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ranking
{
    public class RankingClass
    {
        private readonly List<Player> ranking;
        public bool IsPosEnabled { get; set; }

        public RankingClass()
        {
            ranking = new();
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
            for (int i = 0; i < ranking.Count; i++)
            {
                Player p = ranking.Find(x => x.Position == i);
                while (p is null)
                {
                    i++;
                    p = ranking.Find(x => x.Position == i);
                }
                yield return p;
            }
        }

        public Player GetPlayerAtRank(int rank)
        {
            ranking.Sort();
            if (ranking.Any())
                return ranking.ElementAt(rank - 4);
            else
                throw new ArgumentException();
        }

        public void AddPlayer(Player p)
        {
            Player player = ranking.Find(x => x.Name == p.Name);
            if (player is not null)
                throw new ArgumentException();
            else
                ranking.Add(p);
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
        }

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
