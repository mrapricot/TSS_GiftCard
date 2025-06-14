using Core.Domain;
using System.Text.RegularExpressions;

namespace Core.GiftCards
{
    public class GiftCardService
    {
        private static readonly Regex CodeRegex = new(@"^GC-\d{4}-\d{4}$", RegexOptions.Compiled);
        private readonly List<GiftCard> _cards = new();

        public GiftCardService()
        {
            // seed with one demo card (active, expires in 1 year)
            _cards.Add(new GiftCard("GC-1234-5678", 150m, GiftCardStatus.Active, DateTime.UtcNow.AddYears(1)));
        }

        public string Handle(string code, string action, decimal? amount = null)
        {
            if (!ValidateCode(code))
                return "Invalid code";

            var card = _cards.FirstOrDefault(c => c.Code == code);
            if (card is null)
                return "Card not found";

            switch (action?.ToLowerInvariant())
            {
                case "balance":
                    return $"Balance: {card.Balance:F2} EUR, Status: {card.Status}, Expires: {card.ExpiryDate:yyyy-MM-dd}";                case "redeem":
                    if (amount is null || amount <= 0)
                        return "Invalid amount";
                    if (amount % 5 != 0)
                        return "Amount must be multiple of 5";
                    if (!card.IsValid())
                        return DateTime.UtcNow > card.ExpiryDate 
                            ? "Card expired" 
                            : "Card inactive";
                    try
                    {
                        card.Redeem(amount.Value);
                        return $"Success: Redeemed {amount:F2} EUR. New balance: {card.Balance:F2} EUR";
                    }
                    catch (InvalidOperationException ex)
                    {
                        return ex.Message.Contains("Insufficient") ? "Insufficient funds" : "Card invalid";
                    }                case "load":
                    if (amount is null || amount <= 0)
                        return "Invalid amount";
                    if (amount > 500)
                        return "Amount exceeds maximum load limit";
                    if (!card.IsValid())
                        return DateTime.UtcNow > card.ExpiryDate 
                            ? "Card expired" 
                            : "Card inactive";
                    try
                    {
                        card.Load(amount.Value);
                        return $"Success: Loaded {amount:F2} EUR. New balance: {card.Balance:F2} EUR";
                    }
                    catch (InvalidOperationException)
                    {
                        return "Card invalid";
                    }

                case "status":
                    if (amount is null)
                        return "Status parameter required (0=Active, 1=Inactive, 2=Expired, 3=Blocked)";
                    var newStatus = (GiftCardStatus)(int)amount.Value;
                    if (!Enum.IsDefined(typeof(GiftCardStatus), newStatus))
                        return "Invalid status";
                    card.SetStatus(newStatus);
                    return $"Status updated to: {newStatus}";

                default:
                    return "Invalid action";
            }
        }

        private static bool ValidateCode(string? code) 
        {
            return !string.IsNullOrEmpty(code) && CodeRegex.IsMatch(code);        }

        // Helper for tests to seed additional cards
        public void AddCard(GiftCard card) => _cards.Add(card);
    }
}
