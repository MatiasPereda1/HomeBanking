using HomeBanking.Models.Enums;

namespace HomeBanking.Models
{
    public class Card
    {
        public long Id { get; set; }
        public string CardHolder { get; set; }
        public CardType Type { get; set; }
        public CardColorType Color { get; set; }
        public string Number { get; set; }
        public int Cvv { get; set; }
        public DateTime FromDate { get; set; } = DateTime.Now;
        public DateTime ThruDate { get; set; } = DateTime.Now.AddYears(5);
        public Client Client { get; set; }
        public long ClientId { get; set; }
    }
}