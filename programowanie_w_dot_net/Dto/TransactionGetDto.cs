using programowanie_w_dot_net.Models;

namespace programowanie_w_dot_net.Dto;

public class TransactionGetDto
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; }
}