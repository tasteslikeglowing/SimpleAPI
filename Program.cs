using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options =>
  {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = "yourIssuer",
        ValidAudience = "yourAudience",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("yourSecretKey1234567891011121314")),
        SignatureValidator = (token, parameters) =>
    {
        var handler = new JwtSecurityTokenHandler();
        return handler.ReadJwtToken(token); // Skip kid lookup
    }

    };
  });

// Build and configure app
var app = builder.Build();

app.UseAuthentication(); // Must be before authorization
app.UseAuthorization();



// Development-only error page
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// 🌐 Log requests early for visibility
app.UseMiddleware<LoggingMiddleware>(); // Custom logger (method/path/status)

// ⚠️ Global exception handler
app.UseMiddleware<ErrorHandlingMiddleware>(); // Catch and format exceptions

// 🔐 Security
app.UseHttpsRedirection();
app.UseMiddleware<TokenValidationMiddleware>(); // Validate token manually (if not using built-in JWT)
app.UseAuthentication(); // Built-in JWT or auth scheme
app.UseAuthorization();

// 🚀 Routing
app.MapControllers();
app.Run();


// 🛠 Global exception handler (could come after logging if preferred)
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\": \"An unexpected error occurred.\"}");
    });
});

// Map your controller routes
app.MapControllers();

app.Run();


app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = 500;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync("{\"error\": \"An unexpected error occurred.\"}");
    });
});

