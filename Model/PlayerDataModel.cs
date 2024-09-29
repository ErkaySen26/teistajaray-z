using System.ComponentModel; // BindingList için gerekli


namespace PlayerManagement.Model
{
    public class PlayerDataModel
    {
        public string SavingDate { get; set; }  // JSON'da "saving date" alanı
        public BindingList<Player> Players { get; set; }  // JSON'da "players" listesi
        public BindingList<PlayerStatistics> PlayerStats { get; set; } // JSON'da "player stats" listesi

        public PlayerDataModel()
        {
            Players = new BindingList<Player>();  // BindingList olarak başlatıyoruz
            PlayerStats = new BindingList<PlayerStatistics>(); // PlayerStats'ı initialize ediyoruz
        }
    }
}
