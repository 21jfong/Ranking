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

        public Player GetPlayerAtRank(int rank)
        {
            return ranking.ElementAt(rank - 1);
        }

        public void AddPlayer(Player p)
        {
            if (ranking.Contains(p))
                throw new ArgumentException();
            else
                ranking.Add(p);
            ranking.Sort();
        }

        public class Player : IComparable
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }

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
