namespace programowanie_w_dot_net.Models;

public class Transaction
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
    
    // Foreign key reference to the User
    public int UserId { get; set; }
    public User User { get; set; } // Navigation property
}