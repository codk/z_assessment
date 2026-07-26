using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Products.Domain.Interfaces;
using Products.Infrastructure;
using Products.Infrastructure.Repositories;
using Products.interfaces;
using Products.Services;

using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// define connection string (from appsettings.json or fallback)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                       ?? "Data Source=products.db";

builder.Services.AddDbContext<AppDBContext>(opt =>
    opt.UseNpgsql(connectionString));

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi(x =>
{
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IStockMovementRepository, StockMovementRepository>();
builder.Services.AddScoped<IStockMovementService, StockMovementService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<AppDBContext>();
  db.Database.Migrate();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.MapOpenApi();
  app.MapScalarApiReference();
}

//handle exceptions and return JSON response
app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
  var ex = ctx.Features.Get<IExceptionHandlerFeature>()?.Error;
  ctx.Response.ContentType = "application/json";
  ctx.Response.StatusCode = ex switch
  {
    KeyNotFoundException => 404,
    ArgumentException => 400,
    InvalidOperationException => 409,
    _ => 500
  };
  await ctx.Response.WriteAsJsonAsync(new { error = ex?.Message });
}));

app.UseHttpsRedirection();

app.MapControllers();

app.Run();