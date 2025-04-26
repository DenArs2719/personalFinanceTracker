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
            
            System.Console.WriteLine("transactionDtos" + transactionDtos.Transactions);
            return Ok(transactionDtos);
        }
        
        [HttpPost]
        public async Task<IActionResult> PostTransaction([FromBody] TransactionDto transactionDto) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userId == null) {
                return Unauthorized();
            }

            // Ensure the Date is in UTC
            if (transactionDto.Date.Kind == DateTimeKind.Unspecified) {
                transactionDto.Date = DateTime.SpecifyKind(transactionDto.Date, DateTimeKind.Utc);
            } else if (transactionDto.Date.Kind == DateTimeKind.Local) {
                transactionDto.Date = transactionDto.Date.ToUniversalTime();
            }

            var transaction = new Transaction {
                Amount = transactionDto.Amount,
                CategoryId = transactionDto.CategoryId,
                Date = transactionDto.Date,  // Now it's guaranteed to be in UTC
                UserId = int.Parse(userId)  // Convert string to int
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