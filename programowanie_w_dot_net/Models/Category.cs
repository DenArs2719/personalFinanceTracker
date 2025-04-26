namespace programowanie_w_dot_net.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int UserId { get; set; }  // Foreign key to User
    
    public User User { get; set; }  // Navigation property
    
    // 👇 Add this!
    public ICollection<Transaction> Transactions { get; set; }
}
