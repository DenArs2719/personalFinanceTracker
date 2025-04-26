namespace programowanie_w_dot_net.Models;

public class User
{
    public int Id { get; set; }
    public string Login { get; set; }
    public string PasswordHash { get; set; }
    
    public ICollection<Category> Categories { get; set; }
    public ICollection<Transaction> Transactions { get; set; }
}