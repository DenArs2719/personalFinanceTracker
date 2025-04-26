using Microsoft.EntityFrameworkCore;
using programowanie_w_dot_net.Data;
using programowanie_w_dot_net.Dto;

namespace programowanie_w_dot_net.Service;

public class ExpenseService(BudgetDbContext context)
{
    public async Task<List<SummaryDto>> GetExpenseSummary(int parsedUserId)
    {
        var summary = await context.Transactions
            .Where(t => t.UserId == parsedUserId)
            .GroupBy(t => t.Category.Name)
            .Select(g => new SummaryDto
            {
                CategoryName = g.Key,
                TotalAmount = g.Sum(t => t.Amount)
            })
            .ToListAsync();

        return summary;
    }
}