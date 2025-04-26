using Microsoft.EntityFrameworkCore;
using programowanie_w_dot_net.Data;
using programowanie_w_dot_net.Dto;
using programowanie_w_dot_net.Models;

namespace programowanie_w_dot_net.Service;

public class TransactionService
{
    private readonly BudgetDbContext _context;
    
    public TransactionService(BudgetDbContext context)
    {
        _context = context;
    }
    
    public async Task<TransactionDtoGetResponse> GetTransactionsAsync(
        int userId,
        int page,
        int pageSize,
        DateTime? dateFrom,
        DateTime? dateTo,
        int? categoryId,
        decimal? minAmount,
        decimal? maxAmount)
    {
        var query = _context.Transactions
            .Where(transaction => transaction.UserId == userId)
            .AsQueryable();

        // Ensure UTC for date filters
        if (dateFrom.HasValue)
        {
            query = query.Where(transaction => transaction.Date >= dateFrom.Value.ToUniversalTime());
        }

        if (dateTo.HasValue)
        {
            query = query.Where(transaction => transaction.Date <= dateTo.Value.ToUniversalTime());
        }

        if (categoryId.HasValue)
        {
            query = query.Where(transaction => transaction.CategoryId == categoryId.Value);
        }

        if (minAmount.HasValue)
        {
            query = query.Where(transaction => transaction.Amount >= minAmount.Value);
        }

        if (maxAmount.HasValue)
        {
            query = query.Where(transaction => transaction.Amount <= maxAmount.Value);
        }
        
        var totalItems = await query.CountAsync();
    
        var transactions = await query
            .OrderByDescending(transaction => transaction.Date)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        // Map each Category to a CategoryDto
        var transactionDtos = transactions.Select(transaction => new TransactionGetDto()
        {
            Id = transaction.Id,
            Amount = transaction.Amount,
            Date = transaction.Date,  
            CategoryId = transaction.CategoryId,
            Category = transaction.Category
        }).ToList();
        
        var transactionDtoGetResponse = new TransactionDtoGetResponse(totalItems, transactionDtos);
        
        return transactionDtoGetResponse;
    }
    
    public async Task<Transaction> CreateTransactionAsync(TransactionRequestDto transactionRequestDto, string userId)
    {
        // Ensure the Date is in UTC
        var normalizedDate = transactionRequestDto.Date.Kind switch
        {
            DateTimeKind.Unspecified => DateTime.SpecifyKind(transactionRequestDto.Date, DateTimeKind.Utc),
            DateTimeKind.Local => transactionRequestDto.Date.ToUniversalTime(),
            _ => transactionRequestDto.Date
        };

        var transaction = new Transaction
        {
            Amount = transactionRequestDto.Amount,
            CategoryId = transactionRequestDto.CategoryId,
            Date = normalizedDate,
            UserId = int.Parse(userId)
        };

        _context.Transactions.Add(transaction);
        
        await _context.SaveChangesAsync();

        return transaction;
    }
}
