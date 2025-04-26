using Microsoft.EntityFrameworkCore;
using programowanie_w_dot_net.Data;
using programowanie_w_dot_net.Dto;

namespace programowanie_w_dot_net.Service;

public class ExpenseService(BudgetDbContext context)
{
    public async Task<List<SummaryDto>> GetExpenseSummary(int parsedUserId)
    {
        var summary = await context.Transactions
            .Where(transaction => transaction.UserId == parsedUserId)
            .GroupBy(transaction => transaction.CategoryId)
            .Select(g => new
            {
                CategoryId = g.Key,
                TotalAmount = g.Sum(t => t.Amount)
            })
            .Join(context.Categories,
                g => g.CategoryId,
                category => category.Id,
                (g, category) => new SummaryDto
                {
                    CategoryName = category.Name,
                    TotalAmount = g.TotalAmount
                })
            .ToListAsync();

        return summary;
    }
}