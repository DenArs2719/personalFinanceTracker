namespace programowanie_w_dot_net.Dto;

public class TransactionUpdateDto
{
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public int CategoryId { get; set; }
}