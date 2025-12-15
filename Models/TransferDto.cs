namespace OwaspTop10Demo.Models
{
    public class TransferDto
    {
        public string FromAccount { get; set; } = default!;
        public string ToAccount { get; set; } = default!;
        public decimal Amount { get; set; }
    }
}
