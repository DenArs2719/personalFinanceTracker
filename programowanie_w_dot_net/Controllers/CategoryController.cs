using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using programowanie_w_dot_net.Data;
using programowanie_w_dot_net.Models;
using Microsoft.AspNetCore.Authorization;
using programowanie_w_dot_net.Dto;
using programowanie_w_dot_net.Service;

namespace programowanie_w_dot_net.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;

    public CategoryController(BudgetDbContext context)
    {
        _categoryService = new CategoryService(context);
    }
    
    // GET: api/Category
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
    {
        // Retrieve the user ID from the JWT token
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized(); // If no user ID is found, return unauthorized
        }

        // Convert userId to integer (or another type depending on your data model)
        if (!int.TryParse(userId, out var parsedUserId))
        {
            return Unauthorized(); // If userId cannot be parsed, return unauthorized
        }

        // Retrieve categories for the specific user
        var categories = await _categoryService.GetAllCategories(parsedUserId);

        return Ok(categories);
    }

    [HttpPost]
    public async Task<ActionResult<Category>> PostCategory([FromBody] CategoryDto categoryDto)
    {
        // Retrieve the user ID from the JWT token
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized(); // If no user ID is found, return unauthorized
        }

        var category = await _categoryService.CreateCategory(categoryDto, userId);
        
        return Ok(category);
    }

    // DELETE: api/Category/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        // Retrieve the user ID from the JWT token
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userId == null)
        {
            return Unauthorized(); // If no user ID is found, return unauthorized
        }

        // Find the category by ID
        var category = await _categoryService.GetCategoryById(id);
    
        if (category == null)
        {
            return NotFound("Category not found");
        }

        await _categoryService.DeleteCategory(category);
    
        return NoContent(); // Return 204 No Content on successful deletion
    }
}