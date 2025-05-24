using Azure.Messaging.ServiceBus;
using Business.Factories;
using Business.Forms;
using Business.Models;
using Business.Services;
using Data.Contexts;
using Data.Data.Seed;
using Data.Entities;
using Data.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using QuestPDF;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// This is to use QuestPDF for PDF generation
Settings.License = LicenseType.Community;

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy( policy =>
    {
        policy.WithOrigins(
                "https://localhost:7293", 
                "https://localhost:7183"
                ) // Add PUBLISHED frontend URL here
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "InvoiceMicroservice API",
        Version = "v1"
    });
    options.EnableAnnotations();
    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        Description = "API Key needed to access the endpoints. Example: 'x-api-key: your-key-here'",
        Type = SecuritySchemeType.ApiKey,
        Name = "x-api-key",
        In = ParameterLocation.Header,
        Scheme = "ApiKeyScheme"
    });

    // x-user-id Header (Optional but required for user-role access)
    options.AddSecurityDefinition("UserId", new OpenApiSecurityScheme
    {
        Description = "User ID header (x-user-id) for identifying individual users",
        Type = SecuritySchemeType.ApiKey,
        Name = "x-user-id",
        In = ParameterLocation.Header,
        Scheme = "UserIdScheme"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "ApiKey"
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        },
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "UserId"
                },
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddDbContext<InvoiceDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));

// 2) ServiceBusClient + Processor
builder.Services.AddSingleton(sp =>
{
    // Will now find ConnectionStrings:ServiceBus
    var conn = builder.Configuration.GetConnectionString("ServiceBus");
    return new ServiceBusClient(conn);
});

builder.Services.AddSingleton(sp =>
{
    var client = sp.GetRequiredService<ServiceBusClient>();
    var options = new ServiceBusProcessorOptions { AutoCompleteMessages = false };
    return client.CreateProcessor("invoice-service", options);
});

// 3) Hosted worker that pumps messages off the queue
builder.Services.AddHostedService<InvoiceQueueHandler>();


builder.Services.AddScoped<IInvoiceRepo, InvoiceRepo>();

builder.Services.AddScoped<IInvoiceService, InvoiceService>();

builder.Services.AddScoped<IInvoicePdfService, InvoicePdfService>();

builder.Services.AddScoped<IMappingFactory<InvoiceEntity, InvoiceModel>, InvoiceMappingFactory>();
builder.Services.AddScoped<IUpdateMappingFactory<InvoiceEntity, UpdateInvoiceForm>, UpdateInvoiceMappingFactory>();
builder.Services.AddScoped<IUpdateMappingFactory<InvoiceEntity, SoftDeleteInvoiceForm>, SoftDeleteInvoiceMappingFactory>();


var app = builder.Build();

// This is purely for seeding data
/*using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InvoiceDbContext>();

    // Apply any pending EF Core migrations
    db.Database.Migrate();

    // Seed only if still empty
    InvoiceSeeder.SeedTestInvoices(db);
}*/


// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "InvoiceMicroservice v1");
});


app.UseHttpsRedirection();


app.UseCors();

// Authentication is first AND THEN Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
