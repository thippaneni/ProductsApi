using Microsoft.EntityFrameworkCore;
using ProductsApi.Database;
using ProductsApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<ProductsDbContext>(opt => opt.UseInMemoryDatabase("Products"));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapGet("/", () => "hello welcome to Products Api");

app.MapGet("/products", async (ProductsDbContext db) => await db.Products.ToListAsync());

app.MapGet("/products/{Id}", async (int Id, ProductsDbContext db) => await db.Products.FindAsync(Id));

app.MapPost("/products", async (Product product, ProductsDbContext db) =>
{
    db.Products.Add(product);
    await db.SaveChangesAsync();
    return Results.Created($"/products/{product.Id}", product);
});

app.MapPut("/products/{Id}", async (int Id, Product newProduct, ProductsDbContext db) =>
{
    var product = await db.Products.FindAsync(Id);
    if (product == null) return Results.NotFound();
    product.Name = newProduct.Name;
    product.Price = newProduct.Price;
    await db.SaveChangesAsync();
    return Results.Accepted();
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.Run();
