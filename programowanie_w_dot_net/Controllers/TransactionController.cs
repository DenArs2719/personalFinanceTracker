using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using programowanie_w_dot_net.Data;
using programowanie_w_dot_net.Dto;
using programowanie_w_dot_net.Models;
using programowanie_w_dot_net.Service;

namespace programowanie_w_dot_net.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController(BudgetDbContext context) : ControllerBase
    {
        private readonly TransactionService _transactionService = new(context);
        
        public async Task<ActionResult<IEnumerable<TransactionDtoGetResponse>>> GetTransactions(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] int? categoryId = null,
            [FromQuery] decimal? minAmount = null,
            [FromQuery] decimal? maxAmount = null
        )
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var transactionDtos = await _transactionService.GetTransactionsAsync(
                int.Parse(userId),
                page,
                pageSize,
                dateFrom,
                dateTo,
                categoryId,
                minAmount,
                maxAmount
            );
            
            return Ok(transactionDtos);
        }
        
        [HttpPost]
        public async Task<IActionResult> PostTransaction([FromBody] TransactionRequestDto transactionRequestDto) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) {
                return Unauthorized();
            }

            transactionRequestDto.Date = transactionRequestDto.Date.Kind switch
            {
                // Ensure the Date is in UTC
                DateTimeKind.Unspecified => DateTime.SpecifyKind(transactionRequestDto.Date, DateTimeKind.Utc),
                DateTimeKind.Local => transactionRequestDto.Date.ToUniversalTime(),
                _ => transactionRequestDto.Date
            };

            var transaction = new Transaction {
                Amount = transactionRequestDto.Amount,
                CategoryId = transactionRequestDto.CategoryId,
                Date = transactionRequestDto.Date, 
                UserId = int.Parse(userId)
            };

            context.Transactions.Add(transaction);
            
            await context.SaveChangesAsync();

            return Ok(transaction);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutTransaction(int id, [FromBody] TransactionUpdateDto transactionUpdateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null)
            {
                return Unauthorized();
            }

            var transaction = await context.Transactions
                .Where(t => t.Id == id && t.UserId == int.Parse(userId))
                .FirstOrDefaultAsync();

            if (transaction == null)
            {
                return NotFound();
            }

            transaction.Amount = transactionUpdateDto.Amount;
            transaction.Date = transactionUpdateDto.Date;
            transaction.CategoryId = transactionUpdateDto.CategoryId;

            await context.SaveChangesAsync();

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTransaction(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) {
                return Unauthorized();
            }
            
            // Fetch transactions for the current user
            var transaction = await context.Transactions
                .Where(t => t.UserId == int.Parse(userId)) // Filter by UserId
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();

            if (transaction == null)
            {
                return NotFound();
            }

            context.Transactions.Remove(transaction);

            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}