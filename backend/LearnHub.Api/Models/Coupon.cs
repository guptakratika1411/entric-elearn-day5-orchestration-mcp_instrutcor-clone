namespace LearnHub.Api.Models;

public class Coupon
{
    public string Code { get; set; } = "";
    public string Type { get; set; } = "Percent"; // "Percent" or "Fixed"
    public decimal Value { get; set; }
    public DateTime ExpiresOn { get; set; }
    public int MaxUses { get; set; }
    public int UsedCount { get; set; }
    public decimal MinOrderAmount { get; set; }
    public string Category { get; set; } = ""; // empty = every course
}
