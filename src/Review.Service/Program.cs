using System.Data;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Review.Service;
using Review.Service.Repository;
using Review.Service.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddDbContext<ReviewDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IReviewRepository, ReviewRepository>();
builder.Services.AddScoped<IReviewService, ReviewService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ReviewDbContext>();
    EnsureCreatedWithRetry(db);
}

app.Run();

static void EnsureCreatedWithRetry(DbContext db)
{
    for (var attempt = 1; attempt <= 10; attempt++)
    {
        try
        {
            if (db.Database.CanConnect() && !TableExists(db, "Reviews"))
            {
                db.Database.EnsureDeleted();
            }

            db.Database.EnsureCreated();
            return;
        }
        catch (Exception ex) when (attempt < 10)
        {
            Console.WriteLine($"Database is not ready yet ({attempt}/10): {ex.Message}");
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }
}

static bool TableExists(DbContext db, string tableName)
{
    var connection = db.Database.GetDbConnection();
    var shouldClose = connection.State == ConnectionState.Closed;

    if (shouldClose)
    {
        connection.Open();
    }

    try
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT OBJECT_ID(@tableName)";

        var parameter = command.CreateParameter();
        parameter.ParameterName = "@tableName";
        parameter.Value = tableName;
        command.Parameters.Add(parameter);

        return command.ExecuteScalar() != DBNull.Value;
    }
    finally
    {
        if (shouldClose)
        {
            connection.Close();
        }
    }
}
