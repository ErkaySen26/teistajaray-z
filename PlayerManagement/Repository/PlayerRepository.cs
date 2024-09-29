using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test.Model;

namespace Test.Repository
{
    public class PlayerRepository
    {
        private static PlayerRepository _instance;
        private List<Player> _players;

        private PlayerRepository()
        {
            _players = new List<Player>();
        }

        public static PlayerRepository GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PlayerRepository();
            }
            return _instance;
        }

        public void Add(Player player)
        {
            _players.Add(player);
        }

        public List<Player> FindAll()
        {
            Console.WriteLine($"Player count: {_players.Count}");
            return _players;
        }
        public List<Player> Like(string like)
        {
            return _players.Where(player => player.Name.IndexOf(like, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                             player.Surname.IndexOf(like, StringComparison.OrdinalIgnoreCase) >= 0)
                           .ToList();
        }

        public void RemoveAll()
        {
            _players.Clear();
        }
    }
}
