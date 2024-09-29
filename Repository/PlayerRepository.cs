using System;
using System.Collections.Generic;
using System.ComponentModel; // BindingList için gerekli
using System.Linq;
using PlayerManagement.Model;

namespace Test.Repository
{
    public class PlayerRepository
    {
        private static PlayerRepository _instance;
        private BindingList<Player> _players; // List yerine BindingList kullanıyoruz

        private PlayerRepository()
        {
            _players = new BindingList<Player>(); // BindingList'i initialize ediyoruz
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
            _players.Add(player); // BindingList'e ekleme
        }

        public BindingList<Player> FindAll() // BindingList türünde döndürüyoruz
        {
            Console.WriteLine($"Player count: {_players.Count}");
            return _players;
        }

        public BindingList<Player> Like(string like)
        {
            // BindingList'i filtreleme ve yeni bir BindingList döndürme
            var filteredPlayers = _players.Where(player => player.Name.IndexOf(like, StringComparison.OrdinalIgnoreCase) >= 0 ||
                                                            player.Surname.IndexOf(like, StringComparison.OrdinalIgnoreCase) >= 0)
                                           .ToList();
            return new BindingList<Player>(filteredPlayers);
        }

        public void Update(Player player)
        {
            var existingPlayer = _players.FirstOrDefault(p => p.Id == player.Id); // Burada oyuncunun benzersiz bir ID'si olduğunu varsayıyoruz.
            if (existingPlayer != null)
            {
                existingPlayer.Name = player.Name;
                existingPlayer.Surname = player.Surname;
                existingPlayer.Height = player.Height;
                existingPlayer.Weight = player.Weight;
                existingPlayer.Attack = player.Attack;
                existingPlayer.Defense = player.Defense;
            }
        }

        public void RemoveAll()
        {
            _players.Clear(); // BindingList'i temizleme
        }

        public void UpdatePlayers(List<Player> players)
        {
            RemoveAll(); // Öncelikle mevcut oyuncuları temizleyin
            foreach (var player in players)
            {
                Add(player); // Yeni oyuncuları ekleyin
            }
        }
    }
}
