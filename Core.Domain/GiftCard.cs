namespace Core.Domain
{
    public enum GiftCardStatus
    {
        Active,
        Inactive,
        Expired,
        Blocked
    }

    public class GiftCard
    {
        public string Code { get; }
        public decimal Balance { get; private set; }
        public GiftCardStatus Status { get; private set; }
        public DateTime ExpiryDate { get; }

        public GiftCard(string code, decimal balance, GiftCardStatus status, DateTime expiryDate)
        {
            Code = code;
            Balance = balance;
            Status = status;
            ExpiryDate = expiryDate;
        }

        public bool IsValid()
        {
            return Status == GiftCardStatus.Active && DateTime.UtcNow <= ExpiryDate;
        }

        public void Load(decimal amount)
        {
            if (!IsValid())
                throw new InvalidOperationException("Cannot load to inactive or expired card");
            
            Balance += amount;
        }
        public void Redeem(decimal amount)
        {
            if (!IsValid())
                throw new InvalidOperationException("Cannot redeem from inactive or expired card");
            
            if (amount > Balance)
                throw new InvalidOperationException("Insufficient funds");
                
            Balance -= amount;
        }

        public void SetStatus(GiftCardStatus status)
        {
            Status = status;
        }
    }
}
