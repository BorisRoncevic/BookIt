using Notification.Service;
using Notification.Service.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<SendGridSettings>(
    builder.Configuration.GetSection("SendGrid"));

var sendGridApiKey = builder.Configuration["SendGrid:ApiKey"];
var sendGridSenderEmail = builder.Configuration["SendGrid:SenderEmail"];
var useSendGrid = !string.IsNullOrWhiteSpace(sendGridApiKey)
                  && !string.IsNullOrWhiteSpace(sendGridSenderEmail)
                  && !sendGridApiKey.StartsWith("OVDE_", StringComparison.OrdinalIgnoreCase);

if (useSendGrid)
{
    builder.Services.AddScoped<IEmailService, SendGridEmailService>();
}
else
{
    builder.Services.AddScoped<IEmailService, EmailService>();
}

builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
