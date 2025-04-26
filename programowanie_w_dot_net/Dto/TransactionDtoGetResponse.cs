using programowanie_w_dot_net.Models;

namespace programowanie_w_dot_net.Dto;

public class TransactionDtoGetResponse
{
    public TransactionDtoGetResponse(int TotalItems, List<TransactionGetDto> transactions)
    {
        this.TotalItems = TotalItems;
        this.Transactions = transactions;
    }
    public int TotalItems { get; set; }
   
    public List<TransactionGetDto> Transactions { get; set; }
}