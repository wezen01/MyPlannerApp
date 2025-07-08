namespace MyPlanner.Models
{
    public class Moneybox
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Operation { get; set; }
    }
}
