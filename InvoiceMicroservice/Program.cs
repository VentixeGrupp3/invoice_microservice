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

var builder = WebApplication.CreateBuilder(args);


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


builder.Services.AddScoped<IInvoiceRepo, InvoiceRepo>();

builder.Services.AddScoped<IInvoiceService, InvoiceService>();

builder.Services.AddScoped<IMappingFactory<InvoiceEntity, InvoiceModel>, InvoiceMappingFactory>();
builder.Services.AddScoped<IUpdateMappingFactory<InvoiceEntity, UpdateInvoiceForm>, UpdateInvoiceMappingFactory>();
builder.Services.AddScoped<IUpdateMappingFactory<InvoiceEntity, SoftDeleteInvoiceForm>, SoftDeleteInvoiceMappingFactory>();


var app = builder.Build();

// This is purely for seeding data
/*using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<InvoiceDbContext>();
    db.Database.EnsureCreated(); // Optional: auto-create DB if needed
    InvoiceSeeder.SeedTestInvoices(db);
}*/


// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "InvoiceMicroservice v1");
});


app.UseHttpsRedirection();


// Authentication is first AND THEN Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
