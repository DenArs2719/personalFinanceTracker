using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using programowanie_w_dot_net.Data;
using Microsoft.AspNetCore.Authorization;
using programowanie_w_dot_net.Dto;
using programowanie_w_dot_net.Service;

namespace programowanie_w_dot_net.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ExpenseController(BudgetDbContext context) : ControllerBase
{
    private readonly ExpenseService _expenseService = new(context);

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

        var summary = await _expenseService.GetExpenseSummary(parsedUserId);

        return Ok(summary);
    }
}