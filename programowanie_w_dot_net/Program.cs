using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using programowanie_w_dot_net.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Configure CORS policy to allow any origin, method, and header
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add DbContext for PostgreSQL database
builder.Services.AddDbContext<BudgetDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Validate the token issuer
            ValidateAudience = true, // Validate the token audience
            ValidateLifetime = true, // Validate the token expiration
            ValidIssuer = builder.Configuration["Jwt:Issuer"], // Expected issuer
            ValidAudience = builder.Configuration["Jwt:Audience"], // Expected audience
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])) // Key for signature validation
        };
    });

// Add controller support
builder.Services.AddControllers();

// Configure Swagger for API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline

// Enable Swagger middleware for API documentation
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1");
    c.RoutePrefix = string.Empty; // Serve Swagger UI at application root (e.g., https://localhost:5001/)
});

// 1. Serve static files (if needed)
app.UseStaticFiles();

// 2. Enable CORS (should be placed before routing)
app.UseCors("AllowAllOrigins");

// 3. Enable routing (should come before authentication and authorization)
app.UseRouting();

// 4. Enable authentication and authorization (should come after routing)
app.UseAuthentication();
app.UseAuthorization();

// 5. Map controller routes
app.MapControllers();

// Start the application
app.Run();
