namespace MyPlanner.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string Type { get; set; } // "Entrata" o "Spesa"
        public string Category { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        public string UserId { get; set; } = string.Empty;

    }
}
