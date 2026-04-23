
using Notification.Service;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<SendGridSettings>(
    builder.Configuration.GetSection("SendGrid"));

builder.Services.AddScoped<IEmailService>();
builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();