namespace PlayerManagement.Model
{
    public class Player
    {
        public int Id { get; set; } // Benzersiz tanımlayıcı
        public string Name { get; set; }
        public string Surname { get; set; }
        public double Height { get; set; }
        public double Weight { get; set; }
        public double Attack { get; set; }
        public double Defense { get; set; }
    }

    public class PlayerStatistics
    {
        public double Attack { get; set; }
        public double Defense { get; set; }
    }
}
