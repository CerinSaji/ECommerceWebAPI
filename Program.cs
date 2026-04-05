//registering services with DI container
using Microsoft.EntityFrameworkCore;
using ECommerceWebAPI.Models;
using FluentValidation;
using FluentValidation.AspNetCore;
using DotNetEnv;
using MongoDB.Driver;
using ECommerceWebAPI.Data;

DotNetEnv.Env.Load(); 

var builder = WebApplication.CreateBuilder(args);

//CORS - suited for development, not for production
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()   // Allows any website to call the API
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
// builder.Services.AddDbContext<ProductContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<MongoDbService>(sp => 
    new MongoDbService(connectionString!, databaseName!));

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<OrderRequestValidator>();

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

app.UseCors("AllowAll"); //must be placed AFTER UseRouting and BEFORE UseAuthorization

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
