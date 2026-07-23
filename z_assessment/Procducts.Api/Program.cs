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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
