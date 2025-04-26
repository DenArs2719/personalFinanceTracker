using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using programowanie_w_dot_net.Data;
using programowanie_w_dot_net.Dto;
using programowanie_w_dot_net.Models;

namespace programowanie_w_dot_net.Service;

public class CategoryService
{
    private readonly BudgetDbContext _context;
    
    public CategoryService(BudgetDbContext context)
    {
        _context = context;
    }
    
    public async Task<Category> CreateCategory(CategoryDto categoryDto, string userId)
    {
        var category = new Category()
        {
            Name = categoryDto.Name,
            UserId = int.Parse(userId)  // Convert string to int
        };
    
        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return category;
    }

    public async Task<List<Category>> GetAllCategories(int parsedUserId)
    {
        // Retrieve categories for the specific user
        var categories = await _context.Categories
            .Where(c => c.UserId == parsedUserId)
            .ToListAsync();

        return categories;
    }

    public async Task<Category?> GetCategoryById(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task DeleteCategory(Category category)
    {
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
    }
}