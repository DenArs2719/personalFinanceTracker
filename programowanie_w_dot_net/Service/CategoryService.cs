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
    
    public async Task<Category> CreateCategory(CategoryRequestDto categoryRequestDto, string userId)
    {
        var category = new Category()
        {
            Name = categoryRequestDto.Name,
            UserId = int.Parse(userId)
        };
    
        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return category;
    }

    public async Task<List<CategoryGetResponseDto>> GetAllCategories(int parsedUserId)
    {
        // Retrieve categories for the specific user
        var categories = await _context.Categories
            .Where(c => c.UserId == parsedUserId)
            .ToListAsync();
        
        // Map each Category to a CategoryDto
        var categoryDtos = categories.Select(category => new CategoryGetResponseDto
        {
            Id = category.Id,
            Name = category.Name,
        }).ToList();

        return categoryDtos;
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