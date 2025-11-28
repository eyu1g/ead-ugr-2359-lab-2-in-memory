using Microsoft.EntityFrameworkCore;
using PizzaStore.API.Data;
using PizzaStore.API.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "PizzaStore API",
        Description = "Making the Pizzas you love",
        Version = "v1"
    });
});


builder.Services.AddDbContext<PizzaDb>(options =>
    options.UseInMemoryDatabase("Pizzas"));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PizzaStore API V1");
    });
}

app.UseHttpsRedirection();

// API Endpoints
app.MapGet("/", () => "Welcome to PizzaStore API!");

// Get all pizzas
app.MapGet("/pizzas", async (PizzaDb db) =>
    await db.Pizzas.ToListAsync());

// Get pizza by id
app.MapGet("/pizzas/{id}", async (int id, PizzaDb db) =>
    await db.Pizzas.FindAsync(id)
        is Pizza pizza
            ? Results.Ok(pizza)
            : Results.NotFound());

// Create a new pizza
app.MapPost("/pizzas", async (Pizza pizza, PizzaDb db) =>
{
    db.Pizzas.Add(pizza);
    await db.SaveChangesAsync();
    return Results.Created($"/pizzas/{pizza.Id}", pizza);
});

// Update a pizza
app.MapPut("/pizzas/{id}", async (int id, Pizza updatedPizza, PizzaDb db) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    
    if (pizza is null) return Results.NotFound();
    
    pizza.Name = updatedPizza.Name;
    pizza.Description = updatedPizza.Description;
    
    await db.SaveChangesAsync();
    return Results.NoContent();
});

// Delete a pizza
app.MapDelete("/pizzas/{id}", async (int id, PizzaDb db) =>
{
    var pizza = await db.Pizzas.FindAsync(id);
    
    if (pizza is null) return Results.NotFound();
    
    db.Pizzas.Remove(pizza);
    await db.SaveChangesAsync();
    
    return Results.Ok();
});

app.Run();
