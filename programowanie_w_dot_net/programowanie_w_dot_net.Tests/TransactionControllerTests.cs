using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using programowanie_w_dot_net.Controllers;
using programowanie_w_dot_net.Data;
using programowanie_w_dot_net.Dto;
using programowanie_w_dot_net.Models;
using Xunit;

namespace BudgetApp.Tests
{
    public class TransactionControllerTests
    {
        private TransactionController GetControllerWithUser(BudgetDbContext context, string userId)
        {
            var controller = new TransactionController(context);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId)
            }, "mock"));

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        private BudgetDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<BudgetDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new BudgetDbContext(options);
        }

        [Fact]
        public async Task PostTransaction_CreatesTransaction_ForAuthorizedUser()
        {
            var context = GetInMemoryDbContext();
            var controller = GetControllerWithUser(context, "2");

            var dto = new TransactionRequestDto
            {
                Amount = 200,
                CategoryId = 1,
                Date = DateTime.Now
            };

            var result = await controller.PostTransaction(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var transaction = Assert.IsType<Transaction>(okResult.Value);

            Assert.Equal(200, transaction.Amount);
            Assert.Equal(2, transaction.UserId);
        }

        [Fact]
        public async Task PutTransaction_UpdatesTransaction_WhenExists()
        {
            var context = GetInMemoryDbContext();
            context.Transactions.Add(new Transaction
            {
                Id = 1,
                Amount = 100,
                CategoryId = 1,
                Date = DateTime.UtcNow,
                UserId = 5
            });
            context.SaveChanges();

            var controller = GetControllerWithUser(context, "5");

            var updateDto = new TransactionUpdateDto
            {
                Amount = 150,
                CategoryId = 2,
                Date = DateTime.UtcNow
            };

            var result = await controller.PutTransaction(1, updateDto);

            Assert.IsType<NoContentResult>(result);
            Assert.Equal(150, context.Transactions.First().Amount);
        }

        [Fact]
        public async Task DeleteTransaction_RemovesTransaction_WhenExists()
        {
            var context = GetInMemoryDbContext();
            context.Transactions.Add(new Transaction
            {
                Id = 10,
                Amount = 100,
                CategoryId = 1,
                Date = DateTime.UtcNow,
                UserId = 3
            });
            context.SaveChanges();

            var controller = GetControllerWithUser(context, "3");

            var result = await controller.DeleteTransaction(10);

            Assert.IsType<NoContentResult>(result);
            Assert.Empty(context.Transactions);
        }
    }
}
