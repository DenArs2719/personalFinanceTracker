using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using programowanie_w_dot_net.Data;
using Microsoft.AspNetCore.Authorization;
using programowanie_w_dot_net.Dto;

namespace programowanie_w_dot_net.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ExpenseController : ControllerBase
{
    private readonly BudgetDbContext _context;
    
    public ExpenseController(BudgetDbContext context)
    {
        _context = context;
    }
    
    
    [HttpGet]
    public async Task<IActionResult> GetExpenseSummary()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized();
        }

        if (!int.TryParse(userId, out var parsedUserId))
        {
            return Unauthorized();
        }

        var summary = await _context.Transactions
            .Where(t => t.UserId == parsedUserId)
            .GroupBy(t => t.Category.Name)
            .Select(g => new SummaryDto
            {
                CategoryName = g.Key,
                TotalAmount = g.Sum(t => t.Amount)
            })
            .ToListAsync();

        return Ok(summary);
    }
}