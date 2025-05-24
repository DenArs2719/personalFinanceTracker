using Xunit;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using programowanie_w_dot_net.Controllers;
using programowanie_w_dot_net.Data;
using programowanie_w_dot_net.Dto;
using programowanie_w_dot_net.Models;


namespace BudgetApp.Tests;

public class CategoryControllerTests
{
    
    [Fact]
    public async Task PostCategory_CreatesCategory_ForAuthorizedUser()
    {
        var context = GetInMemoryDbContext();
        var controller = GetControllerWithUser(context, "2");
        // Arrange
        var requestDto = new CategoryRequestDto { Name = "Transport" };
        
        // Act
        var result = await controller.PostCategory(requestDto);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var category = Assert.IsType<Category>(okResult.Value);
        Assert.Equal("Transport", category.Name);
    }
    
    [Fact]
    public async Task DeleteCategory_DeletesCategory_ForAuthorizedUser()
    {
        // Arrange
        var context = GetInMemoryDbContext();
        var categoryId = 5;

        context.Categories.Add(new Category
        {
            Id = categoryId,
            Name = "Food",
            UserId = 2
        });
        context.SaveChanges();

        var controller = GetControllerWithUser(context, "2");

        // Act
        var result = await controller.DeleteCategory(categoryId);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }
    
    private CategoryController GetControllerWithUser(BudgetDbContext context, string userId)
    {
        var controller = new CategoryController(context);

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
}