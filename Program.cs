//registering services with DI container
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using DotNetEnv;
using ECommerceWebAPI.Data;
using Microsoft.AspNetCore.Mvc.Versioning;
//using Microsoft.AspNetCore.Mvc.ApiExplorer;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Microsoft.AspNetCore.Mvc.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;

    options.ApiVersionReader = ApiVersionReader.Combine(
        new QueryStringApiVersionReader("api-version"),   // ?api-version=1.0
        new HeaderApiVersionReader("x-api-version")       // Header: x-api-version:1.0
    );
});

//CORS - FOR DEV ONLY!!!, not for production
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()   // Allows any website to call the API - ONLY FOR DEV FOR NOW
                  .AllowAnyMethod()   // Allows GET, POST, PUT, DELETE
                  .AllowAnyHeader();  // Allows custom headers like Content-Type
        });
});

var connectionString = Environment.GetEnvironmentVariable("MONGODB_CONNECTION_STRING");
var databaseName = Environment.GetEnvironmentVariable("MONGODB_DATABASE_NAME");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("CRITICAL: Connection string not found. Check your .env file location!");
}

if (string.IsNullOrEmpty(databaseName))
{
    throw new Exception("CRITICAL: Database name not found. Check your .env file location!");
}

// Add (Registering) services to the container.
//builder.Services.AddDbContext<ProductContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<MongoDbService>(sp => 
    new MongoDbService(connectionString!, databaseName!));

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<OrderRequestValidator>();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseMiddleware<RequestLogger>(); //custom mw for request logging

//Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    //swagger ui 
    app.UseSwaggerUi(options =>
    {
        options.DocumentPath = "/openapi/v1.json";
    });
}


app.UseHttpsRedirection(); //browser does not follow CORS on redirects

app.UseCors("AllowAll"); //must be placed AFTER UseRouting and BEFORE UseAuthorization

app.UseAuthorization();

app.UseExceptionHandler(); //global error handling middleware

app.MapControllers();

app.Run();
