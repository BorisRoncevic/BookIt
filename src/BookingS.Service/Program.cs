using BookingS.Service.Application.Interfaces;
using BookingS.Service;
using BookingS.Service.Infrastructure;
using BookingS.Service.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<BookingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddHttpClient<INotificationClient, NotificationClient>(client =>
{
    var baseUrl = builder.Configuration["NotificationService:BaseUrl"] ?? "http://notification:8080";
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddHttpClient<IVenueClient, VenueClient>(client =>
{
    var baseUrl = builder.Configuration["VenueService:BaseUrl"] ?? "http://venue:8080";
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddScoped<IBookingService, BookingService>();

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
    var db = scope.ServiceProvider.GetRequiredService<BookingDbContext>();
    EnsureCreatedWithRetry(db);
}

app.Run();

static void EnsureCreatedWithRetry(DbContext db)
{
    for (var attempt = 1; attempt <= 10; attempt++)
    {
        try
        {
            if (db.Database.CanConnect() && !TableExists(db, "Bookings"))
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
    var shouldClose = connection.State == System.Data.ConnectionState.Closed;

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
